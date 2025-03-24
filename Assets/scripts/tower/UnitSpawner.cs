using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : Spawner<Unit>
{
    public event Action<Unit> Spawned;
    public event Action<Unit> Removed;

    private void Start()
    {
        MinPositionX = -3f;
        MaxPositionX = -1.5f;
        MinPositionZ = -3f;
        MaxPositionZ = 2f;
        MinRotationY = 0;
        MaxRotationY = 360;
        PoolCapacity = 3;
        PoolMaxSize = 3;       
    }

    protected override void OnGet(Unit @object)
    {
        @object.transform.position = new Vector3
            (Random.Range(MinPositionX, MaxPositionX), @object.transform.position.y, Random.Range(MinPositionZ, MaxPositionZ));

        @object.transform.rotation = Quaternion.Euler
            (@object.transform.rotation.x, Random.Range(MinRotationY, MaxRotationY), @object.transform.rotation.z);

        @object.gameObject.SetActive(true);

        @object.Died += ReleaseObject;

        Spawned?.Invoke(@object);
    }

    protected override void OnRelease(Unit @object)
    {
        @object.gameObject.SetActive(false);

        @object.Died -= ReleaseObject;

        Removed?.Invoke(@object);
    }
}