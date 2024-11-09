using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BouncingBlade : SkillBase
{
    [SerializeField] private GameObject _bladePf;
    [SerializeField] private float _bladeVelocity;
    [SerializeField] private Transform _bladeSpawnPoint;
    [SerializeField] private float _skillRadius = 35f;
    [SerializeField] private float _bounceDistance = 15f;
    [SerializeField] private float _bounceTime = 1f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private int _maxBounces = 2;
    [SerializeField] private LayerMask _ignoreMask;
    [SerializeField] private Transform _raycastPoint;
    [SerializeField] private float _arcDuration = 0.5f;
    [SerializeField] private float _arcMultiplier = 2f;
    [SerializeField] private float _backOffset = -2f;
    [SerializeField] private float _fallingHeight = 1.3f;
    [SerializeField] private float _fallingDuration = 1.3f;
    [SerializeField] private float _cooldownReduce = 15f;
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

        Debug.DrawRay(rayOrigin, rayDirection * _skillRadius, Color.blue, 2.0f);

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, _skillRadius, ~_ignoreMask))
        {
            _casted = true;
            if (hit.collider.CompareTag("Enemy"))
            {
                SpawnBlade(hit.collider.gameObject);
                _casted = true;
            }
        }
    }

    protected override IEnumerator CooldownTimer()
    {
        _uiElement.StartCooldown(_cooldownTimer);
        _startTime = Time.time;
        Debug.Log(_cooldownTimer + " " + _cooldown + " corutine");
        yield return new WaitForSeconds(_cooldownTimer);
        ResetVariabels();
    }

    private void ResetVariabels()
    {
        _cooldownTimer = _cooldown;
        _startTime = 0f;
        _elapsedTime = 0f;
        _canCast = true;
        _casted = false;
        _uiElement.ResetUI();
        StopAllCoroutines();
    }
    private void SpawnBlade(GameObject enemy)
    {
        GameObject blade = Instantiate(_bladePf, _bladeSpawnPoint.position, Quaternion.identity);
        BouncingBladeBlade bladeScript = blade.GetComponent<BouncingBladeBlade>();
        if (bladeScript != null)
        {
            bladeScript.InitializeBouncingBlade(enemy, _bladeVelocity, _bounceDistance, _maxBounces,_damage,_arcDuration,_arcMultiplier, transform.position, _backOffset,_fallingHeight,_fallingDuration);
        }
    }
    
    

    public void DecreaseCooldown()
    {
        _cooldownTimer -= (_cooldownReduce + _elapsedTime);
        StopCoroutine(CooldownTimer());
        if (_cooldownTimer > 0)
        {
            StartCoroutine(CooldownTimer());
            _uiElement.StartCooldown(_cooldownTimer);
        }
        else
        {
            ResetVariabels();
        }
    }
    
}