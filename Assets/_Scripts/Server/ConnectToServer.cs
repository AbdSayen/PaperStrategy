using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public static ConnectToServer Instance { get; private set; }
    public bool IsConnectedToServer { get; set; } = false;
    
    [SerializeField] private Text connectionStatusText;
    [SerializeField] private GameObject menu;

    private void Awake()
    {
        Instance = this;
        connectionStatusText.text = Localize.Get("Connecting to Server...", "Подключение к Серверу...");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("ru");

        connectionStatusText.gameObject.SetActive(true);
        menu.SetActive(false);
    }

    private void OnFailedToConnectToMasterServer()
    {
        Awake();
    }

    private void OnFailedToConnect()
    {
        Awake();
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = Random.Range(0, 10000).ToString();
    }

    public override void OnConnectedToMaster()
    {
        connectionStatusText.text = Localize.Get("Connecting to Lobby...", "Подключение к Лобби...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        StartCoroutine(e());
        IEnumerator e()
        {
            IsConnectedToServer = true;
            yield return new WaitForSeconds(0.5f);
            connectionStatusText.text = Localize.Get("Connection Successful.", "Успешное подключение.");
            menu.SetActive(true);
            yield return new WaitForSeconds(4f);
            connectionStatusText.text = string.Empty;
            connectionStatusText.gameObject.SetActive(false);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectionStatusText.text = Localize.Get("Connection Error...", "Ошибка подключения...");
    }
}