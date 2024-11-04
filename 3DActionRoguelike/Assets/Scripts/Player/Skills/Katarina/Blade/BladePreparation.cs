using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladePreparation : BladeBase
{
    private float _groundLevel;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _face;
    [SerializeField] private float _fallingDuration = 0.5f;
    [SerializeField] private float _fallingHeight = 2f;
    private Vector3 _startPosition;
    private Vector3 _groundPos;
    private float _arcTime = 0f;

    private void Awake()
    {
        _face.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _groundMask))
        {
            _groundLevel = hit.point.y;
            _groundPos = new Vector3(transform.position.x, _groundLevel, transform.position.z);
            if (transform.position.y >= _groundLevel && !_fell)
            {
                _arcTime += Time.deltaTime;
                float t = _arcTime / _fallingDuration;
                if (t > 1f)
                {
                    t = 1f;
                }

                float height = Mathf.Sin(t * Mathf.PI) * _fallingHeight;
                var arcPosition = Vector3.Lerp(_startPosition, _groundPos, t);

                arcPosition.y += height;
                Debug.Log(arcPosition);
                _face.LookAt(arcPosition);
                transform.position = arcPosition;
                if (t >= 1f)
                {
                    _arcTime = 0f;
                    transform.position = _groundPos;
                    if (!_fell)
                    {
                        StartTimer();
                    }
                    _fell = true;
                }
            }
        }
    }
}
