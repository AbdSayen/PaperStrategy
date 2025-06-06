using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayersManager : MonoBehaviourPun
{
    public static PlayersManager Instance;
    public List<PlayerData> Players { get; private set; } = new List<PlayerData>();
    
    public event Action<PlayerData> OnPlayerRegistered;
    public event Action OnLocalReady;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(c());
        IEnumerator c()
        {
            while (!areAllPlayersProcessed(false))
            {
                yield return new WaitForSeconds(0.1f);
            }
            LocalRegisterPlayer();
            while (!areAllPlayersProcessed(true))
            {
                yield return new WaitForSeconds(0.1f);
            }
            OnLocalReady?.Invoke();
        }
    }

    private void Update()
    {
        //string output = string.Empty;
        //for (int i = 0; i < Players.Count; i++)
        //{
        //    output += $"{Players[i].Color}\n";
        //}
        //DebugText.UpdateText(output);
    }

    public void LocalRegisterPlayer()
    {
        if (Players.Any(p => p.GlobalId == PhotonNetwork.LocalPlayer.UserId)) return;

        Color newColor = new Color(
            UnityEngine.Random.Range(0.25f, 0.7f),
            UnityEngine.Random.Range(0.25f, 0.7f),
            UnityEngine.Random.Range(0.25f, 0.7f)
        );

        var newPlayer = new PlayerData
        {
            GlobalId = PhotonNetwork.LocalPlayer.UserId,
            LocalId = Players.Count,
            Name = PhotonNetwork.LocalPlayer.NickName,
            Color = newColor
        };

        Camera.main.transform.Rotate(new(0, 0, 
            Players.Count switch {
            0 => 0,
            1 => -90,
            2 => 180,
            3 => 90,
            _ => 0
        }));

        photonView.RPC("GlobalRegisterPlayer", RpcTarget.AllBuffered,
            newPlayer.GlobalId,
            newPlayer.LocalId,
            newPlayer.Name,
            new Vector3(newPlayer.Color.r, newPlayer.Color.g, newPlayer.Color.b));
    }

    private bool areAllPlayersProcessed(bool includeLocal = false) 
    {
        if (includeLocal) return PhotonNetwork.CurrentRoom.Players.Count == Players.Count;
        else return PhotonNetwork.CurrentRoom.Players.Count - 1 == Players.Count;
    }

    [PunRPC] 
    private void GlobalRegisterPlayer(string globalId, int localId, string name, Vector3 color)
    {
        if (Players.Any(p => p.GlobalId == globalId)) return;

        var newPlayer = new PlayerData
        {
            GlobalId = globalId,
            LocalId = localId,
            Name = name,
            Color = new(color.x, color.y, color.z) 
        };

        Players.Add(newPlayer);
        OnPlayerRegistered?.Invoke(newPlayer);
    }

    public PlayerData GetPlayerData(string id) => Players.FirstOrDefault(p => p.GlobalId == id);
}

[Serializable]
public class PlayerData
{
    public string GlobalId;
    public int LocalId;
    public Color Color;
    public string Name;
}