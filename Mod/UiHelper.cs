using System;
using System.Collections.Generic;
using System.Linq;
using Core.GameManagerStates;
using GameTemplate;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Items;
using MapMagic;
using TMPro;
using UIComponents.Layout.AtomicObjects.Buttons;
using UIComponents.Layout.Components.Containers;
using UIComponents.Layout.Components.Lists;
using UIComponents.Screens;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace com.thegamefire.sablearchipelago;

public class UiHelper
{
    private static TMP_FontAsset _euclidMediumFont;
    private static TMP_FontAsset _euclidRegularFont;
    public static readonly Queue<ApPopUp> ToShowPopUpQueue = new ();

    public static void CheckShowPopUp()
    {
        var stateMachine = Resources.FindObjectsOfTypeAll<StateMachine>().FirstOrDefault((s) => s.name == "GameManager");
        if (ToShowPopUpQueue.Count == 0 || stateMachine == null || stateMachine.CurrentStateType != Il2CppType.Of<GameplayState>())
        {
            if (ToShowPopUpQueue.Count != 0)
            {
            }

            return;
        }
        ApPopUp popUp = ToShowPopUpQueue.Dequeue();
        ShowPopUp(popUp);
    }

    private static void ShowPopUp(ApPopUp popUpData)
    {
        ItemDefinition fakeItemDef = new ItemDefinition();
        fakeItemDef.Name = "ArchipelagoPopup";
        fakeItemDef.Name_EN = popUpData.Title;
        fakeItemDef.Description_EN = popUpData.Description;
        fakeItemDef.Rarity = Rarity.Rare;
        fakeItemDef.Icon = "ArchipelagoIcon";
                
        Item fakeItem = ScriptableObject.CreateInstance<Item>();
        fakeItem.Initialise(fakeItemDef);
                
        var itemData = new ChestItemData { item = fakeItem, quantity = 1 };
        var state = new PopUpState(ChestContents.Item, itemData, false);
                
        var stateMachine = Resources.FindObjectsOfTypeAll<StateMachine>().FirstOrDefault((s) => s.name == "GameManager");
        if (stateMachine != null)
        {
            stateMachine.ForceNextState(state);
        }
    }

    public static Sprite GetArchipelagoIcon()
    {
        var myTexture = LoadEmbeddedTexture("com.thegamefire.sablearchipelago.Assets.APIcon.png");
        return Sprite.Create(
            myTexture,
            new Rect(0, 0, myTexture.width , myTexture.height),
            new Vector2(0.5f, 0.5f)
        );
    }
    
    private static Texture2D LoadEmbeddedTexture(string resourceName)
    {
        try
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    Plugin.Log.LogError($"[SableArchipelago] Embedded resource not found: {resourceName}");
                    return null;
                }

                byte[] fileData = new byte[stream.Length];
                stream.Read(fileData, 0, fileData.Length);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                return tex;
            }
        }
        catch (Exception ex)
        {
            Plugin.Log.LogError($"[SableArchipelago] Error loading embedded texture: {ex.Message}");
            return null;
        }
    }
    
    private static TMP_FontAsset GetEuclidMediumFontAsset()
    {
        if (_euclidMediumFont != null)
            return _euclidMediumFont;

        _euclidMediumFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(b => b.name == "EuclidFlex-Medium SDF");
        if (_euclidMediumFont == null)
            Plugin.Log.LogError("Could not find EuclidFlex-Medium Font");

        return _euclidMediumFont;
    }
    
    private static TMP_FontAsset GetEuclidRegularFontAsset()
    {
        if (_euclidRegularFont != null)
            return _euclidRegularFont;

        _euclidRegularFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(b => b.name == "EuclidFlex-Regular SDF");
        if (_euclidRegularFont == null)
            Plugin.Log.LogError("Could not find EuclidFlex-Medium Font");

        return _euclidRegularFont;
    }
    
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
            
            var image = apButtonGo.GetComponentsInChildren<Image>().FirstOrDefault(i => i.activeSprite!= null && i.activeSprite.name == "CreditsSettingsIcon");
            if (image == null)
            {
                Plugin.Log.LogError("Couldn't find Button Icon");
                return;
            }
            image.sprite = GetArchipelagoIcon();

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
            
            CreateText("Hostname", new Vector2(0, 0), 24, apPanel.transform);
            CreateInput("archipelago.gg:38281", new Vector2(0, 0), apPanel.transform);
            CreateSpacer(5, apPanel.transform);
            
            CreateText("Slot", new Vector2(0, 0), 24, apPanel.transform);
            CreateInput("Player1", new Vector2(0, 0), apPanel.transform);
            CreateSpacer(5, apPanel.transform);
            
            CreateText("Password", new Vector2(0, 0), 24, apPanel.transform);
            CreateInput("", new Vector2(0, 0), apPanel.transform);

            apPanel.SetActive(false);

            var newComponents = new GameObject[components.Length + 1];
            components.CopyTo(newComponents, 0);
            newComponents[^1] = apPanel;
            splitContainer.components = newComponents;
        }

        static TMP_Text CreateText (string text, Vector2 pos, int size, Transform parent)
        {
            
            GameObject obj = new GameObject(text, Il2CppType.Of<TextMeshProUGUI>());
            obj.transform.SetParent(parent, false);
            var tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = Color.black;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.font = GetEuclidMediumFontAsset();
            RectTransform rect = tmp.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(280, 30);
            rect.anchoredPosition = pos;
            
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 35;
            return tmp;
        }
        
        private static TMP_InputField CreateInput(string initial, Vector2 pos, Transform parent)
        {
            GameObject inputObj = new GameObject("InputField", Il2CppType.Of<Image>(), Il2CppType.Of<TMP_InputField>());
            inputObj.transform.SetParent(parent, false);

            Image bg = inputObj.GetComponent<Image>();
            bg.type = Image.Type.Sliced;
            bg.color = new Color(0.91764f, 0.86274f, 0.84313f, 1f);
            RectTransform rect = inputObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 45);
            rect.anchoredPosition = pos;

            GameObject textArea = new GameObject("TextArea", Il2CppType.Of<RectMask2D>());
            textArea.transform.SetParent(inputObj.transform, false);
            RectTransform textRect = textArea.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(5, 5);
            textRect.offsetMax = new Vector2(-5, -5);

            TMP_Text textComp = new GameObject("Text", Il2CppType.Of<TextMeshProUGUI>()).GetComponent<TextMeshProUGUI>();
            textComp.transform.SetParent(textArea.transform, false);
            textComp.text = initial;
            textComp.fontSize = 20;
            textComp.color = Color.black;
            textComp.font = GetEuclidRegularFontAsset();
            textComp.alignment = TextAlignmentOptions.Left;

            RectTransform textCompRect = textComp.GetComponent<RectTransform>();
            textCompRect.anchorMin = Vector2.zero;
            textCompRect.anchorMax = Vector2.one;
            textCompRect.offsetMin = Vector2.zero;
            textCompRect.offsetMax = Vector2.zero;

            TMP_InputField input = inputObj.GetComponent<TMP_InputField>();
            input.textViewport = textRect;
            input.textComponent = textComp;
            input.text = initial;
            input.caretColor = Color.black;

            input.enabled = false;
            input.enabled = true;
            
            var layout = inputObj.AddComponent<LayoutElement>();
            layout.minHeight = 40;

            return input;
        }
        
        private static void CreateSpacer(float height, Transform parent)
        {
            var spacer = new GameObject("Spacer");
            spacer.transform.SetParent(parent, false);

            var layout = spacer.AddComponent<LayoutElement>();
            layout.minHeight = height;
        }
    }
    

}

public record ApPopUp(string Title, string Description);