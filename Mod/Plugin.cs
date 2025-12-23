using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Sable.exe")]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    //////////////// Config //////////////////
    internal static ConfigEntry<string> configApHost;
    internal static ConfigEntry<string> configApSlot;
    internal static ConfigEntry<string> configApPassword;

    internal static ArchipelagoClient client;

    internal static bool sableWasExhausted = false;
    internal static ItemDatabase itemDatabase = null;
    
    public override void Load()
    {
        // Plugin startup logic
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
     
        Log.LogInfo("Loading config...");
        LoadConfig();
        Log.LogInfo("Config loaded!");
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), null);

        var itemDatabase = Resources
            .FindObjectsOfTypeAll<ItemDatabase>()
            .FirstOrDefault();
        Log.LogMessage("Database found: "+ (itemDatabase != null));
        
        Log.LogInfo("Connecting to Archipelago Server...");
        client = new ArchipelagoClient();
        client.Connect();
    }

    private void LoadConfig()
    {
        configApHost = Config.Bind("Archipelago Connection",
            "AP_Host",
            "localhost:38281",
            "The ip (and port) of the archipelago server to connect to, usually in this is something in the form of archipelago.gg:<port>");
        configApSlot = Config.Bind("Archipelago Connection",
            "AP_Slot",
            "Player1",
            "The name of the slot (player) you want to connect as.");
        configApPassword = Config.Bind("Archipelago Connection",
            "AP_Password",
            "",
            "The password of the archipelago server, if there is no password leave this empty.");
    }

    private static void LogAllItemsInGame()
    {
        if (itemDatabase != null)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var item in itemDatabase.Items)
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

    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.Add), new [] { typeof(Items.Item), typeof(int) })]
    static class InventoryAddPatch
    {
        // static MethodBase TargetMethod()
        // {
        //     var playerInvType = AccessTools.TypeByName("PlayerInventory");
        //     var itemType = AccessTools.TypeByName("Items.Item");
        //     return AccessTools.Method(playerInvType, "Add", new[] { itemType, typeof(int) });
        // }

        static void Prefix(PlayerInventory __instance, Items.Item item, Int32 quantity)
        {
            Log.LogDebug($"Added Item: {item.ItemDef.Name}, {quantity}");
        }
    }

    [HarmonyPatch(typeof(SableCharacterController), nameof(SableCharacterController.Update))]
    static class OnFrame
    {
        static void Prefix(SableCharacterController __instance)
        {
            if (itemDatabase == null)
            {
                itemDatabase = Resources
                    .FindObjectsOfTypeAll<ItemDatabase>()
                    .FirstOrDefault();
            }

            if (sableWasExhausted != __instance.Exhausted)
            {
                if (__instance.Exhausted)
                {
                    client.SendDeath();
                }

                sableWasExhausted = __instance.Exhausted;
            }
        }
    }

}
