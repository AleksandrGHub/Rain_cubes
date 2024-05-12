using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private GameObject _spawnArea;
    [SerializeField] private float _repeatRate = 1f;
    [SerializeField] private int _poolCapacity = 10;
    [SerializeField] private int _poolMaxSize = 10;

    private ObjectPool<GameObject> _pool;
    private List<Cube> _cubes = new List<Cube>();

    private void OnDisable()
    {
        foreach (var cube in _cubes)
        {
            if (cube != null)
            {
                cube.CollisionDetected -= ReleaseObject;
            }
        }
    }

    private void Awake()
    {
        _pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(_prefab),
            actionOnGet: (obj) => ActionOnGet(obj),
            actionOnRelease: (obj) => ActionOnRelease(obj),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize);
    }

    private void Start()
    {
        InvokeRepeating(nameof(GetCube), 0.0f, _repeatRate);
    }

    private void ActionOnGet(GameObject obj)
    {
        obj.transform.position = GetPosition();
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.GetComponent<Renderer>().material.color = Color.green;
        obj.SetActive(true);
    }

    private void ActionOnRelease(GameObject obj)
    {
        obj.SetActive(false);
        obj.GetComponent<Cube>().AssignCanReleaseFalse();
        obj.GetComponent<Cube>().AssignIsTouchFalse();
    }

    private void GetCube()
    {
        var cube = _pool.Get().GetComponent<Cube>();
        cube.CollisionDetected += ReleaseObject;
        _cubes.Add(cube);
    }

    private void ReleaseObject()
    {
        foreach (var cube in _cubes)
        {
            if (cube.CanRelease)
            {
                _pool.Release(cube.gameObject);
            }
        }
    }

    private Vector3 GetPosition()
    {
        int divider = 2;
        int minPositionX = -System.Convert.ToInt32(_spawnArea.transform.localScale.x / divider);
        int maxPositionX = System.Convert.ToInt32(_spawnArea.transform.localScale.x / divider);
        int minPositionZ = -System.Convert.ToInt32(_spawnArea.transform.localScale.z / divider);
        int maxPositionZ = System.Convert.ToInt32(_spawnArea.transform.localScale.z / divider);
        int pozitionX = Random.Range(minPositionX, maxPositionX) + System.Convert.ToInt32(_spawnArea.transform.position.x);
        int pozitionY = System.Convert.ToInt32(_spawnArea.transform.position.y);
        int pozitionZ = Random.Range(minPositionZ, maxPositionZ) + System.Convert.ToInt32(_spawnArea.transform.position.z);
        return new Vector3(pozitionX, pozitionY, pozitionZ);
    }
}