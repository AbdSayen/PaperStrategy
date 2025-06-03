using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialsSprites", menuName = "Materials/Sprites")]
public class MaterialsSprites : ScriptableObject
{
    [SerializeField] private Sprite money;
    [SerializeField] private Sprite gold;
    [SerializeField] private Sprite wood;
    [SerializeField] private Sprite iron;
    [SerializeField] private Sprite oil;
    [SerializeField] private Sprite titanium;
    [SerializeField] private Sprite uranus;

    private Dictionary<string, Sprite> _spritesCache;

    public Dictionary<string, Sprite> Sprites
    {
        get
        {
            if (_spritesCache == null)
            {
                _spritesCache = new Dictionary<string, Sprite>
                {
                    { Materials.MONEY, money },
                    { Materials.GOLD, gold },
                    { Materials.WOOD, wood },
                    { Materials.IRON, iron },
                    { Materials.OIL, oil },
                    { Materials.URANUS, uranus },
                    { Materials.TITANIUM, titanium },

                };
            }
            return _spritesCache;
        }
    }
}