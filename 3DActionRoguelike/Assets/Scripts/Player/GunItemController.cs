using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GunItemController : MonoBehaviour
{
    [SerializeField] private float _maxRange = 100f;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _shootingPoint;
    private GameObject[] _enemies;
    private GameObject _closestEnemy;
    private bool _isCD;


    private void Update()
    {
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (_enemies.Length > 0)
        {
            GetClosestEnemy();
            transform.LookAt(_closestEnemy.transform);
            Shoot();
        }


    }

    private void Shoot()
    {
        if (_closestEnemy != null && !_isCD)
        {
            if((Vector3.Distance(_closestEnemy.transform.position, transform.position)) <= _maxRange)
            {
                Instantiate(_bullet, _shootingPoint.position, _shootingPoint.rotation);
                _isCD = true;
                StartCoroutine(Cooldown());
            }
        }
    }


    private void GetClosestEnemy()
    {
        _closestEnemy = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject go in _enemies)
        {
            Vector3 directionToTarget = go.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                _closestEnemy = go;
            }
        }

    }
    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        _isCD = false;
    }

}
