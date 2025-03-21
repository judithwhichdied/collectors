using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private UnitSpawner _spawner;
    [SerializeField] private Scanner _scanner;

    private List<Resource> _resources = new List<Resource>();
    private List<Unit> _units = new List<Unit>();
    private List<Rigidbody> _lootableResources = new List<Rigidbody>();

    private int _startUnitCount = 3;
    private float _scannerDelay = 1f;
    private float _unitDelay = 3f;
    private float _pointPositionZ = 1.5f;

    private Vector3 _towerPosition;

    public event Action ResourceCountChanged;

    private void Start()
    {
        _towerPosition = new Vector3(transform.position.x, transform.position.y, (transform.position.z - _pointPositionZ));

        for (int i = 0; i < _startUnitCount; i++)
            _spawner.Spawn();

        StartCoroutine(StartScanning());
        StartCoroutine(StartLooting());
    }

    private void OnEnable()
    {
        _spawner.Spawned += _units.Add;
        _spawner.Removed += RemoveUnit;
    }

    private void OnDisable()
    {
        _spawner.Spawned -= _units.Add;
        _spawner.Removed -= RemoveUnit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resource))
        {
            _resources.Add(resource);

            ResourceCountChanged?.Invoke();

            resource.transform.SetParent(null);

            resource.Looted();
        }
    }

    public int GetResourcesCount()
    {
        return _resources.Count;
    }

    private void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }

    private void ReturnToTower(Unit unit)
    {
        unit.MoveToPoint(_towerPosition);

        StartCoroutine(DelayUnBusy(unit));
    }

    private void LootResources()
    {       
        foreach (Rigidbody resource in _lootableResources)
        {
            if (resource.GetComponent<Resource>().Taked)
                continue;

            foreach (Unit unit in _units)
            {
                if (unit.Busy == false)
                {
                    unit.ResourceTaked += ReturnToTower;

                    unit.MoveToPoint(resource.transform.position);

                    resource.GetComponent<Resource>().Picked();

                    break;
                }
                else
                {
                    continue;
                }
            }
        }      
    }

    private IEnumerator StartScanning()
    {
        while(enabled)
        {
            yield return new WaitForSeconds(_scannerDelay);

            _lootableResources = _scanner.Scan();
        }
    }

    private IEnumerator DelayUnBusy(Unit unit)
    {
        while (unit.transform.position != _towerPosition)
        {
            yield return null;
        }

        unit.UnBusy();
    }

    private IEnumerator StartLooting()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(_unitDelay);

            LootResources();
        }
    }
}