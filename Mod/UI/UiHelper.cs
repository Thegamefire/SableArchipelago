using System;
using System.Collections.Generic;
using System.Linq;
using Core.GameManagerStates;
using GameTemplate;
using Il2CppInterop.Runtime;
using Items;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace com.thegamefire.sablearchipelago.UI;

public class UiHelper
{
    private static TMP_FontAsset _euclidMediumFont;
    private static TMP_FontAsset _euclidRegularFont;
    public static readonly Queue<ApPopUp> ToShowPopUpQueue = new ();


    public static Sprite GetArchipelagoIcon()
    {
        var myTexture = LoadEmbeddedTexture("com.thegamefire.sablearchipelago.Assets.APIcon.png");
        return Sprite.Create(
            myTexture,
            new Rect(0, 0, myTexture.width , myTexture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    public static Sprite GetCircleSprite()
    {
        var myTexture = LoadEmbeddedTexture("com.thegamefire.sablearchipelago.Assets.Circle.png");
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
    
    public static TMP_FontAsset GetEuclidMediumFontAsset()
    {
        if (_euclidMediumFont != null)
            return _euclidMediumFont;

        _euclidMediumFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(b => b.name == "EuclidFlex-Medium SDF");
        if (_euclidMediumFont == null)
            Plugin.Log.LogError("Could not find EuclidFlex-Medium Font");

        return _euclidMediumFont;
    }
    
    public static TMP_FontAsset GetEuclidRegularFontAsset()
    {
        if (_euclidRegularFont != null)
            return _euclidRegularFont;

        _euclidRegularFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault(b => b.name == "EuclidFlex-Regular SDF");
        if (_euclidRegularFont == null)
            Plugin.Log.LogError("Could not find EuclidFlex-Medium Font");

        return _euclidRegularFont;
    }
    public static TMP_Text CreateText (string text, Vector2 pos, int size, Transform parent)
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
    
    public static TMP_InputField CreateInput(string initial, Vector2 pos, Transform parent)
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
    
    public static void CreateSpacer(float height, Transform parent)
    {
        var spacer = new GameObject("Spacer");
        spacer.transform.SetParent(parent, false);

        var layout = spacer.AddComponent<LayoutElement>();
        layout.minHeight = height;
    }
    
    public static Toggle CreateToggle(string label, bool initial, Vector2 pos, Transform parent)
    {
        GameObject toggleObj = new GameObject(
            "Toggle",
            Il2CppType.Of<Image>(),
            Il2CppType.Of<Toggle>()
        );

        toggleObj.transform.SetParent(parent, false);

        RectTransform rect = toggleObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(250, 40);
        rect.anchoredPosition = pos;

        Image bg = toggleObj.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0);

        Toggle toggle = toggleObj.GetComponent<Toggle>();
        toggle.isOn = initial;

        
        GameObject textObj = new GameObject("Label", Il2CppType.Of<TextMeshProUGUI>());
        textObj.transform.SetParent(toggleObj.transform, false);

        TMP_Text labelText = textObj.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 24;
        labelText.color = Color.black;
        labelText.font = GetEuclidMediumFontAsset();
        labelText.alignment = TextAlignmentOptions.MidlineLeft;

        RectTransform labelRect = labelText.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = new Vector2(0, 0);
        labelRect.offsetMax = new Vector2(-50, 0);

        
        GameObject boxObj = new GameObject("Box", Il2CppType.Of<Image>());
        boxObj.transform.SetParent(toggleObj.transform, false);

        Image box = boxObj.GetComponent<Image>();
        box.color = new Color(0.91764f, 0.86274f, 0.84313f, 1f);
        box.type = Image.Type.Sliced;

        RectTransform boxRect = box.GetComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(1, 0.5f);
        boxRect.anchorMax = new Vector2(1, 0.5f);
        boxRect.pivot = new Vector2(1, 0.5f);
        boxRect.sizeDelta = new Vector2(25, 25);
        boxRect.anchoredPosition = new Vector2(-5, 0);

        
        GameObject checkObj = new GameObject("Check", Il2CppType.Of<Image>());
        checkObj.transform.SetParent(boxObj.transform, false);

        Image check = checkObj.GetComponent<Image>();
        check.color = Color.black;

        RectTransform checkRect = check.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.5f, 0.5f);
        checkRect.anchorMax = new Vector2(0.5f, 0.5f);
        checkRect.pivot = new Vector2(0.5f, 0.5f);
        checkRect.sizeDelta = new Vector2(12, 12);

        
        toggle.targetGraphic = box;
        toggle.graphic = check;

        checkObj.SetActive(initial);

        toggle.onValueChanged.AddListener((UnityAction<bool>)(value =>
        {
            checkObj.SetActive(value);
        }));

        // force refresh like your input field trick
        toggle.enabled = false;
        toggle.enabled = true;

        // optional layout element
        var layout = toggleObj.AddComponent<LayoutElement>();
        layout.minHeight = 40;

        return toggle;
    }
}

public record ApPopUp(string Title, string Description);