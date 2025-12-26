using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using BehaviorDesigner.Runtime.Tasks;
using CollectiblesBehaviour;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

public class ArchipelagoClient
{
    private ArchipelagoSession _session;
    public Dictionary<string, string> ServerItemMap = UtilityMappings.LoadServerItemNameDict();
    public int LastHandledItemIndex;
    public int HicaricRingLocationsChecked;
    public int ChumTearLocationsChecked;

    public ArchipelagoClient()
    {
        _session = ArchipelagoSessionFactory.CreateSession(Plugin.ConfigApHost.Value);
        _session.MessageLog.OnMessageReceived += this.OnMessageReceived;
        _session.Socket.ErrorReceived += this.OnErrorReceived;
        _session.Items.ItemReceived += this.OnItemReceived;

    }

    public void Connect()
    {
        LoginResult result;

        try
        {
            result = _session.TryConnectAndLogin("Sable", Plugin.ConfigApSlot.Value, ItemsHandlingFlags.AllItems, 
                new Version(0, 6, 5), null, null, Plugin.ConfigApPassword.Value);
        }
        catch (Exception e)
        {
            result = new LoginFailure(e.GetBaseException().Message);
        }

        if (!result.Successful)
        {
            LoginFailure failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to {Plugin.ConfigApHost.Value} as {Plugin.ConfigApSlot.Value}:";
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
        Plugin.Log.LogMessage("Archipelago: " + message);
    }

    private void OnErrorReceived(Exception e, string message)
    {
        Plugin.Log.LogError($"Archipelago Error: {message}");
    }

    private void OnItemReceived(ReceivedItemsHelper itemHelper)
    {
        ItemInfo item = itemHelper.DequeueItem();
        if (itemHelper.Index <= LastHandledItemIndex)
        {
            return;
        }

        LastHandledItemIndex++;
        if (!ServerItemMap.ContainsKey(item.ItemName))
        {
            Plugin.Log.LogError("Received Unknown Item: "+item);
            return;
        }

        string ingameName = ServerItemMap[item.ItemName];
        Plugin.ReceiveItem(ingameName);
    }

    public void SendDeath()
    {  
        Plugin.Log.LogWarning("Stamina Ran Out");
    }

    public void SendLocation(string locationName)
    {
        if (locationName == "Hicaric Ring Artefact")
        {
            locationName = $"Hicaric Ring {this.HicaricRingLocationsChecked + 1}";
            HicaricRingLocationsChecked += 1;
        } else if (locationName == "Chum Queen Tear")
        {
            locationName = $"Chum Tear {this.ChumTearLocationsChecked + 1}";
            HicaricRingLocationsChecked += 1;
        }

        Plugin.Log.LogMessage($"Sending Location: {locationName}");
        _session.Locations.CompleteLocationChecks(_session.Locations.GetLocationIdFromName("Sable", locationName));
    }

    public void SendChum(ChumBehaviour chum)
    {
        Vector3 pos = chum.transform.position;
        string key = $"{Math.Round(pos.x)};{Math.Round(pos.y)};{Math.Round(pos.z)}";
        SendLocation(Plugin.ChumNameMap[key]);
    }
}

