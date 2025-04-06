using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;

    private ObjectPool<Resource> _pool;

    private float _delay = 1f;

    private float _rotationX = -120;

    private float _incorrectPosition = 2;

    private float _minRotationY = -208f;
    private float _maxRotationY = -73f;
    private float _minPositionX = -6.6f;
    private float _maxPositionX = 6.6f;
    private float _minPositionZ = -6.6f;
    private float _maxPositionZ = 6.6f;

    private int _poolCapacity = 3;
    private int _poolMaxSize = 3;

    private void Awake()
    {
        _pool = new ObjectPool<Resource>
            (
                 createFunc: () => Instantiate(_resourcePrefab),
                 actionOnGet: OnGet,
                 actionOnRelease: (@object) => OnRelease(@object),
                 actionOnDestroy: (@object) => Destroy(@object.gameObject),
                 collectionCheck: true,
                 defaultCapacity: _poolCapacity,
                 maxSize: _poolMaxSize
            );
    }

    private void Start()
    {
        StartCoroutine(SpawnDelaying());
    }

    private void ReleaseObject(Resource resource)
    {
        _pool.Release(resource);
    }

    private void OnGet(Resource resource)
    {
        resource.transform.position = new Vector3
            (Random.Range(_minPositionX, _maxPositionX), resource.transform.position.y, Random.Range(_minPositionZ, _maxPositionZ));

        resource.transform.rotation = Quaternion.Euler
            (_rotationX, Random.Range(_minRotationY, _maxRotationY), resource.transform.rotation.z);

        if (TryGetCorrectPosition(resource))
        {
            resource.gameObject.SetActive(true);

            resource.Collected += ReleaseObject;
        }
        else
        {
            ReleaseObject(resource);
        }
    }

    private void OnRelease(Resource resource)
    {
        resource.gameObject.SetActive(false);

        resource.Collected -= ReleaseObject;
    }

    private bool TryGetCorrectPosition(Resource resource)
    {
        if (resource.transform.position.x <= _incorrectPosition && resource.transform.position.z <= _incorrectPosition)
            return false;

        return true;
    }

    private void Spawn()
    {
        _pool.Get();
    }

    private IEnumerator SpawnDelaying()
    {
        WaitForSeconds wait = new WaitForSeconds(_delay);

        while(enabled)
        {
            Spawn();

            yield return wait;
        }
    }
}