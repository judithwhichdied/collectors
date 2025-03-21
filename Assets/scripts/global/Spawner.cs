using UnityEngine;
using UnityEngine.Pool;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T _objectPrefab;

    protected ObjectPool<T> _pool;

    protected float _minRotationY;
    protected float _maxRotationY;

    protected float _minPositionX;
    protected float _maxPositionX;
    protected float _minPositionZ;
    protected float _maxPositionZ;

    protected int _poolCapacity = 3;
    protected int _poolMaxSize = 3;

    protected void Awake()
    {
        _pool = new ObjectPool<T>
            (
                 createFunc: () => Instantiate(_objectPrefab),
                 actionOnGet: OnGet,
                 actionOnRelease: (@object) => OnRelease(@object),
                 actionOnDestroy: (@object) => Destroy(@object.gameObject),
                 collectionCheck: true,
                 defaultCapacity: _poolCapacity,
                 maxSize: _poolMaxSize
            );
    }

    protected abstract void OnGet(T @object);

    protected abstract void OnRelease(T @object);
    
    public void Spawn()
    {
        _pool.Get();
    }

    public void ReleaseObject(T @object)
    {
        _pool.Release(@object);
    }
}