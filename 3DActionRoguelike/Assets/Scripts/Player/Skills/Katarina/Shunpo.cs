using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using UnityEngine;
using UnityEngine.InputSystem;


public class Shunpo : SkillBase
{
    [SerializeField] private float _skillRadius = 35f;
    [SerializeField] private LayerMask _ignoreMask;
    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private GameObject _player;
    [SerializeField] private float _distanceFromEnemy = 1.5f; 
    [SerializeField] private CharacterMovement _characterMovement;
    [SerializeField] private float _cooldownReduce = 15f;
    [SerializeField] private float _voracracityCDReduce = 15f;
    private float _elapsedTime = 0f;
    private float _cooldownTimer = 0f;

    
    private void Start()
    {
        _cooldownTimer = _cooldown;
    }
    
    private void Update()
    {
        if (_startTime > 0f)
        {
            _elapsedTime = Time.time - _startTime;
        }
    }
    protected override void Cast()
    {
        var rayOrigin = _raycastPoint.position;
        var rayDirection = Camera.main.transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * _skillRadius, Color.red, 2.0f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _skillRadius, ~_ignoreMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Vector3 enemyPosition = hit.point;
                Vector3 playerPosition = _player.transform.position;
                Vector3 directionToPlayer = (playerPosition - enemyPosition).normalized;
                Vector3 newTeleportPosition = enemyPosition + directionToPlayer * _distanceFromEnemy;
                _characterMovement.RemoveVelocity();
                _player.transform.position = newTeleportPosition;
                _characterMovement.position = newTeleportPosition;
                _casted = true;
                _characterMovement._canMove = true;
            }

            if (hit.collider.CompareTag("Knife"))
            {
                _characterMovement.RemoveVelocity();
                _player.transform.position = hit.collider.transform.position;
                _characterMovement.position = hit.collider.transform.position;
                Debug.Log(hit.collider.transform.position);
                _casted = true;
                Debug.Log(_player.transform.position);
                _characterMovement._canMove = true;
            }

        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }
    

    private void ResetVariabels()
    {
        _cooldownTimer = _cooldown;
        _startTime = 0f;
        _elapsedTime = 0f;
        _canCast = true;
        _casted = false;
    }
    
    protected override IEnumerator CooldownTimer()
    {
        _startTime = Time.time;
        yield return new WaitForSeconds(_cooldown);
        ResetVariabels();
    }
    
    public void DecreaseCooldown()
    {
        _cooldownTimer -= (_cooldownReduce + _elapsedTime);
        StopCoroutine(CooldownTimer());
        if (_cooldownTimer > 0)
        {
            StartCoroutine(CooldownTimer());
        }
        else
        {
            ResetVariabels();
        }
    } 
    
    public void VoracracityDecreaseCooldown()
    {
        _cooldownTimer -= (_cooldownReduce + _elapsedTime);
        StopCoroutine(CooldownTimer());
        if (_cooldownTimer > 0)
        {
            StartCoroutine(CooldownTimer());
        }
        else
        {
            ResetVariabels();
        }
    }
    
}

