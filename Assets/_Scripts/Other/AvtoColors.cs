using UnityEngine;

public class AvtoColors : MonoBehaviour
{
    public static AvtoColors Instance;

    private void Awake() => Instance = this;

    public void AutoColor()
    {
        return;

        //Module[] modules = FindObjectsByType<Module>(
        //    FindObjectsInactive.Include, 
        //    FindObjectsSortMode.None);

        //for (int i = 0; i < modules.Length; i++) 
        //{
        //    if (modules[i].SpriteRenderer is null)
        //        continue;

        //    if (modules[i].SpriteRenderer.color != modules[i].NormalColor) 
        //    {
        //        modules[i].SpriteRenderer.color = modules[i].NormalColor;
        //    }
        //}
    }
}