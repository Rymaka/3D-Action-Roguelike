using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathLotus : SkillBase
{
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _maxDuration = 5f;
    [SerializeField] private float _damagePerKnife = 10f;
    [SerializeField] private GameObject _knife;
    [SerializeField] private GameObject _spawnPoint1;
    [SerializeField] private GameObject _spawnPoint2;
    [SerializeField] private float _knifeSpeed = 20f;
    [SerializeField] private float _cooldownReduce = 5f;
    private float _timeBetweenKnives = 20f;
    private float _cooldownTimer;
    [SerializeField] private int _maxKnives = 30;
    private int _knives = 0;
    public bool interrupted = false;
    private bool _finised = false;
    private float _duration = 0f;
    private bool _canThrow = true;
    private bool _isCooldown = false;
    private float _elapsedTime;
    private bool _throwing = false;
    private bool _waiting = false;
    private Collider[] _colliders;

    private void Start()
    {
        _timeBetweenKnives = _maxDuration / _maxKnives;
        _cooldownTimer = _cooldown;
    }
    private void Update()
    {
        if (_throwing)
        {
            _colliders = UpdateRadius();
        }
        
        CheckStatus();
        Cast();
        
        if (_startTime > 0f)
        {
            _elapsedTime = Time.time - _startTime;
        }
       
        if (!_throwing&&_waiting)
        {
            WaitEnd();
        }
    }
    protected override void Cast()
    {
        if (_duration <= _maxDuration && _casted && !interrupted)
        {   
            _duration += Time.deltaTime;
            if (!_finised && _knives < _maxKnives && _canThrow)
            {
                _throwing = true;
                StartCoroutine(ThrowKnife());
            }
            else
            {
                _throwing = false;
            }
        
            if (_duration >= _maxDuration)
            {
                _finised = true;
            }
        }
    }
    
    protected override void TryCast(InputAction.CallbackContext context)
    {
        if (_canCast)
        {
            _casted = true;
        }       
    }

    private void CheckStatus()
    {
        if (_casted&&(interrupted || _finised)&&!_isCooldown)
        {
            _isCooldown = true;
            Cooldown();
        }
    }

    private Collider[] UpdateRadius()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        return colliders;
    }
    private IEnumerator ThrowKnife()
    {
        if (_canThrow)
        {
            _colliders = UpdateRadius();
        }
        _canThrow = false;

        List<Collider> enemies = new List<Collider>();

        if (_colliders.Length > 0)
        {
            foreach (var col in _colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    enemies.Add(col);
                }
            }
        }


        if (enemies.Count > 0)
        {
            foreach (var en in enemies)
            {
                if (en.gameObject != null)
                {
                    SpawnBlade(en.gameObject);
                    en.gameObject.SendMessage("GetDamage", _damagePerKnife);
                    _knives++;
                    yield return new WaitForSeconds(_timeBetweenKnives);
                }
            }
        }
        _canThrow = true;
    }
    
    private void SpawnBlade(GameObject enemy)
    {
        GameObject spawnPoint = (Random.Range(0, 2) == 0) ? _spawnPoint1 : _spawnPoint2;
        GameObject blade = Instantiate(_knife, spawnPoint.transform.position, Quaternion.identity);
        DeathLotusBlade bladeScript = blade.GetComponent<DeathLotusBlade>();
        if (bladeScript != null)
        {
            bladeScript.InitializeLothusBlade(enemy, _knifeSpeed, _damagePerKnife);
        }
    }
    
    public void Interrupt()
    {
        if (!interrupted)
        {
            interrupted = true;
            Cooldown();
        }
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
    protected override IEnumerator CooldownTimer()
    {
        _startTime = Time.time;
        yield return new WaitForSeconds(_cooldownTimer);
        Debug.Log("Corutine Ended");
        ResetVariabels();
    }

    private void ResetVariabels()
    {
        if(!_throwing) {
            _canCast = true;
            _casted = false;
            interrupted = false;
            _finised = false;
            _duration = 0;
            _knives = 0;
            _isCooldown = false;
            _cooldownTimer = _cooldown;
            _elapsedTime = 0f;
            _startTime = 0f;
            _waiting = false;
            Debug.Log("cooldowned");
        }
        else
        {
            _waiting = true;
        }
        

    }

    private void WaitEnd()
    {
        ResetVariabels();
    }
    
    
}
