using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class PickupItem : MonoBehaviour
{
    [Header("Анимация предмета")]
    [Label("изменение высоты"), SerializeField] AnimationCurve _heightPos;
    [Label("Скорость поворота"), SerializeField] AnimationCurve _RotationSpeed;

    float _currentHeightTime, _totalHeightTime;
    float _currentRotationTime, _totalRotationTime;

    void Start()
    {
        _totalHeightTime = _heightPos.keys[_heightPos.keys.Length - 1].time;
        _totalRotationTime = _RotationSpeed.keys[_RotationSpeed.keys.Length - 1].time;
    }


    void Update()
    {
        transform.position = new Vector3(transform.position.x, _heightPos.Evaluate(_currentHeightTime), transform.position.z);
        transform.Rotate(Vector3.up, _RotationSpeed.Evaluate(_currentRotationTime) * Time.deltaTime);

        _currentHeightTime += Time.deltaTime;

        if (_currentHeightTime >= _totalHeightTime)
        {
            _currentHeightTime = 0;
        }

        _currentRotationTime += Time.deltaTime;

        if (_currentRotationTime >= _totalRotationTime)
        {
            _currentRotationTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}
