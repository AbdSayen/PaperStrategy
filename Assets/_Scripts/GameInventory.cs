using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInventory : MonoBehaviour
{
    
    public static GameInventory Instance;

    [SerializeField] private Text goldText;
    [SerializeField] private Text woodText;
    [SerializeField] private Text oilText;

    public static Materials _balance;
    public static Materials Balance 
    { 
        get 
        { 
            if (_balance == null) _balance = new(); 
            return _balance; 
        }
        set 
        {
            _balance = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GetStandartMaterials();
    }

    private void GetStandartMaterials()
    {
        Balance.SetCount(Materials.GOLD, 30000);
        Balance.SetCount(Materials.WOOD, 10000);
        Balance.SetCount(Materials.OIL, 25);
        UpdateUI();
    }

    public void UpdateUI()
    {
        goldText.text = Balance.GetCount(Materials.GOLD).ToString();
        woodText.text = Balance.GetCount(Materials.WOOD).ToString();
        oilText.text = Balance.GetCount(Materials.OIL).ToString();
    }
}

public class Materials 
{
    public static string MONEY { get; } = "MONEY";
    public static string GOLD { get; } = "GOLD";
    public static string WOOD { get; } = "WOOD";
    public static string IRON { get; } = "IRON";
    public static string TITANIUM { get; } = "TITANIUM";
    public static string URANUS { get; } = "URANUS";
    public static string OIL { get; } = "OIL";

    public Dictionary<string, int> MaterialsCount { get; private set; }
        = new Dictionary<string, int>();

    public int GetCount(string name) => 
        MaterialsCount.TryGetValue(name, out int count) ? count : 0;

    public void SetCount(string name, int amount) =>
        MaterialsCount[name] = amount;

    public void IncreaseCount(string name, int amount) =>
        MaterialsCount[name] = GetCount(name) + amount;

    public void DecreaseCount(string name, int amount) =>
        MaterialsCount[name] = GetCount(name) - amount;

    public bool HasEnough(string name, int requiredAmount) =>
        GetCount(name) >= requiredAmount;

    public int Count { get => MaterialsCount.Count; }
}