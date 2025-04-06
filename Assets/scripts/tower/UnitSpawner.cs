using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;

    public event Action<Unit> Spawned;

    private float _minPositionX = -3f;
    private float _maxPositionX = -1.5f;
    private float _minPositionZ = -3f;
    private float _maxPositionZ = 2f;
    private float _minRotationY = 0;
    private float _maxRotationY = 360;

    public void Spawn()
    {
        Unit unit;

        unit = Instantiate(_unitPrefab,
            new Vector3
            (Random.Range(transform.position.x - _minPositionX, transform.position.x - _maxPositionX),
            _unitPrefab.transform.position.y,
            Random.Range(transform.position.z - _minPositionZ, transform.position.z + _maxPositionZ)),
            Quaternion.Euler(_unitPrefab.transform.rotation.x, Random.Range(_minRotationY, _maxRotationY), _unitPrefab.transform.rotation.z));

        Spawned?.Invoke(unit);
    }
}