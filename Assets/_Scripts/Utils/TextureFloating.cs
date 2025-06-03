using UnityEngine;
using UnityEngine.UI; 

public class TextureFloating : MonoBehaviour
{
    [SerializeField] private float xSpeed = 0.15f;
    [SerializeField] private float ySpeed = 0.15f;

    private Image image;
    private Material material;
    private Vector2 currentOffset;

    private void Start()
    {
        image = GetComponent<Image>();

        if (image == null)
        {
            Debug.LogError("No Image component found!", this);
            enabled = false;
            return;
        }

        material = new Material(image.material);
        image.material = material;
    }

    private void Update()
    {
        if (material == null) return;

        currentOffset.x += xSpeed * Time.deltaTime;
        currentOffset.y += ySpeed * Time.deltaTime;
        material.mainTextureOffset = currentOffset;
    }

    private void OnDestroy()
    {
        if (material != null && Application.isPlaying)
        {
            Destroy(material);
        }
    }
}