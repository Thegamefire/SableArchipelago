using Items;
using UnityEngine;
using UnityEngine.Playables;

namespace com.thegamefire.sablearchipelago;

public class AreaBlockerBehaviour : MonoBehaviour
{
    private Area? _area1;
    private Area? _area2;

    public void SetAreas(Area area1, Area area2)
    {
        _area1 = area1;
        _area2 = area2;
    }

    public void RecheckAreas()
    {
        if (!ShouldExist(_area1, _area2))
        {
            gameObject.Destroy();
        }
    }

    public static bool ShouldExist(Area? area1, Area? area2)
    {
        if (area1 == null || area2 == null)
        {
            Plugin.Log.LogWarning("Checked if area blocker with null areas should exist");
            return true;
        }

        var inventoryUtility = Utility.GetPlayerInventoryUtility();
        Item area1Map = SingletonAsset.Instance<ItemDatabase>().GetItemFromName(area1.Value.GetMapName());
        Item area2Map = SingletonAsset.Instance<ItemDatabase>().GetItemFromName(area2.Value.GetMapName());
        bool hasArea1 = inventoryUtility.GetQuantityHeld(area1Map) > 0;
        bool hasArea2 = inventoryUtility.GetQuantityHeld(area2Map) > 0;
        return  !(hasArea1 && hasArea2);
    }
}