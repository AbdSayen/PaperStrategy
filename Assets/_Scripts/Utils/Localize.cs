using UnityEngine;
using UnityEngine.UI;
//using YG;

public class Localize : MonoBehaviour
{
    [SerializeField][TextArea] private string en;
    [SerializeField][TextArea] private string ru;

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
        text.text = Get(en, ru);
    }

    public static string Get(string en, string ru)
    {
        //switch (YG2.envir.language)
        //{
        //    case "en":
        //        return en;
        //    case "ru":
        //        return ru;
        //    default:
        //        goto case "en";
        //}
        return ru;
    }
}