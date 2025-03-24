using UnityEngine;
using UnityEngine.Pool;

public abstract class Spawner<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T ObjectPrefab;

    protected ObjectPool<T> Pool;

    protected float MinRotationY;
    protected float MaxRotationY;

    protected float MinPositionX;
    protected float MaxPositionX;
    protected float MinPositionZ;
    protected float MaxPositionZ;

    protected int PoolCapacity = 3;
    protected int PoolMaxSize = 3;

    protected void Awake()
    {
        Pool = new ObjectPool<T>
            (
                 createFunc: () => Instantiate(ObjectPrefab),
                 actionOnGet: OnGet,
                 actionOnRelease: (@object) => OnRelease(@object),
                 actionOnDestroy: (@object) => Destroy(@object.gameObject),
                 collectionCheck: true,
                 defaultCapacity: PoolCapacity,
                 maxSize: PoolMaxSize
            );
    }

    protected abstract void OnGet(T @object);

    protected abstract void OnRelease(T @object);
    
    public void Spawn()
    {
        Pool.Get();
    }

    public void ReleaseObject(T @object)
    {
        Pool.Release(@object);
    }
}