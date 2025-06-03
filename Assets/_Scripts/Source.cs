using UnityEngine.UI;

public abstract class Source : Module
{
    public abstract string MaterialName { get; protected set; }

    private Image displayImage;
    private void Start()
    {
        displayImage = GetComponentInChildren<Image>();
        displayImage.color = normalColor;
    }
}
