using System;
using System.Collections.Generic;
using System.Linq;
using Core.GameManagerStates;
using GameTemplate;
using Il2CppInterop.Runtime;
using Items;
using UnityEngine;


namespace com.thegamefire.sablearchipelago;

public class UiHelper
{
    
    public static Queue<ApPopUp> ToShowPopUpQueue = new Queue<ApPopUp>();

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
}

public record ApPopUp(string Title, string Description);