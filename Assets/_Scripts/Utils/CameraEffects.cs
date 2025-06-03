using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    public static CameraEffects Instance { get; private set; }

    private Vector3 _currentBasePosition;
    private float _shakeDuration = 0f;
    private float _shakeAmplitude = 0f;

    private void Awake()
    {
        Instance = this;

        _currentBasePosition = transform.localPosition;
    }

    private void LateUpdate() 
    {
        if (_shakeDuration > 0)
        {
            transform.localPosition = _currentBasePosition + Random.insideUnitSphere * _shakeAmplitude;
            _shakeDuration -= Time.deltaTime;
        }
        else
        {
            _currentBasePosition = transform.localPosition;
        }
    }

    public void ShakeCamera(float amplitude, float duration)
    {
        _shakeAmplitude = amplitude;
        _shakeDuration = duration;
        _currentBasePosition = transform.localPosition;
    }
}