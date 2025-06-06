using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public static DebugText Instance;
    private Text textObj;
    private void Awake() => Instance = this;
    private void Start() => textObj = GetComponent<Text>();
    public static void UpdateText(string text) => Instance.textObj.text = text;
}