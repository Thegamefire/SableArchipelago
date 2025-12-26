using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BehaviorDesigner.Runtime.Tasks.Unity.Timeline;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using CollectiblesBehaviour;
using HarmonyLib;
using Items;
using UI.HUD.Notifications;
using UI.HUD.Notifications.NotificationTypes;
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

    internal static Dictionary<string, string> ChumNameMap = UtilityMappings.LoadChumDictionary();
    
    internal static ArchipelagoClient Client;

    internal static bool SableWasExhausted = false;
    internal static ItemDatabase ItemDB = null;
    internal static PlayerInventory SableInventory = null;

    internal static bool ReceivingItem = false;
    internal static Queue<string> ReceivedItemsQueue = new Queue<string>();
    
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
    

    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.Add), new [] { typeof(Items.Item), typeof(int) })]
    static class InventoryAddPatch
    {
        static bool Prefix(PlayerInventory __instance, Items.Item item, Int32 quantity)
        {
            Log.LogMessage($"Added Item: {item.ItemDef.Name}, {quantity} | Map: {item.ItemDef.Name.Contains("Map")}");
            if (Plugin.ReceivingItem)
            {
                Plugin.ReceivingItem = false;
                return true;
            }
            if (Plugin.SableInventory == null) {
                Plugin.SableInventory = __instance;
            }
            

            return true;
        }
    }

    [HarmonyPatch(typeof(ChumBehaviour), nameof(ChumBehaviour.Use))]
    static class OnChumUse
    {
        static bool Prefix(ChumBehaviour __instance)
        {
            Client.SendChum(__instance);
            
            return false;
        }
    }
    

    [HarmonyPatch(typeof(SableCharacterController), nameof(SableCharacterController.Update))]
    static class OnFrame
    {
        static void Prefix(SableCharacterController __instance)
        {
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
                    inventory.Add(ItemDB.GetItemFromName(itemName), 1);
                }
            }

            if (SableWasExhausted != __instance.Exhausted)
            {
                if (__instance.Exhausted)
                {
                    Client.SendDeath();
                    var inventories = Resources.FindObjectsOfTypeAll<PlayerInventory>();
                    Log.LogMessage($"Inventories found: {inventories.Length}");
                    foreach (PlayerInventory inventory in inventories) // the 2nd seems to be the correct one for some reason
                    {
                            Log.LogWarning($"Money: {inventory.moneyHeld}");
                    }
                    

                }

                SableWasExhausted = __instance.Exhausted;
            }
        }
    }

}
