using System.Collections.Generic;
using com.thegamefire.sablearchipelago.UI;
using HarmonyLib;
using Il2CppInterop.Runtime;
using TMPro;
using UIComponents.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace com.thegamefire.sablearchipelago;

public class ApConnectionIndicator : MonoBehaviour
{
    private TMP_Text _label;
    private Image _circle;

    private static readonly Dictionary<ApConnectionState, (string text, Color color)> _display = new()
    {
        { ApConnectionState.Disconnected, ("Disconnected", new Color(1f, 0.59607f, 0.58823f))   },
        { ApConnectionState.Connecting,   ("Connecting", new Color(0.97647f, 0.90196f, 0.50588f)) },
        { ApConnectionState.Connected,    ("Connected", new Color(0.65098f, 1f, 0.61960f))  },
    };

    public static void Create()
    {
        // One persistent Canvas — survives all scene loads
        var go = new GameObject("ApOverlay");
        DontDestroyOnLoad(go);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        go.AddComponent<CanvasScaler>();

        var bg = new GameObject("AP Connection Indicator", Il2CppType.Of<Image>());
        bg.transform.SetParent(go.transform, false);
        var img = bg.GetComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0);
        var bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = bgRect.anchorMax = new Vector2(0f, 1f);
        bgRect.pivot      = new Vector2(0f, 1f);
        bgRect.anchoredPosition = new Vector2(10f, -10f);
        bgRect.sizeDelta  = new Vector2(240f, 28f);

        // Circle
        var circleGo = new GameObject("CircleIndicator", Il2CppType.Of<Image>());
        circleGo.transform.SetParent(bg.transform, false);
        var circleImg = circleGo.GetComponent<Image>();
        circleImg.color = Color.red;
        circleImg.sprite = UiHelper.GetCircleSprite();
        var circleRect = circleGo.GetComponent<RectTransform>();
        circleRect.sizeDelta = new Vector2(16f, 16f);
        circleRect.anchorMin = new Vector2(0f, 0.5f);
        circleRect.anchorMax = new Vector2(0f, 0.5f);
        circleRect.anchoredPosition = new Vector2(12f, 0f);
        var circleLayout = circleGo.AddComponent(Il2CppType.Of<LayoutElement>()).Cast<LayoutElement>();
        circleLayout.minWidth = 12f;
        circleLayout.minHeight = 12f;
        circleLayout.preferredWidth = 12f;
        circleLayout.preferredHeight = 12f;
        
        // Label
        var labelGo = new GameObject("TextIndicator", Il2CppType.Of<TextMeshProUGUI>());
        labelGo.transform.SetParent(bg.transform, false);
        var tmp = labelGo.GetComponent<TextMeshProUGUI>();
        tmp.fontSize = 24;
        tmp.color = Color.black;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;
        tmp.font = UiHelper.GetEuclidMediumFontAsset();

        var labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(28f, 0f);
        labelRect.offsetMax = Vector2.zero;
        
        Plugin.Log.LogInfo("Adding Indicator Component");
        var indicator = go.AddComponent<ApConnectionIndicator>();
        Plugin.Log.LogInfo("Added Indicator Component");
        indicator._label = tmp;
        indicator._circle = circleImg;
    }

    private void Update()
    {
        UpdateConnectionState(ArchipelagoClient.ConnectionState);

        if (Input.GetKeyDown(KeyCode.F6))
        {
            Plugin.Client.Disconnect();
            UpdateConnectionState(ApConnectionState.Connecting);
            Plugin.Client.Connect();
        }
    }

    private void UpdateConnectionState(ApConnectionState state)
    {
        var (text, color) = _display[state];
        _label.SetText(text);
        _circle.color = color;
    }
}

[HarmonyPatch(typeof(TitleScreen), nameof(TitleScreen.OnOpen))]
public class LoadTitleScreenPatch
{
    private static bool _loaded;
    public static void Postfix()
    {
        if (_loaded)
            return;
        Plugin.Log.LogInfo("Loaded Titlescreen");
        ApConnectionIndicator.Create();
        _loaded = true;
    }
}