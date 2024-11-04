using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawnerController : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    private GameObject[] _spawners;
    private GameObject _player;
    [SerializeField] private float _spawnCooldown = 10f;
    [SerializeField] private float _spawnerVFXTimer = 2.5f;
    [SerializeField] private int _enemiesInOnePack = 5;
    private int _numActivatedSpawners = 5;
    private bool _cooldown = false;


    private void Awake()
    {
        _spawners = GameObject.FindGameObjectsWithTag("Spawner");
        Debug.Log(_spawners.Length);
        DisableAllMeshes();
    }

    private void Start()
    {
        _player = PlayerReference.Player;
        AddAllSpawnersToArray();
        FindClosestSpawners();
    }

    private void Update()
    {
        if (!_cooldown)
        {
            StartCoroutine(SpawnEnemies());
        }
    }


    private IEnumerator SpawnEnemies()
    {
        _cooldown = true;
        FindClosestSpawners();
        var cooldown = 0f;
        for (int i = 0; i <= _numActivatedSpawners; i++)
        {
            if (i < _enemiesInOnePack)
            {
               cooldown = 0f;
            }
            else
            {
                cooldown = _spawnCooldown;
            }
            var ps = _spawners[i].GetComponent<EnableParticles>();
            ps.ActivateAnimation();
           // yield return new WaitForSeconds(cooldown - (cooldown - _spawnerVFXTimer));

            Instantiate(_enemy, _spawners[i].transform.position, Quaternion.identity);
            ps.DisableAnimation();
            Debug.Log("spawn");
        }
        yield return new WaitForSeconds(cooldown );
        _cooldown = false;

    }




    private void DisableAllMeshes()
    {
        foreach (GameObject go in _spawners)
        {
            go.GetComponent<Renderer>().enabled = false;
        }
    }

    private void AddAllSpawnersToArray()
    {
        int i = 0;
        foreach (GameObject go in _spawners)
        {
            _spawners[i] = go;
            i++;
        }
    }

    private void FindClosestSpawners()
    {
        System.Array.Sort<GameObject>(_spawners,
        (go1, go2) =>
            (Vector3.Distance(_player.transform.position, go1.transform.position) <
             Vector3.Distance(_player.transform.position, go2.transform.position)) ? -1 : 1);
    }
}
