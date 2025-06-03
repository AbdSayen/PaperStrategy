using Photon.Pun;
using UnityEngine;

public class Module : MonoBehaviour
{
    public ModuleInfo Info { get; set; }
    public float Health { get; private set; }
    public int OwnerId { get; set; }

    public Color normalColor { get; private set; } = new();

    private void Awake()
    {
        normalColor = GetComponent<SpriteRenderer>().color;
    }

    private void Start()
    {
        if (GetComponent<PhotonView>().IsMine)
            OwnerId = int.Parse(PhotonNetwork.LocalPlayer.NickName);
    }

    public void Initialize(int ownerId)
    {
        OwnerId = ownerId;
        Health = Info.maxHealth;
    }

    public void NormalizeColor()
    {
        GetComponent<SpriteRenderer>().color = normalColor;
    }

    public void TakeDamage(int damage) => Health = Mathf.Max(0, Health - damage);
    public void Repair(int amount) => Health = Mathf.Min((int)Info.maxHealth, Health + amount);

    protected virtual void OnBuilt() { }
    protected virtual void OnDestroyed() { }
}