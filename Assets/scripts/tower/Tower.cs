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
    private List<Resource> _lootableResources = new List<Resource>();

    private int _startUnitCount = 3;
    private float _scannerDelay = 1f;
    private float _unitDelay = 1.5f;
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

            _lootableResources.Remove(resource);

            ResourceCountChanged?.Invoke();

            if (resource.transform.parent != null && resource.transform.parent.TryGetComponent(out Unit unit))
            {
                unit.UnBusy();
            }

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
        unit.MoveToTower(transform.position);
    }

    private void LootResources()
    {       
        foreach (Resource resource in _lootableResources)
        {
            if (resource.Taked)
                continue;

            foreach (Unit unit in _units)
            {
                if (unit.Busy == false)
                {
                    unit.ResourceTaked += ReturnToTower;

                    unit.MoveToResource(resource);

                    resource.Picked();

                    break;
                }
            }
        }      
    }

    private IEnumerator StartScanning()
    {
        while(enabled)
        {
            yield return new WaitForSeconds(_scannerDelay);

            _lootableResources.AddRange(_scanner.Scan());
        }
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