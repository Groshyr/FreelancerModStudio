using System;
using System.Collections.Generic;
using System.IO;
using FreelancerModStudio.Data.INI;
using FreelancerModStudio.Data.IO;

namespace FreelancerModStudio.Data
{
    public class FreelancerManifest
    {
        const string FreelancerIniPath = "EXE\\freelancer.ini";

        static readonly object CacheLock = new object();
        static readonly Dictionary<string, FreelancerManifest> Cache = new Dictionary<string, FreelancerManifest>(StringComparer.OrdinalIgnoreCase);

        readonly string _dataPath;
        readonly string _freelancerIni;
        DateTime _freelancerIniLastWriteUtc;
        DateTime _universeIniLastWriteUtc;
        string _universeIni;

        // Stores all values per key (e.g. multiple "solar =" lines) in load order.
        readonly Dictionary<string, List<string>> _dataFiles = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> _systemFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        FreelancerManifest(string rootPath)
        {
            _dataPath = Path.Combine(rootPath, "DATA");
            _freelancerIni = Path.Combine(rootPath, FreelancerIniPath);
        }

        public string DataPath
        {
            get { return _dataPath; }
        }

        public static FreelancerManifest FromFile(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return null;
            }

            string directory = File.Exists(file) ? Path.GetDirectoryName(Path.GetFullPath(file)) : Path.GetFullPath(file);
            while (!string.IsNullOrEmpty(directory))
            {
                string freelancerIni = Path.Combine(directory, FreelancerIniPath);
                if (File.Exists(freelancerIni))
                {
                    return GetCachedManifest(directory);
                }

                DirectoryInfo parent = Directory.GetParent(directory);
                directory = parent == null ? null : parent.FullName;
            }

            return null;
        }

        /// <summary>
        /// Clears the in-memory manifest cache. Normally this is unnecessary because cached
        /// manifests are refreshed automatically when freelancer.ini or universe.ini changes.
        /// </summary>
        public static void ClearCache()
        {
            lock (CacheLock)
            {
                Cache.Clear();
            }
        }

        static FreelancerManifest GetCachedManifest(string rootPath)
        {
            string key = Normalize(rootPath);
            lock (CacheLock)
            {
                FreelancerManifest manifest;
                if (Cache.TryGetValue(key, out manifest) && manifest.IsCurrent())
                {
                    return manifest;
                }

                manifest = new FreelancerManifest(key);
                manifest.Load();
                Cache[key] = manifest;
                return manifest;
            }
        }

        public int GetTemplateIndex(string file)
        {
            string fullPath = Normalize(file);
            if (fullPath == null)
            {
                return -1;
            }

            // Check universe file
            List<string> universeFiles;
            if (_dataFiles.TryGetValue("universe", out universeFiles))
            {
                foreach (string uf in universeFiles)
                {
                    if (SamePath(fullPath, uf))
                    {
                        return Helper.Template.Data.UniverseFile;
                    }
                }
            }

            // Check all solar archetype files (mods may list multiple)
            List<string> solarFiles;
            if (_dataFiles.TryGetValue("solar", out solarFiles))
            {
                foreach (string sf in solarFiles)
                {
                    if (SamePath(fullPath, sf))
                    {
                        return Helper.Template.Data.SolarArchetypeFile;
                    }
                }
            }

            if (_systemFiles.Contains(fullPath))
            {
                return Helper.Template.Data.SystemFile;
            }

            return -1;
        }

        /// <summary>
        /// Returns the first existing solar archetype file listed in freelancer.ini.
        /// Use <see cref="GetAllSolarArchetypeFiles"/> when you need to load all of them.
        /// </summary>
        public string GetSolarArchetypeFile()
        {
            return GetFirstExistingDataFile("solar");
        }

        /// <summary>
        /// Returns every existing solar archetype file listed in freelancer.ini, in load order.
        /// Mods such as Discovery list multiple "solar =" entries whose archetypes must all be
        /// merged to resolve object types correctly.
        /// </summary>
        public List<string> GetAllSolarArchetypeFiles()
        {
            return GetExistingDataFiles("solar");
        }

        /// <summary>
        /// Returns every existing file declared for a <c>[Data]</c> key, in the same order as
        /// freelancer.ini. This is intentionally generic: many keys (for example equipment,
        /// ships and effects) can occur more than once and must never be collapsed to one path.
        /// </summary>
        public List<string> GetExistingDataFiles(string key)
        {
            List<string> result = new List<string>();
            List<string> files;
            if (!string.IsNullOrEmpty(key) && _dataFiles.TryGetValue(key, out files))
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        result.Add(file);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the first existing file declared for a <c>[Data]</c> key, or null.
        /// </summary>
        public string GetFirstExistingDataFile(string key)
        {
            List<string> files = GetExistingDataFiles(key);
            return files.Count > 0 ? files[0] : null;
        }

        void Load()
        {
            if (!File.Exists(_freelancerIni))
            {
                return;
            }

            foreach (INIBlock block in new INIManager(_freelancerIni).Read())
            {
                if (!block.Name.Equals("Data", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (KeyValuePair<string, List<INIOption>> option in block.Options)
                {
                    foreach (INIOption value in option.Value)
                    {
                        string path = ResolveDataPath(value.Value);
                        if (path != null)
                        {
                            // Append rather than overwrite so that every entry is preserved
                            // (e.g. multiple "solar =" lines in Discovery's freelancer.ini).
                            List<string> existing;
                            if (!_dataFiles.TryGetValue(option.Key, out existing))
                            {
                                existing = new List<string>();
                                _dataFiles[option.Key] = existing;
                            }

                            existing.Add(path);
                        }
                    }
                }
            }

            LoadUniverseSystems();
            CaptureFileVersions();
        }

        bool IsCurrent()
        {
            if (!File.Exists(_freelancerIni) || File.GetLastWriteTimeUtc(_freelancerIni) != _freelancerIniLastWriteUtc)
            {
                return false;
            }

            if (string.IsNullOrEmpty(_universeIni))
            {
                return true;
            }

            return File.Exists(_universeIni) && File.GetLastWriteTimeUtc(_universeIni) == _universeIniLastWriteUtc;
        }

        void CaptureFileVersions()
        {
            _freelancerIniLastWriteUtc = File.GetLastWriteTimeUtc(_freelancerIni);
            _universeIni = GetFirstExistingDataFile("universe");
            _universeIniLastWriteUtc = string.IsNullOrEmpty(_universeIni) ? DateTime.MinValue : File.GetLastWriteTimeUtc(_universeIni);
        }

        void LoadUniverseSystems()
        {
            List<string> universeFiles = GetExistingDataFiles("universe");
            if (universeFiles.Count == 0)
            {
                return;
            }

            // Use the first universe file (there is only ever one in standard FL).
            string universeFile = universeFiles[0];
            if (!File.Exists(universeFile))
            {
                return;
            }

            string universeDirectory = Path.GetDirectoryName(universeFile);
            foreach (INIBlock block in new INIManager(universeFile).Read())
            {
                if (!block.Name.Equals("system", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                List<INIOption> files;
                if (!block.Options.TryGetValue("file", out files))
                {
                    continue;
                }

                foreach (INIOption file in files)
                {
                    string path = Normalize(Path.Combine(universeDirectory, file.Value));
                    if (path != null)
                    {
                        _systemFiles.Add(path);
                    }
                }
            }
        }

        string ResolveDataPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            string fullPath = Path.IsPathRooted(path) ? path : Path.Combine(_dataPath, path);
            return Normalize(fullPath);
        }

        static string Normalize(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        static bool SamePath(string left, string right)
        {
            return string.Equals(Normalize(left), Normalize(right), StringComparison.OrdinalIgnoreCase);
        }
    }
}
