using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class Bootstrap : MonoBehaviour
{
    public enum StartMode { None, Host, Server, Client }
    [Header("Startup")]
    public StartMode startAs = StartMode.Host;
    public NetworkManager networkManager;

    [Header("Gameplay Databases")]
    public ItemDatabase itemDatabase;

    private void Awake()
    {
        if (networkManager == null) networkManager = FindObjectOfType<NetworkManager>();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        if (networkManager == null)
        {
            Debug.LogError("Bootstrap: NetworkManager is missing.");
            return;
        }

        switch (startAs)
        {
            case StartMode.Host:
                networkManager.StartHost();
                break;
            case StartMode.Server:
                networkManager.StartServer();
                break;
            case StartMode.Client:
                networkManager.StartClient();
                break;
        }
    }
}
