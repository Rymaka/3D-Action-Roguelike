using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voracity : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _cooldown;
    [SerializeField] private GameObject _voracracityBlades;
    [SerializeField] private KatarinaAnimationController _animController;
    private bool _isCooldown = false;
    private DeathLotus _deathLotus;
    private Shunpo _shunpo;
    private BouncingBlade _bouncingBlade;
    private Preparation _preparation;

    private void Start()
    {
        _deathLotus = PlayerReference.Player.GetComponent<DeathLotus>();
        _bouncingBlade = PlayerReference.Player.GetComponent<BouncingBlade>();
        _preparation = PlayerReference.Player.GetComponent<Preparation>();
        _shunpo = PlayerReference.Player.GetComponent<Shunpo>();
        Debug.Log(_deathLotus);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
    public void ActivateVoracity(GameObject blade)
    {
        if (!_isCooldown)
        { 
            _animController.VoracityAnimation();
            _voracracityBlades.SetActive(true);
            Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
            List<Collider> enemies = new List<Collider>();

            foreach (var col in colliders)
            {
                if (col.CompareTag("Enemy"))
                {
                    enemies.Add(col);
                }
            }

            if (enemies.Count > 0)
            {
                foreach (var en in enemies)
                {
                    en.gameObject.SendMessage("GetDamage", _damage);
                }
            }
            Debug.Log("Voracity activated");
            _shunpo.VoracracityDecreaseCooldown();
            Destroy(blade);
            StartCoroutine(CooldownTimer());
            _isCooldown = true;
        }
    }

    private IEnumerator CooldownTimer()
    {
        yield return new WaitForSeconds(_cooldown);
        _isCooldown = false;
    }

    public void Innate()
    {
        Debug.Log("Innate");
        _deathLotus.DecreaseCooldown();
        _bouncingBlade.DecreaseCooldown();
        _shunpo.DecreaseCooldown();
        
    }
}
