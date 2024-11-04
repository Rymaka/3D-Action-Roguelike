using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableParticles : MonoBehaviour
{
    [SerializeField] private GameObject _particles;

    public void ActivateAnimation()
    {
        _particles.SetActive(true);
    }

    public void DisableAnimation()
    {
        _particles.SetActive(false);
    }
}
