using System.Diagnostics;
using com.thegamefire.sablearchipelago.UI;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

public static class DebugHelper
{
    
    [Conditional("DEBUG")]
    public static void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            ChatBox.AddMessage("This is a long message that will not fit normally. Lorem ipsum dolor sit amet some other text that should fill the box completely...");
        if (Input.GetKeyDown(KeyCode.F8))
            ChatBox.AddMessage("Debug Message");
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