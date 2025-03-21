using System.Collections;
using UnityEngine;

public class ResourceSpawner : Spawner<Resource>
{
    private float _delay = 2f;

    private float _rotationX = -120;
  
    private void Start()
    {
        _minPositionX = -6.6f;
        _maxPositionX = 6.6f;
        _minPositionZ = -6.6f;
        _maxPositionZ = 6.6f;
        _minRotationY = -208;
        _maxRotationY = -73;

        StartCoroutine(SpawnDelaying());
    }

    protected override void OnGet(Resource @object)
    {
        @object.transform.position = new Vector3
            (Random.Range(_minPositionX, _maxPositionX), @object.transform.position.y, Random.Range(_minPositionZ, _maxPositionZ));

        @object.transform.rotation = Quaternion.Euler
            (_rotationX, Random.Range(_minRotationY, _maxRotationY), @object.transform.rotation.z);

        @object.gameObject.SetActive(true);

        @object.Collected += ReleaseObject;
    }

    protected override void OnRelease(Resource @object)
    {
        @object.gameObject.SetActive(false);

        @object.Collected -= ReleaseObject;
    }

    private IEnumerator SpawnDelaying()
    {
        while(enabled)
        {
            Spawn();

            yield return new WaitForSeconds(_delay);
        }
    }
}