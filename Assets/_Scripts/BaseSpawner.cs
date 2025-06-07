using Photon.Pun;
using UnityEngine;

public class BaseSpawner : MonoBehaviour
{
    public static BaseSpawner Instance { get; private set; }

    [SerializeField] private Vector2[] basesSpawnPoints;
    [SerializeField] private GameObject basePrefab;

    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start() 
    {
        Instance = this;
        PlayersManager.Instance.OnPlayerRegistered += SpawnBase;
    }

    private void OnDisable() => PlayersManager.Instance.OnPlayerRegistered -= SpawnBase;

    private void SpawnBase(PlayerData player)
    {
        if (!view.IsMine) return;

        Module _base = PhotonNetwork.Instantiate(basePrefab.name, basesSpawnPoints[player.LocalId], Quaternion.identity)
            .GetComponent<Module>();

        _base.Initialize(player.GlobalId, "Base");
    }
}