using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{
    private string _profileName;
    private string _sessionName;
    private string pID;
    private FixedString128Bytes privateID;
    private Guid _id;
    private int _maxPlayers = 4;
    private ConnectionState _state = ConnectionState.Disconnected;
    private ISession _session;
    private NetworkManager m_NetworkManager;
    private UnityTransport unityTransport;
    public GameObject connectionUI;
    public TMP_InputField userName, sessionID, ipAddress;
    public NetworkedPlayerSpawn playerSpawn;
    private ulong clientID;
    private MenuToGame mTG;

    private enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
    }

    private async void Awake()
    {
        m_NetworkManager = GetComponent<NetworkManager>();
        mTG = GameObject.Find("MTG").GetComponent<MenuToGame>();
        if (m_NetworkManager.ConnectedClientsList.Count >= 4)
        {
            SceneManager.LoadScene(0);
            Destroy(this.gameObject);
        }
        else
        {
            m_NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            m_NetworkManager.OnSessionOwnerPromoted += OnSessionOwnerPromoted;
            await UnityServices.InitializeAsync();
        }
    }

    private void OnSessionOwnerPromoted(ulong sessionOwnerPromoted)
    {
        if (m_NetworkManager.LocalClient.IsSessionOwner)
        {
            Debug.Log($"Client-{m_NetworkManager.LocalClientId} is the session owner!");
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (m_NetworkManager.LocalClientId == clientId)
        {
            clientID = clientId;
            Debug.Log($"Client-{pID} is connected and can spawn {nameof(NetworkObject)}s.");
            if (_profileName == "2")
            {
                pID = "a6d07ecc-a184-4797-9c8a-30589a0b33d7";
            }
            if (_profileName == "3")
            {
                pID = "23d2f901-c873-4dca-9e99-d936180bebc7";
            }
            if (_profileName == "4")
            {
                pID = "ff77b5ee-f53e-42f3-84fb-ffd6088a6983";
            }
            privateID = pID;
            playerSpawn.Spawn(clientId, privateID, m_NetworkManager);
        }
    }

    private void Start()
    {
        if (_state == ConnectionState.Connected)
            return;

        //connectionUI.SetActive(_state != ConnectionState.Connecting);

        try { pID = mTG.pID; }
        catch {
            _id = System.Guid.NewGuid();
            pID = _id.ToString();
            PlayerPrefs.SetString("pID", pID);
        }

        if(pID == "")
        {
            _id = System.Guid.NewGuid();
            pID = _id.ToString();
            PlayerPrefs.SetString("pID", pID);
        }

        StartGame();

    }

    void StartGame()
    {
        _profileName = mTG.username;
        _sessionName = mTG.sessionID;
        unityTransport = GetComponent< UnityTransport>();
        UnityTransport.ConnectionAddressData connectionAddressData = unityTransport.ConnectionData;
        connectionAddressData.Address = mTG.ipAddress;
        //connectionUI.SetActive(false);
        CreateOrJoinSessionAsync();
    }

    private void OnDestroy()
    {
        _session?.LeaveAsync();
    }

    private async Task CreateOrJoinSessionAsync()
    {
        _state = ConnectionState.Connecting;

        try
        {
            AuthenticationService.Instance.SwitchProfile(_profileName);
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            var options = new SessionOptions()
            {
                Name = _sessionName,
                MaxPlayers = _maxPlayers
            }.WithDistributedAuthorityNetwork();

            _session = await MultiplayerService.Instance.CreateOrJoinSessionAsync(_sessionName, options);

            _state = ConnectionState.Connected;
        }
        catch (Exception e)
        {
            _state = ConnectionState.Disconnected;
            Debug.LogException(e);
        }
    }

    public void SignOutPlayer()
    {
        string identity = AuthenticationService.Instance.PlayerId;
        _session?.LeaveAsync();
        AuthenticationService.Instance.SignOut(true);
        NetworkManager.Singleton.Shutdown();
        Destroy(this.gameObject);
    }

    public FixedString128Bytes PID
    {
        get { return privateID; }
    }

    public ulong ClientID
    {
        get { return clientID; }
    }

    public string PlayerName
    {
        get { return _profileName; }
    }
}