using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Module : MonoBehaviourPun, IPunObservable
{
    public ModuleInfo Info { get; set; }
    public float Health { get; private set; }
    public string OwnerId { get; set; } = null;
    public Color NormalColor { get; private set; } = new();
    public SpriteRenderer SpriteRenderer { get; private set; }

    private bool _isInitialized;
    private Coroutine _initCoroutine;

    protected virtual void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        // Принудительная проверка PhotonView
        //if (photonView == null)
        //    photonView = gameObject.AddComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        if (photonView.IsMine && OwnerId != null)
        {
            AutoSetColor();
        }
    }

    private void OnEnable()
    {
        _initCoroutine = StartCoroutine(SubscribeWhenReady());
    }

    private void OnDisable()
    {
        if (_initCoroutine != null)
            StopCoroutine(_initCoroutine);

        if (PlayersManager.Instance != null)
            PlayersManager.Instance.OnLocalReady -= AutoSetColor;
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (PlayersManager.Instance == null)
            yield return null;

        PlayersManager.Instance.OnLocalReady += AutoSetColor;
        _isInitialized = true;

        // Если OwnerId уже установлен, но цвет не обновлен
        if (OwnerId != null)
        {
            AutoSetColor();
            photonView.RPC("SyncModuleRPC", RpcTarget.Others, OwnerId, NormalColor.r, NormalColor.g, NormalColor.b);
        }
    }

    [PunRPC]
    private void SyncModuleRPC(string ownerId, float r, float g, float b)
    {
        OwnerId = ownerId;
        NormalColor = new Color(r, g, b);
        SpriteRenderer.color = NormalColor;
    }

    protected void SetColor(Color color)
    {
        SpriteRenderer.color = color;
    }

    protected void SetNormalColor(Color color, bool apply = false)
    {
        NormalColor = color;
        if (apply) NormalizeColor();
    }

    public virtual void AutoSetColor()
    {
        Color targetColor = OwnerId != null
            ? PlayersManager.Instance.GetPlayerData(OwnerId).Color
            : Color.black;

        SetNormalColor(targetColor, true);

        if (_isInitialized && photonView.IsMine)
        {
            photonView.RPC("SyncModuleRPC", RpcTarget.Others,
                OwnerId,
                NormalColor.r,
                NormalColor.g,
                NormalColor.b);
        }
    }

    public void Initialize(string ownerId)
    {
        OwnerId = ownerId;
        if (Info != null) Health = Info.maxHealth;

        if (photonView.IsMine)
        {
            photonView.RPC("InitializeRPC", RpcTarget.AllBuffered, ownerId);
        }
    }

    [PunRPC]
    private void InitializeRPC(string ownerId)
    {
        OwnerId = ownerId;
        if (Info != null) Health = Info.maxHealth;
        AutoSetColor();
    }

    public void NormalizeColor()
    {
        SetColor(NormalColor);
    }

    public void TakeDamage(int damage)
    {
        Health = Mathf.Max(0, Health - damage);
        if (Health <= 0) OnDestroyed();
    }

    public void Repair(int amount)
    {
        if (Info != null)
            Health = Mathf.Min(Info.maxHealth, Health + amount);
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Health);
            stream.SendNext(OwnerId ?? "");
            stream.SendNext(NormalColor.r);
            stream.SendNext(NormalColor.g);
            stream.SendNext(NormalColor.b);
        }
        else
        {
            Health = (float)stream.ReceiveNext();
            OwnerId = (string)stream.ReceiveNext();
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();

            NormalColor = new Color(r, g, b);
            SpriteRenderer.color = NormalColor;
        }
    }

    protected virtual void OnBuilt() { }
    protected virtual void OnDestroyed() { }
}