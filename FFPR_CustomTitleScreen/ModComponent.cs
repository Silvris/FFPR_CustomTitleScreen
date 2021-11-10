using BepInEx.Logging;
using Last.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FFPR_CustomTitleScreen
{
    class ModComponent : MonoBehaviour
    {
        public static ModComponent Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }
        public static Configuration Config { get; private set; }
        public static AudioManager audioManager { get; set; }
        private Boolean _isDisabled = false;
        private bool BackgroundLoaded = false;
        private bool LogoLoaded = false;
        private bool BackgroundDisable = false;
        private bool LogoDisable = false;
        private String _filePath;
        public ModComponent(IntPtr ptr) : base(ptr)
        {
        }
        public void Awake()
        {
            Config = new Configuration(EntryPoint.Instance.Config);
            Assembly thisone = Assembly.GetExecutingAssembly();
            _filePath = Path.GetDirectoryName(thisone.Location);
            Log = BepInEx.Logging.Logger.CreateLogSource("FFPR_CustomTitleScreen");
            try
            {
                Instance = this;
                Log.LogMessage($"[{nameof(ModComponent)}].{nameof(Awake)}: Processed successfully.");
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(Awake)}(): {ex}");
                throw;
            }

        }
        public void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            vp.time = Config.dLoopBeginning;
        }
        bool isScene_CurrentlyLoaded(string sceneName_no_extention)
        {
            for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
                if (scene.name == sceneName_no_extention)
                {
                    //the scene is already loaded
                    return true;
                }
            }

            return false;//scene not currently loaded in the hierarchy
        }
        public static Texture2D ReadTextureFromFile(String fullPath, String Name)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(fullPath);
                Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false) { name = Name };
                texture.filterMode = FilterMode.Bilinear;
                if (!ImageConversion.LoadImage(texture, bytes))
                    throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");

                return texture;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void SetMenuPosition()
        {
            GameObject menu = GameObject.Find("menu_canvas/ui_root/notch_root/title(Clone)/content_root/menu");
            if (menu != null)
            {
                menu.transform.localPosition = Config.MenuPosition;
            }
        }
        public void SetCopyrightColor()
        {
            GameObject copyright = GameObject.Find("menu_canvas/ui_root/notch_root/title(Clone)/screen_all_root/safe_root/copy_right_root/last_text");
            if (copyright != null)
            {
                Text text = copyright.GetComponent<Text>();
                text.color = Config.CopyrightColor;
            }
        }
        public string GetVideoPath()
        {
            List<string> extensions = new List<string> {".mp4", ".mov", ".m4v", ".mpg", ".ogv", ".webm", };
            foreach (string extension in extensions)
            {
                if(File.Exists(_filePath + "/customTitleScreen" + extension))
                {
                    return _filePath + "/customTitleScreen" + extension;
                }
            }
            return "";
        }
        public void Update()
        {
            if(audioManager == null)
            {
                GameObject commonSingleton = GameObject.Find("CommonSingletonObject/Audio");
                if(commonSingleton != null)
                {
                    audioManager = commonSingleton.GetComponent<AudioManager>();
                    //Log.LogInfo("Audio Manager retrieved!");
                    //this will be useful once we are able to get sound from videos working, as we can use this to pause/play the OST
                }
            }
            try
            {
                //Log.LogInfo("ModComponent.Update()");
                if (_isDisabled)
                {
                    return;
                }
                bool sceneLoaded = isScene_CurrentlyLoaded("TitleScreen");
                if (sceneLoaded && (!BackgroundLoaded || !LogoLoaded))
                {
                    SetMenuPosition();
                    SetCopyrightColor();
                    if(!BackgroundDisable && !BackgroundLoaded)
                    {
                        GameObject background = GameObject.Find("background_canvas/ui_root/backgrou_root/background");
                        if (background != null)
                        {
                            string moviePath = GetVideoPath();
                            if (Config.bUseVideo && (moviePath != ""))
                            {
                                Destroy(background.GetComponent<Image>());//I think Destroy null won't be a problem?
                                background.AddComponent<RawImage>();
                                RawImage image = background.GetComponent<RawImage>();
                                //Log.LogInfo(image);
                                if (image is null)
                                {
                                    Log.LogInfo("Failed to set RawImage."); //this one needs to not disable, as image takes time to create apparently
                                }
                                else
                                {
                                    //Log.LogInfo("Creating and Setting RenderTexture");
                                    RenderTexture tex = new RenderTexture(Config.iVideoWidth, Config.iVideoHeight, 1) { name = "CustomTitleScreen" };
                                    tex.hideFlags = HideFlags.HideAndDontSave;
                                    image.texture = tex;
                                    EntryPoint.Player.targetTexture = tex;
                                    EntryPoint.Player.source = VideoSource.Url;
                                    EntryPoint.Player.url = moviePath;
                                    EntryPoint.Player.isLooping = Config.bLoopVideo;
                                    //EntryPoint.Player.loopPointReached = EndReached;
                                    EntryPoint.Player.Play();
                                    //Log.LogInfo("Playing video");
                                    BackgroundLoaded = true;
                                }
                            }
                            else
                            {
                                if(File.Exists(_filePath + "/customTitleScreen.png")){
                                    Image image = background.GetComponent<Image>();
                                    if(image != null)
                                    {
                                        Texture2D tex = ReadTextureFromFile(_filePath + "/customTitleScreen.png", "customTitleScreen");
                                        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(), 1f);
                                        BackgroundLoaded = true;
                                    }
                                }
                                else
                                {
                                    if (!BackgroundLoaded)
                                    {
                                        Log.LogWarning($"File not found:{_filePath + "/customTitleScreen.png"}");
                                        BackgroundDisable = true;
                                    }
                                }
                            }

                        }
                    }
                    if(!LogoDisable && !LogoLoaded)
                    {
                        if(File.Exists(_filePath + "/customLogo.png"))
                        {
                            GameObject titleLogo = GameObject.Find("menu_canvas/ui_root/notch_root/title(Clone)/content_root/title_image");
                            if (titleLogo != null)
                            {
                                Image image = titleLogo.GetComponent<Image>();
                                Texture2D tex = ReadTextureFromFile(_filePath + "/customLogo.png", "customLogo");
                                image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(), 1f);
                                LogoLoaded = true;
                            }
                        }
                        else
                        {
                            if (!LogoLoaded)
                            {
                                Log.LogWarning($"File not found:{_filePath + "/customLogo.png"}");
                                LogoDisable = true;
                            }

                        }
                    }
                    
                }
                if((BackgroundLoaded || LogoLoaded) && !sceneLoaded)
                {
                    BackgroundLoaded = false;
                    LogoLoaded = false;
                    BackgroundDisable = false;
                    LogoDisable = false;
                }
            }
            catch (Exception ex)
            {
                _isDisabled = true;
                Log.LogError($"[{nameof(ModComponent)}].{nameof(Update)}(): {ex}");
                throw;
            }

        }
    }
}
