using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using BehaviorDesigner.Runtime.Tasks;

namespace com.thegamefire.sablearchipelago;

public class ArchipelagoClient
{
    private ArchipelagoSession _session;

    public ArchipelagoClient()
    {
        _session = ArchipelagoSessionFactory.CreateSession(Plugin.configApHost.Value);
        _session.MessageLog.OnMessageReceived += this.OnMessageReceived;
        _session.Socket.ErrorReceived += this.OnErrorReceived;

    }

    public void Connect()
    {
        LoginResult result;

        try
        {
            // handle TryConnectAndLogin attempt here and save the returned object to `result`
            result = _session.TryConnectAndLogin("", Plugin.configApSlot.Value, ItemsHandlingFlags.AllItems, 
                new Version(0, 6, 5), null, null, Plugin.configApPassword.Value);
        }
        catch (Exception e)
        {
            result = new LoginFailure(e.GetBaseException().Message);
        }

        if (!result.Successful)
        {
            LoginFailure failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to {Plugin.configApHost.Value} as {Plugin.configApSlot.Value}:";
            foreach (string error in failure.Errors)
            {
                errorMessage += $"\n    {error}";
            }
            foreach (ConnectionRefusedError error in failure.ErrorCodes)
            {
                errorMessage += $"\n    {error}";
            }
            
            Plugin.Log.LogError(errorMessage);
            return;
        }

        // Successfully connected, `ArchipelagoSession` (assume statically defined as `session` from now on) can now be
        // used to interact with the server and the returned `LoginSuccessful` contains some useful information about the
        // initial connection (e.g. a copy of the slot data as `loginSuccess.SlotData`)
        var loginSuccess = (LoginSuccessful)result;
    }

    private void OnMessageReceived(LogMessage message)
    {
        Plugin.Log.LogInfo(message);
    }

    private void OnErrorReceived(Exception e, string message)
    {
        Plugin.Log.LogError($"Archipelago Error: {message}");
    }

    public void SendDeath()
    {  
        Plugin.Log.LogWarning("Stamina Ran Out");
    }
}

