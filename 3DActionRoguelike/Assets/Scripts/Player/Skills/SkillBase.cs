using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class SkillBase : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    [SerializeField] protected bool _canCast = true;
    [SerializeField] private InputActionReference _inputAction;
    [SerializeField] protected bool _casted = false;
    protected float _startTime = 0f;

    protected virtual void OnAwake()
    {
        
    }

    private void Awake()
    {
        OnAwake();
    }

    protected virtual void OnEnable()
    {
        _inputAction.action.performed += TryCast;
        _inputAction.action.Enable();
    }
    private void OnDisable()
    {
        _inputAction.action.performed -= TryCast;
        _inputAction.action.Disable();
    }

    protected virtual void TryCast(InputAction.CallbackContext context)
    {
        if (_canCast)
        {
            Cast();
            if (_casted)
            {
                Cooldown();
            }
        }
    }

    protected virtual void Cast()
    {
        
    }
    protected virtual void Cooldown()
    {
        _canCast = false;
        StartCoroutine(CooldownTimer());
    }

    protected virtual IEnumerator CooldownTimer()
    {
        _startTime = Time.time;
        yield return new WaitForSeconds(_cooldown);
        _canCast = true;
        _casted = false;
    }
    
}
