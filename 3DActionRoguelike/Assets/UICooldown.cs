using System.Collections;
using UnityEngine;

public class UICooldown : MonoBehaviour
{
    [SerializeField] private RectTransform _cooldownIcon;
    [SerializeField] private float _endPosition; // Only Y axis
    [SerializeField] private SkillBase _skill;
    private float _cooldownTimer;
    private float _cooldownOrig;
    private Vector2 _originalPosition;
    private bool _onCooldown = false;
    private bool _animating = false;
    private float _elapsedCooldownTime = 0f;

    private void Start()
    {
        _cooldownOrig = _skill._cooldown;
        _originalPosition = _cooldownIcon.anchoredPosition;
    }

    private void Update()
    {
        if (_onCooldown)
        { 
            AnimateCooldown();
        }
        else
        {
            _cooldownIcon.anchoredPosition = new Vector2(_originalPosition.x, _endPosition);
        }
    }

    private void AnimateCooldown()
    {

        if (!_animating)
        {
            _animating = true;
        }

        _elapsedCooldownTime += Time.deltaTime;
        //Debug.Log(_elapsedCooldownTime+ " elapsedCooldownTime " + _cooldownTimer + " cooldown " );
        float cooldownPercent = Mathf.Clamp01(_elapsedCooldownTime / _cooldownOrig);
        float newYPosition = Mathf.Lerp(_originalPosition.y, _endPosition, cooldownPercent);
        _cooldownIcon.anchoredPosition = new Vector2(_originalPosition.x, newYPosition);

        if (_elapsedCooldownTime >= _cooldownOrig)
        {
            ResetUI();
        }
    }

    public void StartCooldown(float cooldownTimer)
    {
        _cooldownOrig = _skill._cooldown;
        _cooldownTimer = cooldownTimer;
        float cooldownProcent = 1f;
        if (_onCooldown)
        {
            StopAllCoroutines();
            _onCooldown = false;
        }
        
        if (_cooldownTimer > 0)
        {
            cooldownProcent = _cooldownTimer / _cooldownOrig;
        }

        float startPercent = 1f - cooldownProcent;
        
        _elapsedCooldownTime = startPercent * _cooldownOrig;
        MoveToProcent(startPercent);

        
        _onCooldown = true;
        _animating = false;

        if (_cooldownTimer > 0)
        {
            StartCoroutine(CooldownTimer());
        }
        else
        {
            ResetUI();
        }
    }

    private void MoveToProcent(float cooldownProcent)
    { 
        float newYPosition = Mathf.Lerp(_originalPosition.y, _endPosition, cooldownProcent);
        _cooldownIcon.anchoredPosition = new Vector2(_cooldownIcon.anchoredPosition.x, newYPosition);
    }

    private IEnumerator CooldownTimer()
    {
        Debug.Log("coroitne Started " + _cooldownTimer);
        yield return new WaitForSeconds(_cooldownTimer);
        ResetUI();
    }

    public void ResetUI()
    {
        StopAllCoroutines();
        _cooldownIcon.anchoredPosition = new Vector2(_originalPosition.x, _endPosition);
        _onCooldown = false;
        _animating = false;
    }
}
