using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingFloor : MonoBehaviour
{
    [SerializeField] private float _damage = 1;
    [SerializeField] private bool _isDamaging = true;

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (_isDamaging)
            {
                col.gameObject.SendMessage("GetDamage", _damage);
            }
            else
            {
                col.gameObject.SendMessage("Heal", _damage);
            }
        }
    }
}