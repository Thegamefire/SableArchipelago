using System.Collections.Generic;
using com.thegamefire.sablearchipelago.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    private static ChatBox instance;
    
    public float MessageLifetime = 16f;
    public int MaxVisibleMessages = 16;
    public float Width = 1000f;
    public float FontSize = 22f;

    private RectTransform root;

    private readonly Queue<string> pendingMessages = new();
    private readonly List<MessageEntry> activeMessages = new();

    private class MessageEntry
    {
        public GameObject GameObject;
        public float RemainingLifetime;
    }

    public static ChatBox Create()
    {
        if (instance != null)
            return instance;

        GameObject canvasObj = new GameObject("ChatCanvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject root = new GameObject("ChatOverlay");
        root.transform.SetParent(canvas.transform, false);

        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-20, 270);

        VerticalLayoutGroup layout = root.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.LowerRight;
        layout.spacing = 5;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        ContentSizeFitter fitter = root.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        instance = root.AddComponent<ChatBox>();
        instance.root = rect;
        
        rect.sizeDelta = new Vector2(instance.Width, 600);
        
        return instance;
    }

    public static void AddMessage(string message)
    {
        if (instance == null)
            Create();

        instance.InternalAddMessage(message);
    }

    private void Update()
    {
        bool removed = false;

        for (int i = activeMessages.Count - 1; i >= 0; i--)
        {
            activeMessages[i].RemainingLifetime -= Time.deltaTime;

            if (activeMessages[i].RemainingLifetime <= 0)
            {
                Destroy(activeMessages[i].GameObject);
                activeMessages.RemoveAt(i);
                removed = true;
            }
        }

        if (removed)
            FlushQueue();
    }

    private void FlushQueue()
    {
        while (activeMessages.Count < MaxVisibleMessages &&
               pendingMessages.Count > 0)
        {
            CreateMessage(pendingMessages.Dequeue());
        }
    }

    private void InternalAddMessage(string message)
    {
        if (activeMessages.Count >= MaxVisibleMessages)
        {
            pendingMessages.Enqueue(message);
            return;
        }

        CreateMessage(message);
    }

    private void CreateMessage(string message)
    {
        GameObject bg = new GameObject("Message");
        bg.transform.SetParent(root, false);

        Image image = bg.AddComponent<Image>();
        image.color = new Color32(234, 220, 215, 255);

        LayoutElement layout = bg.AddComponent<LayoutElement>();
        layout.preferredWidth = Width;

        ContentSizeFitter bgFitter = bg.AddComponent<ContentSizeFitter>();
        bgFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        HorizontalLayoutGroup padding = bg.AddComponent<HorizontalLayoutGroup>();
        padding.padding = new RectOffset(10, 10, 8, 8);
        padding.childControlHeight = true;
        padding.childControlWidth = true;
        padding.childForceExpandHeight = false;
        padding.childForceExpandWidth = true;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(bg.transform, false);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();

        text.text = message;
        text.fontSize = FontSize;
        text.font = UiHelper.GetEuclidMediumFontAsset();
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.MidlineLeft;
        text.enableWordWrapping = true;
        text.overflowMode = TextOverflowModes.Overflow;

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(Width - 20, 0);

        ContentSizeFitter textFitter = textObj.AddComponent<ContentSizeFitter>();
        textFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // New messages appear at the bottom.
        bg.transform.SetAsLastSibling();

        activeMessages.Add(new MessageEntry
        {
            GameObject = bg,
            RemainingLifetime = MessageLifetime
        });
    }
}