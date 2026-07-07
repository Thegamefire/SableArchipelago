using com.thegamefire.sablearchipelago.Save;
using HarmonyLib;
using Il2CppInterop.Runtime;
using UIComponents.Layout.AtomicObjects.Buttons.ButtonDerivatives;
using UIComponents.Layout.AtomicObjects.Displays.LoadSaveFileDisplay;
using UnityEngine;
using UnityEngine.UI;

namespace com.thegamefire.sablearchipelago.UI;

[HarmonyPatch(typeof(SaveFileDisplay), nameof(SaveFileDisplay.SetupNewFileButton))]
public class UiSaveFileDisplayPatch
{
    static void Postfix(SaveFileDisplay __instance, int saveFileIndex)
    {
        var list = __instance.saveFileList;
        var last = list.children[^1];
        var btn = last.Cast<SaveFileButtonBase>();

        var save = __instance.saveManager.SaveFiles[saveFileIndex];
        if (save == null || !ApSaveChecker.IsApSave(save.ID))
            return;

        var icon = new GameObject("ApIcon", Il2CppType.Of<Image>());
        icon.transform.SetParent(btn.transform, false);
        var image = icon.GetComponent<Image>();
        image.sprite = UiHelper.GetArchipelagoIcon();

        var trans = icon.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(20, 20);
        trans.anchoredPosition = new Vector2(-57, -66);
    }
}

