using System.Collections;
using UnityEngine;

public class ResourceSpawner : Spawner<Resource>
{
    private float _delay = 1f;

    private float _rotationX = -120;
  
    private void Start()
    {
        MinPositionX = -6.6f;
        MaxPositionX = 6.6f;
        MinPositionZ = -6.6f;
        MaxPositionZ = 6.6f;
        MinRotationY = -208;
        MaxRotationY = -73;

        StartCoroutine(SpawnDelaying());
    }

    protected override void OnGet(Resource @object)
    {
        @object.transform.position = new Vector3
            (Random.Range(MinPositionX, MaxPositionX), @object.transform.position.y, Random.Range(MinPositionZ, MaxPositionZ));

        @object.transform.rotation = Quaternion.Euler
            (_rotationX, Random.Range(MinRotationY, MaxRotationY), @object.transform.rotation.z);

        @object.gameObject.SetActive(true);

        @object.Collected += ReleaseObject;
        @object.IncorrectSpawned += ReleaseObject;
    }

    protected override void OnRelease(Resource @object)
    {
        @object.gameObject.SetActive(false);

        @object.Collected -= ReleaseObject;
        @object.IncorrectSpawned -= ReleaseObject;
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