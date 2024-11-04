using System;
using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using UnityEngine;

public class Preparation : SkillBase
{
    [SerializeField] private float _duration;
    [SerializeField] private float _speedBoost = 0.3f;
    [SerializeField] private Character _player;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _baseBladePf;
    private bool _isSpeedBoosted = false;
    private float _baseSpeed;

     private void Start()
     {
         _baseSpeed = _player._maxWalkSpeed;
     }

     protected override void Cast()
    {
        Instantiate(_baseBladePf, _spawnPoint.position, Quaternion.identity);
        if (!_isSpeedBoosted)
        {
            _player._maxWalkSpeed *= (_speedBoost+1);
            _isSpeedBoosted = true;
            StartCoroutine(SpeedBoostCoroutine());
        }

        _casted = true;
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        yield return new WaitForSeconds(_duration);
        _isSpeedBoosted = false;
        _player._maxWalkSpeed = _baseSpeed;
    }
}
