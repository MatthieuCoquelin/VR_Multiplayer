using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

public class NetworkConnect : MonoBehaviour
{
    [SerializeField] private int m_maxConnections = 25;
    [SerializeField] private UnityTransport m_transport = null;

    private Lobby m_currentLobby;
    private float m_heartBeatTimer;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Connect()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(m_maxConnections);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        m_transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, 
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        CreateLobbyOptions lobbyOption = new CreateLobbyOptions();
        lobbyOption.IsPrivate = false;
        lobbyOption.Data = new Dictionary<string, DataObject>();
        DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
        lobbyOption.Data.Add("JOIN_CODE", dataObject);

        m_currentLobby = await Lobbies.Instance.CreateLobbyAsync("Lobby Name", m_maxConnections, lobbyOption);

        NetworkManager.Singleton.StartHost();

        Debug.Log("Lobby Created");
    }

    public async void Join()
    {
        m_currentLobby = await Lobbies.Instance.QuickJoinLobbyAsync();
        string relayJoincode = m_currentLobby.Data["JOIN_CODE"].Value;

        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoincode);

        m_transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
        
        NetworkManager.Singleton.StartClient();

        Debug.Log("Lobby Joined");
    }

    private void Update()
    {
        if(m_heartBeatTimer > 15)
        {
            m_heartBeatTimer -= 15;

            if (m_currentLobby != null && m_currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                LobbyService.Instance.SendHeartbeatPingAsync(m_currentLobby.Id);
        }
        m_heartBeatTimer += Time.deltaTime;
    }
}
