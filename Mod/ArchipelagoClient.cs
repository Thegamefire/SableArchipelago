using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.Converters;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using CollectiblesBehaviour;
using MapMagic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace com.thegamefire.sablearchipelago;

public class ArchipelagoClient
{
    private ArchipelagoSession _session;
    private DeathLinkService _deathLinkService;
    
    public readonly Dictionary<string, string> ServerItemMap = UtilityMappings.LoadServerItemNameDict();
    public int LastHandledItemIndex;
    public int ChumTearLocationsChecked;

    public static ApConnectionState ConnectionState = ApConnectionState.Disconnected;

    public ArchipelagoClient()
    {
        _session = ArchipelagoSessionFactory.CreateSession(Plugin.ConfigApHost.Value);
        _session.MessageLog.OnMessageReceived += this.OnMessageReceived;
        _session.Socket.ErrorReceived += this.OnErrorReceived;
        _session.Items.ItemReceived += this.OnItemReceived;
        _deathLinkService = _session.CreateDeathLinkService();
        _deathLinkService.OnDeathLinkReceived += this.OnDeathReceived;

    }

    public string GetPlayerName()
    {
        return _session.Players.GetPlayerName(_session.ConnectionInfo.Slot);
    }

    public void Connect()
    {
        if (_session.Socket.Connected)
        {
            Plugin.Log.LogMessage("Already Connected");
            return;
        }

        ConnectionState = ApConnectionState.Connecting;
        LoginResult result;

        try
        {

            result = _session.TryConnectAndLogin("Sable", Plugin.ConfigApSlot.Value, ItemsHandlingFlags.AllItems, 
                new Version(0, 6, 5), null, null, Plugin.ConfigApPassword.Value);
            
            if (Plugin.ConfigApDeathlink.Value) {
                _deathLinkService.EnableDeathLink();
            }
        }
        catch (Exception e)
        {
            result = new LoginFailure(e.GetBaseException().Message);
        }

        if (!result.Successful)
        {
            ConnectionState = ApConnectionState.Disconnected;
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
        ConnectionState = ApConnectionState.Connected;
    }

    public void Disconnect()
    {
        if (!_session.Socket.Connected)
            return;
        _session.Socket.DisconnectAsync().Wait();
        ConnectionState = ApConnectionState.Disconnected;
    }

    private void OnMessageReceived(LogMessage message)
    {
        if (message is ItemSendLogMessage itemSendLogMessage && itemSendLogMessage.IsSenderTheActivePlayer && !itemSendLogMessage.IsReceiverTheActivePlayer)
        {
            ItemInfo item = itemSendLogMessage.Item;
            ApPopUp popUp = new ApPopUp(
                $"Collected {item.LocationName}",
                    $"This sent {item.ItemDisplayName} to {itemSendLogMessage.Receiver.Name}"
            );
            UiHelper.ToShowPopUpQueue.Enqueue(popUp);
        }

        Plugin.Log.LogMessage("Archipelago: " + message);
    }

    private void OnErrorReceived(Exception e, string message)
    {
        if (!_session.Socket.Connected)
            ConnectionState = ApConnectionState.Disconnected;

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

    private void OnDeathReceived(DeathLink deathLink)
    {
        if (deathLink.Source != GetPlayerName())
        {
            Plugin.DeathReceived = true;
        }
    }

    public void SendDeath()
    {
        if (ConnectionState != ApConnectionState.Connected)
        {
            Plugin.Log.LogWarning("Tried To Send Death When Not Connected");
            return;
        }

        Plugin.Log.LogWarning("Stamina Ran Out");
        // DeathLink death = new DeathLink(GetPlayerName(), "Stamina Ran Out");
        // _deathLinkService.SendDeathLink(death); // This doesn't work for some reason
        var bouncePacket = new BouncePacket
        {
            Tags = new List<string> { "DeathLink" },
            Data = new Dictionary<string, JToken> {
                {"time", DateTime.UtcNow.ToUnixTimeStamp()},
                {"source", GetPlayerName()},
                {"cause", $"{GetPlayerName()}'s Stamina Ran Out"}
            }
        };

        _session.Socket.SendPacket(bouncePacket);
    }

    public void SendLocation(string locationName)
    {
        if (ConnectionState != ApConnectionState.Connected)
        {
            Plugin.Log.LogWarning($"Tried To Send Location \"{locationName}\" When Not Connected");
            return;
        }
        
        if (locationName == "Chum Queen Tear")
        {
            locationName = $"Chum Tear {this.ChumTearLocationsChecked + 1}";
            ChumTearLocationsChecked += 1;
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

    public void SendHicaricRing(Vector3 playerLoc)
    {
        Vector3 closestRingLoc = new Vector3();
        float closestRingDist = float.MaxValue;
        
        
        foreach (Vector3 ringLoc in UtilityMappings.GetHicaricRingLocations().Keys)
        {
            float dist = ringLoc.DistAxisAligned(playerLoc);
            if (dist < closestRingDist)
            {
                closestRingDist = dist;
                closestRingLoc = ringLoc;
            }
        }
        
        SendLocation(UtilityMappings.GetHicaricRingLocations()[closestRingLoc]);
    }
}

public enum ApConnectionState
{
    Connected, Connecting, Disconnected
}