using System;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBladeBlade : BladeBase
{
    private GameObject _targetEnemy;
    private Vector3 _spawnPosition;
    private float _velocity;
    [SerializeField] private float _fallSpeed;
    private float _bounceDistance;
    private float _groundLevel;
    private int _maxBounces;
    private int _bounceCount = 0;
    [SerializeField] private float _knifeFallSpeed = 2f;
    [SerializeField] private LayerMask _groundMask;

    private float _damage = 10f;
    private float _distanceToEnemy;
    private bool _damageDealt = false;
    private bool _rememberedPos = false;
    private Vector3 _finalPos;
    private GameObject _bounceTarget;
    private Vector3 _arcStartPos;
    private Vector3 _arcEndPos;
    private float _arcTime = 0f;
    private float _arcDuration = 0.5f;
    private float _fallingDuration;
    private float _arcMultiplier = 2f;
    private float _fallingHeight = 2f;
    private float _backOffset;
    private HashSet<GameObject> _hitEnemies = new HashSet<GameObject>();
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _face;

    private void Awake()
    {
        _hitEnemies = new HashSet<GameObject>();
    }

    public void InitializeBouncingBlade(GameObject targetEnemy, float velocity, float bounceDistance, int maxBounces, float damage, float arcDuration, float arcMultiplier, Vector3 playerPosition, float backOffset, float fallingHeight, float fallingDuration)
    {
        _targetEnemy = targetEnemy;
        _velocity = velocity;
        _bounceDistance = bounceDistance;
        _maxBounces = maxBounces;
        _damage = damage;
        _arcDuration = arcDuration;
        _arcMultiplier = arcMultiplier;
        _spawnPosition = playerPosition;
        _backOffset = backOffset;
        _fallingHeight = fallingHeight;
        _fallingDuration = fallingDuration;
    }

    private void FixedUpdate()
    {
        if (_targetEnemy == null)
        {
            Destroy(gameObject);
            return;
        }
        _distanceToEnemy = Vector3.Distance(transform.position, _targetEnemy.transform.position);

        if(_bounceCount == 0)
        {
            MoveDirectlyToTarget();
        }
        if (_bounceCount>0 && _bounceCount<_maxBounces)
        {
            MoveInArc();
        }

        if (_bounceCount >= _maxBounces)
        {
            RollAndFall();
        }

    }

    private void MoveDirectlyToTarget()
    {
        Vector3 targetPosition = _targetEnemy.transform.position;
        float step = _velocity * Time.deltaTime;
        if (!_rememberedPos)
        {
            _finalPos = targetPosition;
            _rememberedPos = true;
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        _face.LookAt(_targetEnemy.transform.position);
        if (_distanceToEnemy < 0.1f && !_damageDealt)
        {
            if (_targetEnemy != null)
            {
                _targetEnemy.SendMessage("GetDamage", _damage);
                BladeHit(_targetEnemy);
            }

            _damageDealt = true;
            BounceToNextEnemy();
        }
        _hitEnemies.Add(_targetEnemy);
    }

    private void BounceToNextEnemy()
    {
        _bounceCount++;
        FindClosestEnemy();

        if (_bounceTarget != null && _bounceCount < _maxBounces)
        {
            _arcStartPos = transform.position;
            _arcEndPos = _bounceTarget.transform.position;
            _arcTime = 0f;
            _targetEnemy = _bounceTarget;
            _bounceTarget = null;
            _damageDealt = false;
        }
        else
        {
            RollAndFall();
        }
    }

    private void MoveInArc()
    {
        _arcTime += Time.deltaTime;
        float t = _arcTime / _arcDuration;
        if (t > 1f)
        {
            t = 1f;
        }
        
        float height = Mathf.Sin(t * Mathf.PI) * _arcMultiplier;
        Vector3 arcPosition = Vector3.Lerp(_arcStartPos, _arcEndPos, t);
        
        arcPosition.y += height; 
        _face.LookAt(arcPosition);
        transform.position = arcPosition;
        if (t >= 1f)
        {
            _arcTime = 0f; 
            transform.position = _arcEndPos;
            _targetEnemy.SendMessage("GetDamage", _damage);
            BounceToNextEnemy();
        }
    }

    private void FindClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _bounceDistance);
        List<Collider> enemies = new List<Collider>();

        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy") && col.gameObject != _targetEnemy && !_hitEnemies.Contains(col.gameObject))
            {
                enemies.Add(col);
            }
        }

        enemies.Sort((a, b) =>
            Vector3.Distance(transform.position, a.transform.position)
                .CompareTo(Vector3.Distance(transform.position, b.transform.position))
        );

        if (enemies.Count > 0)
        {
            _bounceTarget = enemies[0].gameObject;
            _hitEnemies.Add(_bounceTarget);
        }
        else
        {
            _bounceTarget = null;
        }
    }

    private void BladeHit(GameObject enemy)
    {
        if (enemy.CompareTag("Enemy"))
        {
            _hitEnemies.Add(enemy);
        }
    }

    private void RollAndFall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, _groundMask))
        {
            _groundLevel = hit.point.y;
            _arcEndPos = FindBackPosition();
            _arcStartPos = transform.position;
            if (transform.position.y >= _groundLevel && !_fell)
            {
                _arcTime += Time.deltaTime;
                float t = _arcTime / _fallingDuration;
                if (t > 1f)
                {
                    t = 1f;
                }
        
                float height = Mathf.Sin(t * Mathf.PI) * _fallingHeight;
                Vector3 arcPosition = Vector3.Lerp(_arcStartPos, _arcEndPos, t);
        
                arcPosition.y += height; 
                _face.LookAt(arcPosition);
                transform.position = arcPosition;
                if (t >= 1f)
                {
                    _arcTime = 0f; 
                    transform.position = _arcEndPos;
                }
            }

        }
        if(transform.position.y <= _groundLevel && !_fell)
        {
            _fell = true;
            StartTimer();
        }

        _bounceCount = _maxBounces;

    }
    
    private Vector3 FindBackPosition()
    {
        Vector3 direction = (_finalPos - _spawnPosition).normalized;
        Vector3 backPos = _finalPos + (-direction * _backOffset);
        
        backPos.y = _groundLevel;
        return backPos;
    }

}