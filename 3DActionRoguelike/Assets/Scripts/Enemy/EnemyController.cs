using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 1;
    private int _hp;
    private NavMeshAgent _agent;
    private GameObject _player;

    private void Start()
    {
        _player = PlayerReference.Player;
        _hp = _maxHealth;
        _agent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        if (_agent != null & _player != null)
        {
            _agent.SetDestination(_player.transform.position);
        }
    }
    public void GetDamage(int damage)
    {
        if (_hp > 0)
        {
            _hp -= damage;
        }
        CheckHP();
    }

    public void Heal(int heal)
    {
        if (_hp < _maxHealth)
        {
        _hp += heal;
        }
        CheckHP();
    }

    private void CheckHP()
    {
        if (_hp <= 0)
        {
            Destroy(gameObject);
        }
        if (_hp > _maxHealth)
        {
            _hp = _maxHealth;
        }
    }

}
