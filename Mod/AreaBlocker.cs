using System;
using System.Collections.Generic;
using System.Linq;
using GameTemplate.Promises;
using HarmonyLib;
using MapMagic;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

partial class AreaBlocker
{
    private static Shader _mistShader;
    private static GameObject _lastDebugWall;

    
    [HarmonyPatch(typeof(AbstractTerrainContainer), nameof(AbstractTerrainContainer.LoadTerrain))]
    public static class TerrainLoadPatch
    {
        public static void Postfix(AbstractTerrainContainer __instance, ref IPromise __result)
        {
            __result.ThenDo((Action) (() =>
            {
                var terrainName = __instance.Terrain == null ? "null" : __instance.Terrain.name;
                var walls = WallLocations.GetValueOrDefault(terrainName, new HashSet<AreaBlockerLocation>());
                Plugin.Log.LogMessage($"Loaded terrain:  {terrainName} | Applying {walls.Count} walls");
                foreach (AreaBlockerLocation wall in walls)
                {
                    if (AreaBlockerBehaviour.ShouldExist(wall.Area1, wall.Area2))
                        CreateWall(__instance.Terrain, wall);
                }
            }));
        }
    }

    public static Shader GetMistShader()
    {
        if (_mistShader != null)
            return _mistShader;

        _mistShader = Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(b => b.name == "Custom/Mist");
        if (_mistShader == null)
            Plugin.Log.LogError("Could not find MistShader");

        return _mistShader;
    }
    
    public static void CreateDebugWall(Vector3 position)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

        var renderer = wall.GetComponent<MeshRenderer>();
        renderer.material.color = Color.yellow;
        renderer.material.shader = GetMistShader();
        
        
        wall.transform.position = position;
        wall.transform.localScale = new Vector3(3.8f, 5f, 10f);

        // Optional
        wall.name = "APDebugWall";

        _lastDebugWall = wall;
    }

    public static void SolidifyDebugWall()
    {
        if (_lastDebugWall == null)
            return;
        var renderer = _lastDebugWall.GetComponent<MeshRenderer>();
        renderer.material.color = Color.green;

        var length = _lastDebugWall.transform.localScale.z;
        length = (float) Math.Round(length, digits: 1);
        _lastDebugWall.transform.localScale = new Vector3(3.8f, 250, length);

        var pos = _lastDebugWall.transform.position;
        pos.x = (float) Math.Round(pos.x, digits: 1);
        pos.y = 0;
        pos.z = (float) Math.Round(pos.z, digits: 1);

        _lastDebugWall.name = "New AP Wall";

    }

    public static GameObject CreateWall(Terrain parent, AreaBlockerLocation location)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);

        var renderer = wall.GetComponent<MeshRenderer>();
        renderer.material.color = Color.red;
        renderer.material.shader = GetMistShader();
        
        wall.transform.position = new Vector3(location.PosX, 0, location.PosZ);
        wall.transform.localScale = new Vector3(0, location.Rotation, 0);
        wall.transform.rotation = new Vector3(3.8f, 250, location.Length).EulerToQuat();

        wall.name = "AP Wall";
        
        wall.transform.SetParent(parent.transform, true);

        var blocker = wall.AddComponent<AreaBlockerBehaviour>();
        blocker.SetAreas(location.Area1, location.Area2);
        return wall;
    }
}

public enum Area
{
    Ewer,
    Sansee,
    Redsee,
    Hakoa,
    Badlands,
    TheWash,
    SodicWaste
}

public static class AreaMethods
{
    extension(Area area)
    {
        public string GetMapName()
        {
            switch (area)
            {
                case Area.Ewer:
                    return "CatEarCanyonMap";
                case Area.Sansee:
                    return "EasternDuneMap";
                case Area.Redsee:
                    return "WesternDuneMap";
                case Area.Hakoa:
                    return "BlackDesertMap";
                case Area.Badlands:
                    return "BadlandsMap";
                case Area.TheWash:
                    return "WhiteDesertMap";
                case Area.SodicWaste:
                    return "SaltPlainsMap";
            }
            Plugin.Log.LogError("Tried to get Map Name from unknown area");
            return "";
        }
    }
}