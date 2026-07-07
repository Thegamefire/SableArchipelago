using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.Runtime;

namespace com.thegamefire.sablearchipelago.Save;

public static class ApSaveChecker
{
    private static HashSet<string> apSaveIds;

    public static bool IsApSave(string saveID)
    {
        if (apSaveIds == null)
        {
            Plugin.Log.LogError($"Checked if save {saveID} is an AP Save before Saves were loaded");
            return false;
        }
        Plugin.Log.LogInfo($"Checking if save {saveID} is an apsave with apsaves: {apSaveIds.Join()}");
        return apSaveIds.Contains(saveID);
    }

    public static void AddApSave(string saveId)
    {
        Plugin.Log.LogInfo($"Adding {saveId} as an apsave");
        apSaveIds.Add(saveId);
    }

    public static void Save()
    {
        apSaveIds ??= new HashSet<string>();

        var arr = new Il2CppStringArray(apSaveIds.Count);
        
        int i = 0;
        foreach (var id in apSaveIds)
        {
            arr[i++] = id;
        }

        Plugin.Log.LogInfo($"Saving Saves: {apSaveIds.Join()}");
        Plugin.Log.LogInfo($"Il2Cpp Saves: {arr.Join()}");
        
        SaveUtility.Save("APSaves", arr, "ApSaveManager");
    }

    public static void Load()
    {
        var arr = SaveUtility.Load<Il2CppStringArray>("APSaves", "ApSaveManager");
        Plugin.Log.LogInfo($"Is Il2Cpp Saves Null? {arr == null}");
        apSaveIds = new HashSet<string>();
        if (arr == null)
            return;
        
        Plugin.Log.LogInfo($"Loaded Il2Cpp Saves: {arr.Join()}");
        foreach (var saveId in arr)
        {
            AddApSave(saveId);
        }
    }
}


[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.SaveThisObjectToDisk))]
public static class SaveManagerSavePatch
{
    public static void Postfix()
    {
        Plugin.Log.LogInfo("Saving saves");
        ApSaveChecker.Save();
    }
}

[HarmonyPatch(typeof(SaveManager), nameof(SaveManager.Initialise))]
public static class SaveManagerLoadPatch
{
    public static void Postfix()
    {
        Plugin.Log.LogInfo("Loading saves");
        ApSaveChecker.Load();
    }
}