using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelector : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject modeSelector;
    [SerializeField] private List<Button> buttons;

    [SerializeField] private GameObject menu;

    private void Awake()
    {
        modeSelector.SetActive(false);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: new RoomOptions() { MaxPlayers = 3 });
        DisableButtons();
    }

    public void DisableButtons()
    {
        foreach (Button button in buttons) 
        { 
            button.interactable = false;
        }
    }

    public void EnableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    public void OpenModeSelector()
    {
        modeSelector.SetActive(true);
        menu.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        EnableButtons();
    }
}
