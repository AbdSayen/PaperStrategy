using System.Linq;
using UnityEngine;

public abstract class Source : Module
{
    public abstract string MaterialName { get; protected set; }

    private SpriteRenderer displayImage;

    public override void SetColor(Color color)
    {
        base.SetColor(color);

        displayImage = GetComponentsInChildren<SpriteRenderer>(true)
            .FirstOrDefault(sr => sr.transform != transform);

        if (displayImage != null) displayImage.color = NormalColor;
    }
}