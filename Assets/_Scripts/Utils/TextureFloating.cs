using UnityEngine;

public class SpriteFloating : MonoBehaviour
{
    [SerializeField] private float xSpeed = 0.5f; 
    [SerializeField] private float ySpeed = 0.5f;
    [SerializeField] private float xAmplitude = 1f;
    [SerializeField] private float yAmplitude = 1f;

    private Vector2 startPosition;
    private float timeCounter = 0f;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        timeCounter += Time.deltaTime;

        float newX = startPosition.x + Mathf.Sin(timeCounter * xSpeed) * xAmplitude;
        float newY = startPosition.y + Mathf.Cos(timeCounter * ySpeed) * yAmplitude;

        transform.position = new Vector2(newX, newY);
    }
}