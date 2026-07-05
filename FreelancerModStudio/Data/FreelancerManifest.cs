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
        readonly Dictionary<string, string> _dataFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

            string manifestFile;
            if (_dataFiles.TryGetValue("universe", out manifestFile) && SamePath(fullPath, manifestFile))
            {
                return Helper.Template.Data.UniverseFile;
            }

            if (_dataFiles.TryGetValue("solar", out manifestFile) && SamePath(fullPath, manifestFile))
            {
                return Helper.Template.Data.SolarArchetypeFile;
            }

            if (_systemFiles.Contains(fullPath))
            {
                return Helper.Template.Data.SystemFile;
            }

            return -1;
        }

        public string GetSolarArchetypeFile()
        {
            string file;
            if (_dataFiles.TryGetValue("solar", out file) && File.Exists(file))
            {
                return file;
            }

            return null;
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
                            _dataFiles[option.Key] = path;
                        }
                    }
                }
            }

            LoadUniverseSystems();
        }

        void LoadUniverseSystems()
        {
            string universeFile;
            if (!_dataFiles.TryGetValue("universe", out universeFile) || !File.Exists(universeFile))
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
