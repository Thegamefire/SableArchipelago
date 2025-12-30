using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CollectiblesBehaviour;
using Core.GameManagerStates;
using GameTemplate;
using HarmonyLib;
using Il2CppInterop.Runtime.Runtime;
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

    internal static Dictionary<string, string> ChumNameMap = UtilityMappings.LoadChumDictionary();
    
    internal static ArchipelagoClient Client;

    internal static ItemDatabase ItemDB = null;
    internal static PlayerInventory SableInventory = null;
    internal static SableCharacterController CharacterController = null;

    // Values To Be Listened To Next Frame //
    internal static bool DeathReceived = false;
    internal static bool ReceivingItem = false;
    internal static Queue<string> ReceivedItemsQueue = new Queue<string>();
    internal static bool SableWasExhausted = false;
    
    internal static Vector3 lastNamedLocation = new Vector3();
    internal static bool DeathModeTravel = true;
    
    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
     
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
    }

    private static void LogAllItemsInGame()
    {
        if (ItemDB != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var item in ItemDB.Items)
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
            Log.LogMessage($"Item received: from archipelago? {Plugin.IsReceivingItem()}");
            if (Plugin.IsReceivingItem())
            {
                Plugin.ReceivingItem = false;
                Log.LogMessage($"Received Item: {item.itemDef.Name}");
                return true;
            }
            if (Plugin.SableInventory == null) {
                Plugin.SableInventory = __instance;
            }

            if (item.ItemDef.Name == "Chum")
            {
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
            if (CharacterController == null)
            {
                CharacterController = __instance;
            }

            if (ItemDB == null)
            {
                ItemDB = Resources
                    .FindObjectsOfTypeAll<ItemDatabase>()
                    .FirstOrDefault();
            }
            else if (ReceivedItemsQueue.Count > 0)
            {
                string itemName = ReceivedItemsQueue.Dequeue();
                IList<PlayerInventory> inventories = Plugin.SableInventory!=null ? new List<PlayerInventory>{Plugin.SableInventory}: Resources.FindObjectsOfTypeAll<PlayerInventory>();
                foreach (PlayerInventory inventory in
                         inventories) // the 2nd seems to be the correct one for some reason
                {
                    Plugin.ReceivingItem = true;
                    inventory.Add(ItemDB.GetItemFromName(itemName), 1);
                }
            }

            if (DeathReceived)
            {
                Log.LogMessage("Deathlink Received");
                if (DeathModeTravel && Plugin.lastNamedLocation != new Vector3())
                {
                    DebugCommands.FastTravelToCoords(Plugin.lastNamedLocation);
                } else {
                    // This doesn't yet stop a climb, or put the stamina to 0
                    SableWasExhausted = true;
                    __instance.CanClimb = false;
                    __instance.Exhausted = true;
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
            if (scene.HasValue && Plugin.CharacterController != null)
            {
                string locationName = scene.Value.name;
                Log.LogMessage($"Entered Location: {locationName}");
                Plugin.lastNamedLocation = Plugin.CharacterController.transform.position;
            }
        }
    }

}
