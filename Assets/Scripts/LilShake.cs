using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LilShake : MonoBehaviour
{
    [SerializeField] float _stillInterval = 3f;
    [SerializeField] float _shakingInterval = 0.4f;
    [SerializeField] float _speed = 1.0f;
    [SerializeField] private float _amount = 1.0f;
    [SerializeField] Vector3 _originalPos = Vector3.zero;
    private Vector3 _shakePos;
    private float _timer = 0;
    private RectTransform _rectTransform;

    public float Amount { get => _amount; set => _amount = value;}
    public float Speed { get => _speed; set => _speed = value;}

    private void OnEnable()
    {
        _rectTransform = _rectTransform ?? GetComponent<RectTransform>();
        _originalPos = _originalPos == Vector3.zero ? _rectTransform.position : _originalPos;

        _shakePos = _originalPos;
        _rectTransform.position = _originalPos;
        _timer = 0;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer <= _shakingInterval)
        {
            _shakePos.x = _originalPos.x + Mathf.Sin(Time.time * _speed) * _amount;
            _rectTransform.position = _shakePos;
        }
        else
            _rectTransform.transform.position = _originalPos;
        
        if (_timer > _stillInterval + _shakingInterval)
            _timer = 0;
        
    }
}
