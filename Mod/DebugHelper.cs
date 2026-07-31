using System.Diagnostics;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

public static class DebugHelper
{
    
    [Conditional("DEBUG")]
    public static void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
            LogSablePosition();
        if (Input.GetKeyDown(KeyCode.F10))
            AreaBlocker.CreateDebugWall(SableGameManager.MainCharacter.transform.position);
        if (Input.GetKeyDown(KeyCode.F11))
            AreaBlocker.SolidifyDebugWall();
    }

    [Conditional("DEBUG")]
    public static void LogSablePosition()
    {
        var pos = SableGameManager.MainCharacter.transform.position;
        Plugin.Log.LogInfo($"Sable is at: {pos.x}, {pos.y}, {pos.z}");
    }

    [Conditional("DEBUG")]
    public static void OnTitleScreenLoad()
    {
    }
}