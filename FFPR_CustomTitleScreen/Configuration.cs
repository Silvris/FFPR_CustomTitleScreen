using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FFPR_CustomTitleScreen
{
    public class Configuration
    {
        public ConfigEntry<Color> TextColorEntry { get; set; }
        public ConfigEntry<Color> ShadowColorEntry { get; set; }
        public ConfigEntry<Color> FocusTextColorEntry { get; set; }
        public ConfigEntry<Color> FocusShadowColorEntry { get; set; }
        private string Section = "Title Screen Text Color";

        public Configuration(ConfigFile file)
        {
            TomlTypeConverter.AddConverter(typeof(Color), new TypeConverter()
            {
                ConvertToObject = (string s, Type t) =>
                {
                    var split = s.Split(',');
                    var c = new CultureInfo("en-US");
                    return new Color()
                    {
                        r = float.Parse(split[0], c),
                        g = float.Parse(split[1], c),
                        b = float.Parse(split[2], c),
                        a = float.Parse(split[3], c)
                    };
                },
                ConvertToString = (object o, Type t) =>
                {
                    var x = (Color)o;
                    return string.Format(new CultureInfo("en-US"), "{0},{1},{2},{3}",
                        x.r, x.g, x.b, x.a);
                }
            });
            TextColorEntry = file.Bind(new ConfigDefinition(Section, "Text Color"), new Color(0f, 0f, 0f), new ConfigDescription("The color of non-focused text on the title screen."));
            ShadowColorEntry = file.Bind(new ConfigDefinition(Section, "Shadow Color"), new Color(0.68f, 0.68f, 0.68f), new ConfigDescription("The color of non-focused text shadows on the title screen."));
            FocusTextColorEntry = file.Bind(new ConfigDefinition(Section, "Focused Text Color"), new Color(1f, 1f, 0f), new ConfigDescription("The color of focused text on the title screen."));
            FocusShadowColorEntry = file.Bind(new ConfigDefinition(Section, "Focused Shadow Color"), new Color(0f, 0f, 0f), new ConfigDescription("The color of focused text shadows on the title screen."));
        }
        public Color TextColor => TextColorEntry.Value;
        public Color ShadowColor => ShadowColorEntry.Value;
        public Color FocusTextColor => FocusTextColorEntry.Value;
        public Color FocusShadowColor => FocusShadowColorEntry.Value;
    }
}
