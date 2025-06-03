using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messages : MonoBehaviour
{
    [SerializeField] private Text _textUI;
    [SerializeField] private GameObject _panel;

    private static Text textUI;
    private static GameObject panel;
    private static CanvasGroup panelCanvasGroup;
    private static CanvasGroup textCanvasGroup;
    private static Queue<(string message, float duration)> messageQueue = new Queue<(string, float)>();
    private static bool isDisplayingMessage = false;

    private void Start()
    {
        textUI = _textUI;
        panel = _panel;

        panelCanvasGroup = panel.GetComponent<CanvasGroup>() ?? panel.AddComponent<CanvasGroup>();
        textCanvasGroup = textUI.GetComponent<CanvasGroup>() ?? textUI.gameObject.AddComponent<CanvasGroup>();

        panelCanvasGroup.alpha = 0f;
        textCanvasGroup.alpha = 0f;
    }

    public static void Push(string message, GameObject watchTarget = null, float duration = 2f)
    {
        messageQueue.Enqueue((message, duration));
        if (!isDisplayingMessage)
        {
            CoroutineRunner.Instance.StartCoroutine(ProcessQueue());
        }
    }

    public void Push(string message)
    {
        Push(message, duration: 2);
    }

    private static IEnumerator ProcessQueue()
    {
        isDisplayingMessage = true;

        while (messageQueue.Count > 0)
        {
            var (message, duration) = messageQueue.Dequeue();
            yield return MessageRoutine(message, duration);
        }

        isDisplayingMessage = false;
    }

    private static IEnumerator MessageRoutine(string message, float duration)
    {
        textUI.text = message;
        yield return Fade(panelCanvasGroup, 1f, 0.2f);
        yield return Fade(textCanvasGroup, 1f, 0.2f);

        yield return new WaitForSeconds(duration);

        yield return Fade(textCanvasGroup, 0f, 0.25f);
        yield return Fade(panelCanvasGroup, 0f, 0.25f);

        textUI.text = string.Empty;
    }

    private static IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}
