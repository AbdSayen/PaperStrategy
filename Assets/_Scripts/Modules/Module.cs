using Photon.Pun;
using UnityEngine;

public class Module : MonoBehaviour
{
    public ModuleInfo Info { get; set; }
    public float Health { get; private set; }
    public string OwnerId { get; set; } = null;

    public Color NormalColor { get; private set; } = new();

    protected virtual void Awake() { }
    protected virtual void Start() => PlayersManager.Instance.OnLocalReady += AutoSetColor;
    private void OnDisable() => PlayersManager.Instance.OnLocalReady -= AutoSetColor;

    protected void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    protected void SetNormalColor(Color color, bool apply = false)
    {
        NormalColor = color;
        if (apply) SetColor(NormalColor);
    }

    public virtual void AutoSetColor()
    {
        SetNormalColor(
            OwnerId != null
            ? PlayersManager.Instance.GetPlayerData(OwnerId).Color
            : Color.black, true);
    }

    public void Initialize(string ownerId)
    {
        OwnerId = ownerId;
        Health = Info.maxHealth;
    }

    public void NormalizeColor()
    {
        GetComponent<SpriteRenderer>().color = NormalColor;
    }

    public void TakeDamage(int damage) => Health = Mathf.Max(0, Health - damage);
    public void Repair(int amount) => Health = Mathf.Min((int)Info.maxHealth, Health + amount);

    protected virtual void OnBuilt() { }
    protected virtual void OnDestroyed() { }
}