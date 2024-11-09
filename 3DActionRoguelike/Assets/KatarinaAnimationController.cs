using System.Collections;
using System.Collections.Generic;
using EasyCharacterMovement;
using UnityEngine;

public class KatarinaAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private float _timeToNextIdle = 5f;
    [SerializeField] private Character _character;
    [SerializeField] private int _voracityLayer = 1;
    private float _charMaxWalkingSpeed;
    private float _charWalkingSpeed;
    private MovementMode _movementMode;
    private Vector3 _charVelocity;
    private bool _idleCorutineRunning;
    private bool _isIdle = true;
    private bool _isFirstIdle = true;
    private bool _walking = false;
    private bool _falling = false;
    private bool _running = false;

    private void Start()
    {
        _movementMode = _character.GetMovementMode();
        _charMaxWalkingSpeed = _character._maxWalkSpeed;
    }
    private void Update()
    {
        UpdateCharParams();
        UpdatePlayerState();
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        Idle();
        Walking();
        Running();
        Falling();
    }
    
    private void Walking()
    {
        _anim.SetBool("Walking", _walking);
    }

    private void Running()
    {
        _anim.SetBool("Running", _running);
    }

    private void Falling()
    {
        _anim.SetBool("Falling", _falling);
    }

    private void UpdateCharParams()
    {
        _movementMode = _character.GetMovementMode();
        _charVelocity = _character.GetVelocity();
    }

    private void UpdatePlayerState()
    {
        if (_charVelocity == Vector3.zero && _movementMode == MovementMode.Walking)
        {
            if (!_isFirstIdle && !_isIdle)
            {
                _isFirstIdle = true;
            }
            _isIdle = true;
        }

        if (_charVelocity != Vector3.zero)
        {
            _isIdle = false;
            switch (_movementMode)
            {
                case MovementMode.Walking:
                    CheckIfWalking();
                    break;
                
                case MovementMode.Falling:
                    ResetOtherVariables();
                    _falling = true;
                    break;
            }
        }
        
        if(_charVelocity.x == 0f && _charVelocity.z == 0f && (_walking || _running))
        {
            _walking = false;
            _running = false;
        }
    }

    private void ResetOtherVariables()
    {
        _isIdle = false; 
        _isFirstIdle = true;
        _walking = false;
        _falling = false;
        _running = false;
        _anim.ResetTrigger("Idle");
        StopCoroutine(IdleCorutine());
    }

    private void Idle()
    {
        if (_isIdle)
        {
            if (_isFirstIdle)
            {
                _anim.SetTrigger("IdleFirst");
                _isFirstIdle = false;
            }
            if (!_idleCorutineRunning)
            {
                _idleCorutineRunning = true;
                StartCoroutine(IdleCorutine());
            }
            _anim.ResetTrigger("IdleFirst");
        }
        else
        {
            _isFirstIdle = true;
            StopCoroutine(IdleCorutine());
        }
    }

    private void CheckIfWalking()
    {
        _charWalkingSpeed = _character._maxWalkSpeed;
        if (_charMaxWalkingSpeed == _charWalkingSpeed)
        {
            ResetOtherVariables();
            _walking = true;
        }
        if(_charWalkingSpeed > _charMaxWalkingSpeed)
        {
            ResetOtherVariables();
            _running = true;
        }
    }
    private IEnumerator IdleCorutine()
    {
        yield return new WaitForSeconds(_timeToNextIdle);
        _anim.SetTrigger("Idle");
        _idleCorutineRunning = false;
    }

    public void BouncingBladeAnimation()
    {
        _anim.SetLayerWeight (_voracityLayer, 1f);
        _anim.SetTrigger("QSkill");
    }
    public void VoracityAnimation()
    {
        _anim.SetLayerWeight (_voracityLayer, 1f);
        _anim.SetTrigger("Voracity");
    }
}
