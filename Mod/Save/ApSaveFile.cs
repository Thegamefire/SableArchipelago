using com.thegamefire.sablearchipelago.Save;
using HarmonyLib;

namespace com.thegamefire.sablearchipelago;

[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SavePlayerState))]
public class SavePatch
{
    public static void Postfix(SaveManager __instance)
    {
        int slot = __instance.LastLoadedSaveSlot;
        var saveFiles = __instance.SaveFiles;
        if (saveFiles == null || slot < 0 || slot >= saveFiles.Count || saveFiles[slot] == null)
        {
            Plugin.Log.LogError("Unable to save");
            return;
        }

        var saveId = saveFiles[slot].ID;
        
        SaveUtility.Save("AP_LastHandledItemIndex", Plugin.Client.LastHandledItemIndex, saveId, false);
        SaveUtility.Save("AP_ChumTearLocationsChecked", Plugin.Client.ChumTearLocationsChecked, saveId, false);
        ApSaveChecker.AddApSave(saveId);
    }
}

[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.LoadPlayerState))]
public class LoadPatch
{
    public static void Postfix(SaveManager __instance)
    {
        int slot = __instance.LastLoadedSaveSlot;
        var saveFiles = __instance.SaveFiles;
        if (saveFiles == null || slot < 0 || slot >= saveFiles.Count || saveFiles[slot] == null)
        {
            Plugin.Log.LogError("Unable to load save");
            return;
        }

        var saveId = saveFiles[slot].ID;
        
        Plugin.Client.LastHandledItemIndex = SaveUtility.Load<int>("AP_LastHandledItemIndex", saveId);
        Plugin.Client.ChumTearLocationsChecked = SaveUtility.Load<int>("AP_ChumTearLocationsChecked", saveId);
    }
}