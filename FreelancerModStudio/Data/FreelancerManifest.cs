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

        readonly string _dataPath;
        readonly string _freelancerIni;

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
                    FreelancerManifest manifest = new FreelancerManifest(directory);
                    manifest.Load();
                    return manifest;
                }

                DirectoryInfo parent = Directory.GetParent(directory);
                directory = parent == null ? null : parent.FullName;
            }

            return null;
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
            List<string> files;
            if (_dataFiles.TryGetValue("solar", out files))
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                    {
                        return file;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns every existing solar archetype file listed in freelancer.ini, in load order.
        /// Mods such as Discovery list multiple "solar =" entries whose archetypes must all be
        /// merged to resolve object types correctly.
        /// </summary>
        public List<string> GetAllSolarArchetypeFiles()
        {
            List<string> result = new List<string>();
            List<string> files;
            if (_dataFiles.TryGetValue("solar", out files))
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
        }

        void LoadUniverseSystems()
        {
            List<string> universeFiles;
            if (!_dataFiles.TryGetValue("universe", out universeFiles) || universeFiles.Count == 0)
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
