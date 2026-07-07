using System;
using System.Collections.Generic;
using System.IO;
using FreelancerModStudio.Data.INI;
using FreelancerModStudio.Data.IO;
using FreelancerModStudio.SystemPresenter;

namespace FreelancerModStudio.Data
{
    public class ArchetypeManager
    {
        Dictionary<string, ArchetypeInfo> _archetypes;

        /// <summary>
        /// Loads archetypes from a single solar archetype file.
        /// </summary>
        public ArchetypeManager(string file, int templateIndex)
        {
            if (file == null)
            {
                return;
            }

            _archetypes = new Dictionary<string, ArchetypeInfo>(StringComparer.OrdinalIgnoreCase);

            FileManager fileManager = new FileManager(file);
            EditorINIData iniContent = fileManager.Read(FileEncoding.Automatic, templateIndex);
            MergeContentTable(iniContent.Blocks);
        }

        /// <summary>
        /// Loads and merges archetypes from multiple solar archetype files in the given order.
        /// Later files override earlier ones when the same nickname is found in both, matching
        /// the engine's own load order behaviour.
        /// </summary>
        public ArchetypeManager(List<string> files, int templateIndex)
        {
            _archetypes = new Dictionary<string, ArchetypeInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (string file in files)
            {
                if (file == null)
                {
                    continue;
                }

                FileManager fileManager = new FileManager(file);
                EditorINIData iniContent = fileManager.Read(FileEncoding.Automatic, templateIndex);
                MergeContentTable(iniContent.Blocks);
            }
        }

        public ArchetypeInfo TypeOf(string archetype)
        {
            ArchetypeInfo info;
            if (_archetypes != null && _archetypes.TryGetValue(archetype, out info))
            {
                return info;
            }

            return null;
        }

        /// <summary>
        /// Returns the first solar archetype file resolvable from the given system/universe file.
        /// Prefer <see cref="GetAllRelativeArchetypes(string,int)"/> when loading archetypes so that
        /// all files listed in freelancer.ini are included.
        /// </summary>
        public static string GetRelativeArchetype(string file, int fileTemplate)
        {
            return GetRelativeArchetype(Helper.Template.Data.GetDataPath(file, fileTemplate));
        }

        /// <summary>
        /// Returns the first solar archetype file resolvable from the given DATA directory.
        /// </summary>
        public static string GetRelativeArchetype(string dataPath)
        {
            if (dataPath != null)
            {
                FreelancerManifest manifest = FreelancerManifest.FromFile(dataPath);
                if (manifest != null)
                {
                    string manifestArchetypePath = manifest.GetSolarArchetypeFile();
                    if (manifestArchetypePath != null)
                    {
                        return manifestArchetypePath;
                    }
                }

                string archetypePath = Path.Combine(dataPath, Path.Combine("Solar", "SolarArch.ini"));
                if (File.Exists(archetypePath))
                {
                    return archetypePath;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns ALL solar archetype files listed in the mod's freelancer.ini that sits above
        /// <paramref name="file"/>. Mods such as Discovery list multiple "solar =" entries;
        /// loading only the last one causes most objects to be invisible in the 3D view.
        /// Falls back to the single-file convention when no manifest is found.
        /// </summary>
        public static List<string> GetAllRelativeArchetypes(string file, int fileTemplate)
        {
            return GetAllRelativeArchetypes(Helper.Template.Data.GetDataPath(file, fileTemplate));
        }

        /// <summary>
        /// Returns ALL solar archetype files listed in the mod's freelancer.ini that sits above
        /// <paramref name="dataPath"/>.
        /// </summary>
        public static List<string> GetAllRelativeArchetypes(string dataPath)
        {
            if (dataPath != null)
            {
                FreelancerManifest manifest = FreelancerManifest.FromFile(dataPath);
                if (manifest != null)
                {
                    List<string> manifestFiles = manifest.GetAllSolarArchetypeFiles();
                    if (manifestFiles.Count > 0)
                    {
                        return manifestFiles;
                    }
                }

                // Fallback: no freelancer.ini found — try the standard vanilla location.
                string archetypePath = Path.Combine(dataPath, Path.Combine("Solar", "SolarArch.ini"));
                if (File.Exists(archetypePath))
                {
                    return new List<string> { archetypePath };
                }
            }

            return new List<string>();
        }

        /// <summary>
        /// Adds archetypes from <paramref name="blocks"/> into the shared dictionary.
        /// Does not reinitialise the dictionary so that it can be called multiple times
        /// to merge archetypes from several files.
        /// </summary>
        void MergeContentTable(List<EditorINIBlock> blocks)
        {
            foreach (EditorINIBlock block in blocks)
            {
                KeyValuePair<string, ArchetypeInfo> info = SystemParser.GetArchetypeInfo(block);
                if (info.Key != null)
                {
                    _archetypes[info.Key] = info.Value;
                }
            }
        }
    }
}
