using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100;
    private float _currentHealth = 100;
    [SerializeField] private Slider _healthBar;
    private bool _innateActivated = false;

    protected virtual void OnAwake()
    {
        _currentHealth = _maxHealth;
        UpdateHealthBar();
    }
    
    private void Awake()
    {
        OnAwake();
    }

    protected virtual void Start()
    {
        
    }

    private void Update()
    {
        Cursor.visible = false;
    }

    protected virtual void FixedUpdate()
    {
        
    }
    
    public virtual void GetDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Death();
        }
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (_healthBar != null)
        {
            _healthBar.value = _currentHealth/_maxHealth;
        }
        Debug.Log(_currentHealth);
    }
    

    public virtual void Heal(int heal)
    {
        _currentHealth += heal;
        if (_currentHealth > _maxHealth)
        {
            _currentHealth = _maxHealth;
        }
        UpdateHealthBar();
    }
    protected virtual void Death()
    {
        Debug.Log("Death");
        if (gameObject.tag == "Enemy"&&!_innateActivated)
        {
            Voracity voracityInstance = PlayerReference.Player.GetComponent<Voracity>();
            voracityInstance.Innate();
            _innateActivated = true;
            Destroy(gameObject);
        }
    }
}
