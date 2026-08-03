using System.Collections.Generic;
using System.Linq;
using Core.GameManagerStates;
using GameTemplate;
using Il2CppInterop.Runtime;
using Items;
using Messages;
using Messaging;
using UnityEngine;

namespace com.thegamefire.sablearchipelago.UI;

public class SUIT : MonoBehaviour
{
    public static SUIT Instance;

    public static void ShowItemQuantityPopUp(string text, Sprite sprite)
    {
        SingletonAsset.Instance<TextureLoader>().inventoryImagesDictionary["CustomItemPopUpIcon"] = sprite;
        ItemDefinition fakeItemDef = new ItemDefinition();
        fakeItemDef.Name = "CustomItemPopUp";
        fakeItemDef.Name_EN = text;
        fakeItemDef.Rarity = Rarity.Rare;
        fakeItemDef.Icon = "CustomItemPopUpIcon";
                
        Item fakeItem = ScriptableObject.CreateInstance<Item>();
        fakeItem.Initialise(fakeItemDef);

        var msg = new ItemQuantityUpdated(
            fakeItem,
            0,
            1
        );
        MessageBus.Instance.Publish(msg);
    }

    private static Queue<ToastMsg> _queuedToasts = new ();

    private record ToastMsg(string Title, string Subtitle, Sprite Sprite);
    
    public static void ShowToastWithButton(string title, string subtitle, Sprite sprite = null)
    {
        sprite ??= UiHelper.GetCircleSprite();
        _queuedToasts.Enqueue(new ToastMsg(title, subtitle, sprite));
    }
    
    private static void CheckToastQueue()
    {
        var stateMachine = Resources.FindObjectsOfTypeAll<StateMachine>().FirstOrDefault((s) => s.name == "GameManager");
        if (_queuedToasts.Count == 0 || stateMachine == null || stateMachine.CurrentStateType != Il2CppType.Of<GameplayState>())
            return;

        var toast = _queuedToasts.Dequeue();
        
        SingletonAsset.Instance<TextureLoader>().inventoryImagesDictionary["CustomToastIcon"] = toast.Sprite;
        
        ItemDefinition fakeItemDef = new ItemDefinition();
        fakeItemDef.Name = "CustomToast";
        fakeItemDef.Name_EN = toast.Title;
        fakeItemDef.Description_EN = toast.Subtitle;
        fakeItemDef.Rarity = Rarity.Rare;
        fakeItemDef.Icon = "CustomToastIcon";
        
        Item fakeItem = ScriptableObject.CreateInstance<Item>();
        fakeItem.Initialise(fakeItemDef);
        
        var itemData = new ChestItemData { item = fakeItem, quantity = 1 };
        var state = new PopUpState(ChestContents.Item, itemData, false);
       
        if (stateMachine != null)
        {
            stateMachine.ForceNextState(state);
        }
    }

    private void Update()
    {
        CheckToastQueue();
    }

    public static void Create()
    {
        if (Instance != null)
            return;
        var go = new GameObject("SUIT Manager");
       
        Instance = go.AddComponent<SUIT>();
    }
}
