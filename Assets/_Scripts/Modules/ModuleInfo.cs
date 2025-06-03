using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Module Info")]
public class ModuleInfo : ScriptableObject
{
    [Header("Base")]
    public string moduleName;
    [TextArea] public string description;

    [Header("Characteristics")]
    public float maxHealth;
    public Vector2Int size;

    [Header("Price")]
    public int moneyCost;
    public int goldCost;
    public int woodCost;
    public int oilCost;
    public int uranusCost;
    public int titaniumCost;

    [Header("Other")]
    public Sprite sprite;
    public GameObject prefab;

    public Dictionary<string, int> PriceList
    {
        get => new Dictionary<string, int>()
        {
            { Materials.MONEY, moneyCost },
            { Materials.GOLD, goldCost },
            { Materials.WOOD, woodCost },
            { Materials.OIL, oilCost },
            { Materials.TITANIUM, titaniumCost },
            { Materials.URANUS, uranusCost },
        };
    }
}
