using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeBase : MonoBehaviour
{
    [SerializeField] private float _timeToLive = 7f;
    private bool _timerActive = false;
    private Voracity _voracity;
    private GameObject _player;
    protected bool _fell = false;
    private void Start()
    {
        _player = PlayerReference.Player;
        _voracity = _player.GetComponent<Voracity>();
    }
    protected virtual void StartTimer()
    {
        if (!_timerActive)
        {
            _timerActive = true;
            StartCoroutine(Timer());
        }
    }

    private void Update()
    {
        if (_fell)
        {
            gameObject.tag = "Knife";
        }
    }
    
    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("Player")&&_fell)
        {
            _voracity.ActivateVoracity(gameObject);
        }
    }
}
