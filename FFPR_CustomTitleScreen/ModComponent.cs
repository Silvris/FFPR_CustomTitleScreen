using BepInEx.Logging;
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

namespace FFPR_CustomTitleScreen
{
    class ModComponent : MonoBehaviour
    {
        public static ModComponent Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }
        private Boolean _isDisabled = false;
        private bool SceneLoaded = false;
        private String _filePath;
        public ModComponent(IntPtr ptr) : base(ptr)
        {
        }
        public void Awake()
        {
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
        bool isScene_CurrentlyLoaded(string sceneName_no_extention)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
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
                texture.filterMode = FilterMode.Point;
                if (!ImageConversion.LoadImage(texture, bytes))
                    throw new NotSupportedException($"Failed to load texture from file [{fullPath}]");

                return texture;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void Update()
        {
            try
            {
                //Log.LogInfo("ModComponent.Update()");
                if (_isDisabled)
                {
                    return;
                }
                if (isScene_CurrentlyLoaded("TitleScreen") && !SceneLoaded)
                {
                    GameObject background = GameObject.Find("background_canvas/ui_root/backgrou_root/background");
                    if(background != null)
                    {
                        Image image = background.GetComponent<Image>();
                        Texture2D tex = ReadTextureFromFile(_filePath + "/customTitleScreen.png", "customTitleScreen");
                        image.sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(), 1f);
                        SceneLoaded = true;
                    }

                }
                if(SceneLoaded && !isScene_CurrentlyLoaded("TitleScreen"))
                {
                    SceneLoaded = false;
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
