using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private int _damage = 1;

    private void FixedUpdate()
    {
        _rb.velocity = transform.forward * _speed;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Enemy")
        {
            col.gameObject.SendMessage("GetDamage", _damage);
            Destroy(gameObject);
        }
        if (col.tag == "Wall")
        {
            Destroy(gameObject);
        }
        
    }
}
