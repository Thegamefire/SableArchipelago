using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CollectiblesBehaviour;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Items;
using Locations;
using Opencoding.Console;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Sable.exe")]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    //////////////// Config //////////////////
    internal static ConfigEntry<string> ConfigApHost;
    internal static ConfigEntry<string> ConfigApSlot;
    internal static ConfigEntry<string> ConfigApPassword;
    internal static ConfigEntry<bool> ConfigApDeathlink;
    internal static ConfigEntry<bool> ConfigApDeathlinkIsFastTravelMode;

    internal static Dictionary<string, string> ChumNameMap = UtilityMappings.LoadChumDictionary();
    internal static HashSet<string> NonRandomizedItems = UtilityMappings.NonRandomizedItems();
    
    internal static ArchipelagoClient Client;

    // Values To Be Listened To Next Frame //
    internal static bool DeathReceived = false;
    internal static bool ReceivingItem = false;
    internal static Queue<string> ReceivedItemsQueue = new Queue<string>();
    internal static bool SableWasExhausted = false;
    
    internal static Vector3 lastNamedLocation = new Vector3();

    internal static bool ancientRingCollected = false;
    internal static bool RegisteredCustomIcon = false;
    
    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
     
        ClassInjector.RegisterTypeInIl2Cpp<ApConnectionIndicator>();
        
        Log.LogInfo("Loading config...");
        LoadConfig();
        Log.LogInfo("Config loaded!");
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);
        
        Log.LogInfo("Connecting to Archipelago Server...");
        Client = new ArchipelagoClient();
        Client.Connect();
    }

    private void LoadConfig()
    {
        ConfigApHost = Config.Bind("Archipelago Connection",
            "AP_Host",
            "localhost:38281",
            "The ip (and port) of the archipelago server to connect to, usually in this is something in the form of archipelago.gg:<port>");
        ConfigApSlot = Config.Bind("Archipelago Connection",
            "AP_Slot",
            "Player1",
            "The name of the slot (player) you want to connect as.");
        ConfigApPassword = Config.Bind("Archipelago Connection",
            "AP_Password",
            "",
            "The password of the archipelago server, if there is no password leave this empty.");

        ConfigApDeathlink = Config.Bind("Archipelago Connection", "AP_Deathlink", true,
            "Whether to share deaths among players");
        
        ConfigApDeathlinkIsFastTravelMode = Config.Bind("Archipelago Connection", "AP_Deathlink_FastTravel", false,
            "Whether you be sent back to the last visited named location instead of depleting your stamina on deathlink.");
        
    }

    private static void LogAllItemsInGame()
    {
        ItemDatabase itemDb = SingletonAsset.Instance<ItemDatabase>();
        var sb = new System.Text.StringBuilder();
        foreach (var item in itemDb.Items)
        {
            if (item?.ItemDef == null)
                continue;
            sb.Append("'");
            sb.Append(item.ItemDef.Name);
            sb.Append("': '");
            sb.Append(item.ItemDef.Name_EN);
            sb.Append("', \n");
        }
        Log.LogMessage("Items: \n" + sb.ToString());
    }

    public static void ReceiveItem(string itemName)
    {
        ReceivedItemsQueue.Enqueue(itemName);
        Log.LogMessage($"Added Item To Queue: {itemName} | Quecount is {ReceivedItemsQueue.Count}");
    }

    public static bool IsReceivingItem()
    {
        return Plugin.ReceivingItem;
    }

    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.Add), new [] { typeof(Items.Item), typeof(int) })]
    static class InventoryAddPatch
    {
        static bool Prefix(PlayerInventory __instance, Items.Item item, Int32 quantity)
        { // Received Items are still seen as checks and not given
            if (Plugin.IsReceivingItem() || Plugin.NonRandomizedItems.Contains(item.ItemDef.Name))
            {
                Plugin.ReceivingItem = false;
                Log.LogMessage($"Received Item: {item.itemDef.Name}");
                return true;
            }

            if (item.ItemDef.Name == "Chum")
            {
                return false;
            }
            else if (item.ItemDef.Name == "AnAncientRaceKeyItem")
            {
                Plugin.ancientRingCollected = true;
                return false;
            }

            Plugin.Client.SendLocation(item.itemDef.Name_EN);

            return false;
        }
    }

    [HarmonyPatch(typeof(ChumBehaviour), nameof(ChumBehaviour.Use))]
    static class OnChumUse
    {
        static void Prefix(ChumBehaviour __instance)
        {
            Client.SendChum(__instance);
        }
    }

    // [HarmonyPatch(typeof(ChumBehaviour), nameof(ChumBehaviour.SendMessage))]
    // static class BlockChumMessage
    // {
    //     static bool Prefix(ChumBehaviour __instance)
    //     {
    //         Log.LogMessage("Blocking Send Chum Message");
    //         __instance.EndTimeline(); // This only unlocks the camera, not the player
    //
    //         return false;
    //     }
    // }


    [HarmonyPatch(typeof(SableCharacterController), nameof(SableCharacterController.Update))]
    static class OnFrame
    {
        static void Prefix(SableCharacterController __instance)
        {
            if (!RegisteredCustomIcon)
            {
                SingletonAsset.Instance<TextureLoader>().inventoryImagesDictionary.Add("ArchipelagoIcon", UiHelper.GetArchipelagoIcon());
                RegisteredCustomIcon = true;
            }
            UiHelper.CheckShowPopUp();

            if (ReceivedItemsQueue.Count > 0)
            {
                string itemName = ReceivedItemsQueue.Dequeue();
                
                GameObject playerInventoryParent = new GameObject("PlayerInventoryUtilityParent");
                PlayerInventoryUtility inventoryUtility = playerInventoryParent.AddComponent<PlayerInventoryUtility>();
                Item item = SingletonAsset.Instance<ItemDatabase>().GetItemFromName(itemName);
                Plugin.ReceivingItem = true;
                inventoryUtility.AddItemToInventory(item, 1);
            }

            if (DeathReceived)
            {
                Log.LogMessage("Deathlink Received");
                if (ConfigApDeathlinkIsFastTravelMode.Value)
                {
                    if (Plugin.lastNamedLocation != new Vector3())
                    {
                        DebugCommands.FastTravelToCoords(Plugin.lastNamedLocation);
                    }
                }
                else
                {
                    __instance.CurrentClimbDistance = 100f;
                    SableWasExhausted = true;
                }

                DeathReceived = false; 
            }

            if (SableWasExhausted != __instance.Exhausted)
            {
                if (__instance.Exhausted)
                {
                    Client.SendDeath();
                }
                SableWasExhausted = __instance.Exhausted;
            }

            if (ancientRingCollected)
            {
                Log.LogMessage($"Hicaric Ring Collected at {__instance.transform.position}");
                Plugin.Client.SendHicaricRing(__instance.transform.position);
                Plugin.ancientRingCollected = false;
            }
        }
    }

    [HarmonyPatch(typeof(DebugConsole), "set_IsVisible")]
    class ConsoleVisibilityPatch
    {
        // When using DebugCommands, DebugConsole.set_IsVisible gives an error because the debug console
        // is not correctly initialized. Therefore, we simply stop the method from getting called.
        static bool Prefix(DebugConsole __instance)
        {
            return false;
        }
    }
    
    [HarmonyPatch(typeof(LocationTrigger), nameof(LocationTrigger.PlayerEntered))]
    public static class LocationTriggerPlayerEnteredPatch
    {
        static void Postfix()
        {
            var scene = LocationTrigger.LastLocation;
            if (scene.HasValue)
            {
                string locationName = scene.Value.name;
                Log.LogMessage($"Entered Location: {locationName}");
                Plugin.lastNamedLocation = SableGameManager.MainCharacter.transform.position;
            }
        }
    }

}
