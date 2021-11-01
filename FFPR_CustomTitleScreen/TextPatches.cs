using HarmonyLib;
using Last.UI.KeyInput;
using Last.UI.Touch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace FFPR_CustomTitleScreen
{
    //[HarmonyPatch(typeof(TitleCommandContentView),nameof(TitleCommandContentView.SetCommandFocus))]
    class TitleCommandContentView_SetCommandFocus
    {
        public static void Postfix(bool isFocus, TitleCommandContentView __instance)
        {
            Text text = __instance.nameText;
            text.color = ModComponent.Config.TextColor;
        }
    }

    //[HarmonyPatch(typeof(TitleCommandContentController), nameof(TitleCommandContentController.SetTextColor))]
    class TitleCommandContentController_SetTextColor
    {
        public static void Postfix(bool isEnable, TitleCommandContentView __instance)
        {
            Text text = __instance.nameText;
            text.color = ModComponent.Config.FocusTextColor;
            Shadow shadow = __instance.shadow;
            shadow.effectColor = ModComponent.Config.FocusShadowColor;
        }
    }

    [HarmonyPatch(typeof(TitleCommandContentView), nameof(TitleCommandContentView.SetFocus))]
    class TitleCommandContentView_SetFocus
    {
        public static void Postfix(bool isFocus, TitleCommandContentView __instance)
        {
            Text text = __instance.nameText;
            Shadow shadow = __instance.shadow;
            if (isFocus)
            {
                text.color = ModComponent.Config.FocusTextColor;
                shadow.effectColor = ModComponent.Config.FocusShadowColor;
            }
            else
            {
                text.color = ModComponent.Config.TextColor;
                shadow.effectColor = ModComponent.Config.ShadowColor;
            }

        }
    }
}
