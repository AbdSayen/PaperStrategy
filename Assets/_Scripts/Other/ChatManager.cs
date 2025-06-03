using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public static ChatManager Instance;

    [SerializeField] private InputField chatInput;
    [SerializeField] private Text chatContent;

    private int maxMessages = 5;
    private List<string> messages = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void SendChatMessage()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            string message = (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName) ? "Player" : PhotonNetwork.LocalPlayer.NickName) + 
                $" : {chatInput.text}";            
            photonView.RPC("ReceiveMessage", RpcTarget.All, message);
            chatInput.text = "";
        }
    }

    public override void OnJoinedRoom()
    {
        print("νσ χε νΰυσι");
        photonView.RPC("ReceiveMessage", RpcTarget.All, $"Player {PhotonNetwork.LocalPlayer.NickName} connected.");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ReceiveMessage($"Player {otherPlayer.NickName} left room.");
    }

    [PunRPC]
    private void ReceiveMessage(string message)
    {
        messages.Add(message + "\n");

        string output = string.Empty;
        while (messages.Count >= maxMessages)
            messages.RemoveAt(0);
        for (int i = 0; i < messages.Count; i++)
            output += messages[i];
        chatContent.text = output;
    }
}