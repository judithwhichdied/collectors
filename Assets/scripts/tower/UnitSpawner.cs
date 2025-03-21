using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : Spawner<Unit>
{
    public event Action<Unit> Spawned;
    public event Action<Unit> Removed;

    private void Start()
    {
        _minPositionX = -3f;
        _maxPositionX = -1.5f;
        _minPositionZ = -3f;
        _maxPositionZ = 2f;
        _minRotationY = 0;
        _maxRotationY = 360;
        _poolCapacity = 3;
        _poolMaxSize = 3;       
    }

    protected override void OnGet(Unit @object)
    {
        @object.transform.position = new Vector3
            (Random.Range(_minPositionX, _maxPositionX), @object.transform.position.y, Random.Range(_minPositionZ, _maxPositionZ));

        @object.transform.rotation = Quaternion.Euler
            (@object.transform.rotation.x, Random.Range(_minRotationY, _maxRotationY), @object.transform.rotation.z);

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