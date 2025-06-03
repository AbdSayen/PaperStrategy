using UnityEngine;

public class MaterialsSpritesManager : MonoBehaviour
{
    [SerializeField] private MaterialsSprites materialsSprites;
    public static MaterialsSprites Instance;

    private void Awake() =>
        Instance = materialsSprites;
}
