using System;
using System.Collections.Generic;
using HarmonyLib;
using Items;
using Quadtree.Ecology.Chums;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

static class StaminaManager
{
    private static readonly Dictionary<int, float> StaminaLevels = new ()
    {
        { 0, 10 },
        { 1, 15 },
        { 2, 20 },
        { 3, 25 },
        { 4, 30 },
        { 5, 35 },
        { 6, 40 },
    };

    public static void SyncStaminaToInventory()
    {
        var staminaFromTears = CalculateMaxStamina();
        Plugin.Log.LogInfo($"Current Max Stamina: {SableGameManager.MainCharacter.CurrentClimbDistanceValue.Value} | Goal Max Stamina: {staminaFromTears}");
        if (Math.Abs(staminaFromTears - SableGameManager.MainCharacter.CurrentClimbDistanceValue.Value) > 1)
            SableGameManager.MainCharacter.CurrentClimbDistanceValue.Value = CalculateMaxStamina();
    }

    static float CalculateMaxStamina()
    {
        GameObject playerInventoryParent = new GameObject("PlayerInventoryUtilityParent");
        PlayerInventoryUtility inventoryUtility = playerInventoryParent.AddComponent<PlayerInventoryUtility>();
        Item queenChumTears = SingletonAsset.Instance<ItemDatabase>().GetItemFromName("ChumTear");
        int tearCount = inventoryUtility.GetQuantityHeld(queenChumTears);
        tearCount = Math.Clamp(tearCount, 0, 6);
        return StaminaLevels[tearCount];
    }

    [HarmonyPatch(typeof(QueenChumBehaviour), nameof(QueenChumBehaviour.AnimateUpgradeHud))]
    internal class UpgradeAnimationStopper
    {
        public static bool Prefix()
        {
            SyncStaminaToInventory();
            return false;
        }
    }
}


