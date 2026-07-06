using System;
using System.Collections.Generic;
using System.Globalization;

namespace FreelancerModStudio.Data
{
    static class ParameterDescriptions
    {
        static readonly Dictionary<string, string> Descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "nickname", "Unique internal name used by other INI entries to reference this block." },
            { "ids_name", "String resource id for the displayed name." },
            { "strid_name", "String resource id for the displayed name." },
            { "ids_info", "String resource id for the longer infocard or description text." },
            { "ids_short_name", "String resource id for a shortened display name." },
            { "ids_info1", "Additional string resource id used by some equipment and ship infocards." },
            { "ids_info2", "Additional string resource id used by some equipment and ship infocards." },
            { "ids_info3", "Additional string resource id used by some equipment and ship infocards." },
            { "archetype", "Reference to a solar, ship, equipment, or other archetype definition." },
            { "da_archetype", "Path to the model or drawable archetype file used by this entry." },
            { "loadout", "Reference to a loadout that equips ships, solars, or encounters with cargo, weapons, and equipment." },
            { "pilot", "Reference to a pilot entry controlling AI behavior." },
            { "shape", "Zone shape used by the system editor and game engine, such as SPHERE, ELLIPSOID, BOX, RING, or CYLINDER." },
            { "size", "Dimensions for a zone shape. Meaning depends on the shape type." },
            { "pos", "World position. In systems this is usually X, Y, Z." },
            { "rotate", "Euler rotation values, usually pitch, yaw, and roll." },
            { "spin", "Continuous rotation vector and speed." },
            { "star", "Reference to the visual star definition used by sun-type system objects." },
            { "base", "Base nickname associated with this object." },
            { "dock_with", "Base, object, or docking target opened when the player docks here." },
            { "goto", "System, object, and gate tunnel destination used by jump gates and holes." },
            { "parent", "Parent object used by child objects or attached system entities." },
            { "reputation", "Faction reputation assigned to this object." },
            { "behavior", "Behavior mode used by a solar, object, or room script." },
            { "voice", "Voice profile used by NPC chatter or base scenes." },
            { "faction", "Faction reference. Under encounters it sets the faction and weight for that encounter." },
            { "difficulty_level", "Difficulty value used by encounters, NPCs, or objects." },
            { "jump_effect", "Visual effect used while jumping through a gate, hole, or tunnel." },
            { "msg_id_prefix", "String resource prefix used by jump or docking messages." },
            { "tradelane_space_name", "String resource id used for trade lane space name display." },
            { "space_costume", "Costume tuple used for communication with this object." },
            { "property_flags", "Zone visibility and behavior flags used by the navmap and engine." },
            { "property_fog_color", "Fog color applied inside a zone." },
            { "visit", "Visit flag controlling map visibility/discovery state." },
            { "damage", "Damage applied inside a hazard zone." },
            { "music", "Music cue used by a zone, system, room, or base context." },
            { "mission_type", "Mission category allowed or advertised by this zone." },
            { "vignette_type", "Random mission vignette category used by mission generation." },
            { "toughness", "Encounter toughness or random mission difficulty value." },
            { "density", "Population density value for spawning encounters in a zone." },
            { "repop_time", "Time before encounter population can respawn." },
            { "max_battle_size", "Maximum combat population size allowed in a zone." },
            { "pop_type", "Population type used by random mission and encounter logic." },
            { "relief_time", "Delay before relief or replacement ships can spawn." },
            { "population_additive", "Additional population behavior flag used by encounters." },
            { "path_label", "Path label used by patrols, trade lanes, and mission/encounter logic." },
            { "usage", "Usage tag for how a zone is consumed by patrol or mission systems." },
            { "mission_eligible", "Controls whether a zone can be used by random mission generation." },
            { "density_restriction", "Restricts encounter density by legal/illegal or other density classes." },
            { "encounter", "Encounter archetype, difficulty, and density weight used by a population zone." },
            { "spacedust", "Spacedust effect applied inside a zone or system context." },
            { "spacedust_maxparticles", "Maximum number of spacedust particles rendered." },
            { "interference", "Scanner or sensor interference value inside a zone." },
            { "drag_modifier", "Movement drag applied inside a zone." },
            { "edge_fraction", "Fractional edge falloff for zone effects." },
            { "attack_ids", "String resource id used for trade lane attack messages." },
            { "lane_id", "Trade lane identifier." },
            { "tradelane_attack", "Trade lane attack behavior or effect reference." },
            { "tradelane_down", "Trade lane disabled/down behavior or effect reference." },
            { "space_color", "Ambient system space color." },
            { "local_faction", "Local faction used by a system." },
            { "rpop_solar_detection", "Solar detection setting used by random population logic." },
            { "space_farclip", "Maximum effect visibility distance in a system." },
            { "file", "File path referenced by the current section." },
            { "path", "File or data path referenced by the current section." },
            { "zone", "Zone nickname reference." },
            { "system", "System nickname reference." },
            { "room", "Base room nickname reference." },
            { "start_room", "Base room opened first when entering the base." },
            { "ship_repair_cost", "Additional base repair price multiplier or value for hull repair." },
            { "price_variance", "Random price variance multiplier. For example, 0.2 allows roughly +/-20% variance." },
            { "animation_oneshot", "Runs the base room animation once instead of looping." },
            { "tractored_explosion", "Explosion or effect played after tractor collection finishes." },
            { "power_usage", "Energy consumed by a scanner cargo-inspection action." },
            { "dispersion_angle", "Random projectile spread angle for guns, mines, and launcher archetypes." },
            { "shield_collapse_particle", "Effect used when a shield collapses." },
            { "const_effect_delay", "Delay before starting a munition constant effect." },
            { "lgt_time_scale", "Time scale for lighting used by an effect." },
            { "jump_done_effect_player", "Effect played for the player after jump travel finishes." },
            { "jump_done_effect_nonplayer", "Effect played for NPCs after jump travel finishes." },
            { "destroy_parent", "Destroys the parent object when this collision group is destroyed." },
            { "comments", "Comments attached to this INI block." }
        };

        public static string Get(string name, string templateDescription)
        {
            if (!string.IsNullOrEmpty(templateDescription))
            {
                return templateDescription;
            }

            string normalizedName = NormalizeName(name);
            string description;
            if (Descriptions.TryGetValue(normalizedName, out description))
            {
                return description;
            }

            return "No description provided yet.";
        }

        static string NormalizeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            string normalized = name.Trim();
            int lastSpace = normalized.LastIndexOf(' ');
            if (lastSpace > 0)
            {
                string suffix = normalized.Substring(lastSpace + 1);
                int unused;
                if (int.TryParse(suffix, NumberStyles.Integer, CultureInfo.InvariantCulture, out unused))
                {
                    normalized = normalized.Substring(0, lastSpace);
                }
            }

            return normalized;
        }
    }
}
