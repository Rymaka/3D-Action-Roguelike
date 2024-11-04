using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeathLotusBlade : BladeBase
{
    private GameObject _targetEnemy;
    private float _velocity;
    private float _distanceToEnemy;
    private float _damage;
    private bool _damageDealt = false;
    [SerializeField] private Transform _face;
    public void InitializeLothusBlade(GameObject enemy, float _bladeVelocity, float damage)
    {
        _targetEnemy = enemy;
        _velocity = _bladeVelocity;
        _damage = damage;
    }

    private void FixedUpdate()
    {
        if (!_targetEnemy.IsDestroyed())
        {
            _distanceToEnemy = Vector3.Distance(transform.position, _targetEnemy.transform.position);
            MoveToTarget();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void MoveToTarget()
    {
        Vector3 targetPosition = _targetEnemy.transform.position;
        float step = _velocity * Time.deltaTime;
        var _finalPos = targetPosition;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        _face.LookAt(_targetEnemy.transform.position);
        if (_distanceToEnemy < 0.1f && !_damageDealt)
        {
            if (_targetEnemy != null)
            {
                _targetEnemy.SendMessage("GetDamage", _damage);
            }
            _damageDealt = true;
            transform.position = _finalPos;
            Destroy(gameObject);
        }
    }
}
