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
        public ConfigEntry<Vector3> MenuPositionEntry { get; set; }
        public ConfigEntry<Color> TextColorEntry { get; set; }
        public ConfigEntry<Color> ShadowColorEntry { get; set; }
        public ConfigEntry<Color> FocusTextColorEntry { get; set; }
        public ConfigEntry<Color> FocusShadowColorEntry { get; set; }
        public ConfigEntry<Color> LoadingTextColorEntry { get; set; }
        public ConfigEntry<Color> CopyrightTextColorEntry { get; set; }
        public ConfigEntry<bool> UseVideo { get; set; }
        public ConfigEntry<int> VideoWidth { get; set; }
        public ConfigEntry<int> VideoHeight { get; set; }
        public ConfigEntry<bool> LoopVideo { get; set; }
        public ConfigEntry<double> LoopBeginning { get; set; }

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

            TomlTypeConverter.AddConverter(typeof(Vector3), new TypeConverter()
            {
                ConvertToObject = (string s, Type t) =>
                {
                    var split = s.Split(',');
                    var c = new CultureInfo("en-US");
                    return new Vector3()
                    {
                        x = float.Parse(split[0], c),
                        y = float.Parse(split[1], c),
                        z = float.Parse(split[2], c)
                    };
                },
                ConvertToString = (object o, Type t) =>
                {
                    var x = (Vector3)o;
                    return string.Format(new CultureInfo("en-US"), "{0},{1},{2}", x.x, x.y, x.z);
                }
            });

            MenuPositionEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Menu Position"), new Vector3(0, -376, 0), new ConfigDescription("The local position of the title screen menu options."));
            TextColorEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Text Color"), new Color(0f, 0f, 0f), new ConfigDescription("The color of non-focused text on the title screen."));
            ShadowColorEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Shadow Color"), new Color(0.68f, 0.68f, 0.68f), new ConfigDescription("The color of non-focused text shadows on the title screen."));
            FocusTextColorEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Focused Text Color"), new Color(1f, 1f, 0f), new ConfigDescription("The color of focused text on the title screen."));
            FocusShadowColorEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Focused Shadow Color"), new Color(0f, 0f, 0f), new ConfigDescription("The color of focused text shadows on the title screen."));
            LoadingTextColorEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Loading Text Color"), new Color(0f, 0f, 0f), new ConfigDescription("The color of the initial \"Press Any Key\" on the title screen."));
            CopyrightTextColorEntry = file.Bind(new ConfigDefinition("Title Screen Text", "Copyright Text Color"), new Color(0f, 0f, 0f), new ConfigDescription("The color of the copyright text on the title screen."));
            UseVideo = file.Bind(new ConfigDefinition("Video Settings", nameof(UseVideo)), false, new ConfigDescription("Allow the use of video as the background."));
            VideoWidth = file.Bind(new ConfigDefinition("Video Settings", nameof(VideoWidth)), 1920, new ConfigDescription("The width of the video background.",null,"Advanced"));
            VideoHeight = file.Bind(new ConfigDefinition("Video Settings", nameof(VideoHeight)), 1080, new ConfigDescription("The height of the video background.",null, "Advanced"));
            LoopVideo = file.Bind(new ConfigDefinition("Video Settings", nameof(LoopVideo)), true, new ConfigDescription("Allows the video to loop after finishing."));
            //LoopBeginning = file.Bind(new ConfigDefinition("Video Settings", nameof(LoopBeginning)), 0d, new ConfigDescription("Sets the starting point for the video after looping at the end."));

            MenuPositionEntry.SettingChanged += MenuPosValueChanged;
            CopyrightTextColorEntry.SettingChanged += CopyrightColorValueChanged;
        }

        private void CopyrightColorValueChanged(object sender, EventArgs e)
        {
            ModComponent.Instance.SetCopyrightColor();
        }

        private void MenuPosValueChanged(object sender, EventArgs e)
        {
            ModComponent.Instance.SetMenuPosition();
        }

        public Color TextColor => TextColorEntry.Value;
        public Color ShadowColor => ShadowColorEntry.Value;
        public Color FocusTextColor => FocusTextColorEntry.Value;
        public Color FocusShadowColor => FocusShadowColorEntry.Value;
        public Color CopyrightColor => CopyrightTextColorEntry.Value;
        public Color LoadingColor => LoadingTextColorEntry.Value;
        public Vector3 MenuPosition => MenuPositionEntry.Value;
        public bool bUseVideo => UseVideo.Value;
        public int iVideoWidth => VideoWidth.Value;
        public int iVideoHeight => VideoHeight.Value;
        public bool bLoopVideo => LoopVideo.Value;
        public double dLoopBeginning => LoopBeginning.Value;
    }
}
