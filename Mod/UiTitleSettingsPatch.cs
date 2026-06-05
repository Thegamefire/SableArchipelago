using System.Linq;
using HarmonyLib;
using MapMagic;
using UIComponents.Layout.AtomicObjects.Buttons;
using UIComponents.Layout.Components.Containers;
using UIComponents.Layout.Components.Lists;
using UIComponents.Screens;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.thegamefire.sablearchipelago;

[HarmonyPatch(typeof(TitleSettingsScreen), nameof(TitleScreen.OnOpen))]
public static class OpenTitleSettingsPatch
{
    private static bool _buttonAdded = false;

    static void Postfix(TitleSettingsScreen __instance)
    {
        if (_buttonAdded) return;

        var allButtons = __instance.GetComponentsInChildren<UiSelectableButton>();
        var creditsButton = allButtons
            .FirstOrDefault(b => b.gameObject.name.ToLower().Contains("credit"));
        if (creditsButton == null)
        {
            Plugin.Log.LogError("Could not find CreditsButton field");
            return;
        }

        var apButtonGo = Object.Instantiate(creditsButton.gameObject,
            creditsButton.transform.parent);

        apButtonGo.transform.SetSiblingIndex(creditsButton.transform.GetSiblingIndex() + 1);
        apButtonGo.name = "ArchipelagoButton";

        var image = apButtonGo.GetComponentsInChildren<Image>().FirstOrDefault(i =>
            i.activeSprite != null && i.activeSprite.name == "CreditsSettingsIcon");
        if (image == null)
        {
            Plugin.Log.LogError("Couldn't find Button Icon");
            return;
        }

        image.sprite = UiHelper.GetArchipelagoIcon();

        var apButton = apButtonGo.GetComponent<UiSelectableButton>();
        if (apButton == null)
        {
            Plugin.Log.LogError("Cloned GameObject has no UiSelectableButton");
            return;
        }

        apButton.SetName("ArchipelagoButton");
        apButton.Initialize();
        apButton.buttonText.SetText("Archipelago");

        var uiLists = Resources.FindObjectsOfTypeAll<UiList>();
        foreach (var uiList in uiLists)
        {
            if (uiList != null && uiList.name == "List")
            {
                uiList.RefreshChildren();
                uiList.InitializeChildren();
                uiList.InitializeEvents();
            }
        }

        _buttonAdded = true;

        var splitContainer = __instance.GetComponentInChildren<UiSplitInputContainer>();

        var components = splitContainer.components;

        var existingPanel = components[^1];
        if (existingPanel == null)
        {
            Plugin.Log.LogError("[SableArchipelago] Failed To Create Custom Settings Panel");
            return;
        }

        var apPanel = Object.Instantiate(existingPanel, existingPanel.transform.parent);
        apPanel.name = "ArchipelagoPanel";


        apPanel.transform.RemoveChildren();

        var panelUiList = apPanel.GetComponentInChildren<UiList>();
        if (panelUiList != null)
        {
            panelUiList.RefreshChildren();
            panelUiList.InitializeChildren();
            panelUiList.InitializeEvents();
        }

        UiHelper.CreateText("Hostname", new Vector2(0, 0), 24, apPanel.transform);
        UiHelper.CreateInput(Plugin.ConfigApHost.Value, new Vector2(0, 0), apPanel.transform)
            .onEndEdit.AddListener((UnityAction<string>)(value => Plugin.ConfigApHost.Value = value));
        UiHelper.CreateSpacer(5, apPanel.transform);

        UiHelper.CreateText("Slot", new Vector2(0, 0), 24, apPanel.transform);
        UiHelper.CreateInput(Plugin.ConfigApSlot.Value, new Vector2(0, 0), apPanel.transform)
            .onEndEdit.AddListener((UnityAction<string>)(value => Plugin.ConfigApSlot.Value = value));
        UiHelper.CreateSpacer(5, apPanel.transform);

        UiHelper.CreateText("Password", new Vector2(0, 0), 24, apPanel.transform);
        UiHelper.CreateInput(Plugin.ConfigApPassword.Value, new Vector2(0, 0), apPanel.transform)
            .onEndEdit.AddListener((UnityAction<string>)(value => Plugin.ConfigApPassword.Value = value));
        UiHelper.CreateSpacer(5, apPanel.transform);

        UiHelper.CreateToggle("DeathLink", Plugin.ConfigApDeathlink.Value, new Vector2(0, 0), apPanel.transform)
            .onValueChanged.AddListener((UnityAction<bool>)
                (value => Plugin.ConfigApDeathlink.Value = value)
            );
        UiHelper.CreateToggle("FastTravel on Death", false, new Vector2(0, 0), apPanel.transform);

        apPanel.SetActive(false);

        var newComponents = new GameObject[components.Length + 1];
        components.CopyTo(newComponents, 0);
        newComponents[^1] = apPanel;
        splitContainer.components = newComponents;
    }
}