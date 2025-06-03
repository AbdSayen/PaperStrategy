using UnityEngine;
using UnityEngine.UI;

public class MaterialInfoPanel : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text countText;
    public void SetInfo(Sprite image, string text)
    {
        this.image.sprite = image;
        countText.text = text;
    }
}
