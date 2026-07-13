using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FreelancerModStudio.Data.INI;

namespace FreelancerModStudio.Data
{
    public static class IniValidator
    {
        public static List<string> Validate(TableData data, string dataPath)
        {
            List<string> issues = new List<string>();
            Dictionary<string, string> nicknames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, string> pathLegs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (TableBlock tableBlock in data.Blocks)
            {
                EditorINIBlock block = tableBlock.Block;
                string nickname = GetValue(block, "nickname");
                if (!string.IsNullOrEmpty(nickname))
                {
                    string previous;
                    if (nicknames.TryGetValue(nickname, out previous))
                        issues.Add("Duplicate nickname '" + nickname + "' (also used by " + previous + ").");
                    else
                        nicknames[nickname] = block.Name;
                }

                string pathLabel = GetValue(block, "path_label");
                if (!string.IsNullOrEmpty(pathLabel))
                {
                    string previous;
                    if (pathLegs.TryGetValue(pathLabel, out previous))
                        issues.Add("Duplicate path_label '" + pathLabel + "' (also used by " + previous + ").");
                    else
                        pathLegs[pathLabel] = nickname ?? block.Name;
                }

                string position = GetValue(block, "pos");
                if (!string.IsNullOrEmpty(position) && !IsVector(position))
                    issues.Add("Invalid pos vector on " + (nickname ?? block.Name) + ".");

                ValidateFileReferences(block, nickname ?? block.Name, dataPath, issues);
            }

            return issues;
        }

        static void ValidateFileReferences(EditorINIBlock block, string blockName, string dataPath, List<string> issues)
        {
            if (string.IsNullOrEmpty(dataPath))
            {
                return;
            }

            foreach (EditorINIOption option in block.Options)
            {
                if (!option.Name.Equals("file", StringComparison.OrdinalIgnoreCase) &&
                    !option.Name.Equals("filename", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (EditorINIEntry entry in option.Values)
                {
                    string reference = entry.Value == null ? string.Empty : entry.Value.ToString().Trim().Trim('"');
                    if (reference.Length == 0 || reference == "=" || reference.IndexOf('*') != -1)
                    {
                        continue;
                    }

                    string path = Path.IsPathRooted(reference)
                        ? reference
                        : Path.Combine(dataPath, reference.Replace('/', Path.DirectorySeparatorChar));
                    if (!File.Exists(path))
                    {
                        issues.Add("Missing " + option.Name + " reference '" + reference + "' on " + blockName + ".");
                    }
                }
            }
        }

        static string GetValue(EditorINIBlock block, string name)
        {
            foreach (EditorINIOption option in block.Options)
                if (option.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && option.Values.Count > 0)
                    return option.Values[0].Value.ToString().Trim();
            return null;
        }

        static bool IsVector(string value)
        {
            string[] values = value.Split(',');
            double ignored;
            return values.Length == 3 &&
                   double.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out ignored) &&
                   double.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out ignored) &&
                   double.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out ignored);
        }
    }
}
