using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class Module : MonoBehaviourPun, IPunObservable
{
    public ModuleInfo Info { get; set; }
    public float Health { get; private set; }
    public string OwnerId { get; set; }
    public ModuleStatus Status { get; set; } = ModuleStatus.Inactive;
    public Color NormalColor { get; private set; } = new();
    public SpriteRenderer SpriteRenderer { get; private set; }

    protected virtual void Awake() 
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start() { }
    private void OnEnable() => StartCoroutine(SubscribeWhenReady());

    private IEnumerator SubscribeWhenReady()
    {
        while (PlayersManager.Instance == null)
            yield return null;

        if (photonView.ViewID != 0 && !photonView.IsMine && string.IsNullOrEmpty(OwnerId))
        {
            photonView.RPC("RequestSyncRPC", RpcTarget.MasterClient);
        }

        UpdateColor();
    }

    [PunRPC]
    private void RequestSyncRPC()
    {
        print(2);
        if (photonView.IsMine)
        {
            print(3);
            photonView.RPC("InitializeRPC", RpcTarget.OthersBuffered, OwnerId);
        }
    }

    [PunRPC]
    private void InitializeRPC(string ownerId)
    {
        print(4);
        Initialize(ownerId, Info.moduleName);
    }

    public virtual void SetColor(Color color)
    {
        SpriteRenderer.color = color;
    }

    protected void SetNormalColor(Color color, bool apply = false)
    {
        NormalColor = color;
        if (apply) NormalizeColor();
    }

    public void UpdateColor()
    {
        if (PlayersManager.Instance is null) return;

        SetNormalColor(
            OwnerId is not null && PlayersManager.Instance.GetPlayerData(OwnerId) is not null
            ? PlayersManager.Instance.GetPlayerData(OwnerId).Color
            : Color.black, true);
    }

    public void Initialize(string ownerId, string moduleName)
    {
        string path = "ModulesData/" + moduleName;
        Info = Resources.Load<ModuleInfo>(path);

        Status = ModuleStatus.Active;
        OwnerId = ownerId;
        UpdateColor();

        if (Info == null) return;
        Health = Info.maxHealth;
    }

    public void NormalizeColor()
    {
        SetColor(NormalColor);
    }

    public void TakeDamage(int damage) => Health = Mathf.Max(0, Health - damage);
    public void Repair(int amount) => Health = Mathf.Min((int)Info.maxHealth, Health + amount);

    protected virtual void OnBuilt() { }
    protected virtual void OnDestroyed() { }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Health);
            stream.SendNext(OwnerId);
            stream.SendNext((int)Status);
            stream.SendNext(Info?.moduleName ?? "");
        }
        else
        {
            Health = (float)stream.ReceiveNext();
            OwnerId = (string)stream.ReceiveNext();
            Status = (ModuleStatus)(int)stream.ReceiveNext();

            string moduleName = (string)stream.ReceiveNext();
            if (Info == null && !string.IsNullOrEmpty(moduleName))
                Initialize(OwnerId, moduleName);
        }
    }

}

public enum ModuleStatus 
{ 
    Inactive = 0,
    Building = 1,
    Active = 2,
}