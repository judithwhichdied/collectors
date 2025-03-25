using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private UnitSpawner _spawner;
    [SerializeField] private Scanner _scanner;
    [SerializeField] private ResourceDataBase _database;
    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private Input _input;
    [SerializeField] private Tower _towerPrefab;

    private List<Resource> _resources = new List<Resource>();
    private List<Unit> _units = new List<Unit>();

    private int _startUnitCount = 3;
    private int _unitCost = 3;
    private float _unitDelay = 1.5f;

    private bool _priorityChanged = false;

    private int _towerRotationY = 180;

    public event Action ResourceCountChanged;

    public bool FlagCreated { get; private set; } = false;

    private void Start()
    {
        for (int i = 0; i < _startUnitCount; i++)
            _spawner.Spawn();

        StartCoroutine(StartLooting());
    }

    private void OnEnable()
    {
        _spawner.Spawned += AddUnit;
        _spawner.Removed += RemoveUnit;
        _input.FlagPlaced += BuildNewTower;
    }

    private void OnDisable()
    {
        _spawner.Spawned -= AddUnit;
        _spawner.Removed -= RemoveUnit;
        _input.FlagPlaced -= BuildNewTower;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resource))
        {           
            if (resource.transform.parent != null && resource.transform.parent.TryGetComponent(out Unit unit) && TryFindUnit(unit))
            {
                _resources.Add(resource);

                ResourceCountChanged?.Invoke();

                resource.transform.SetParent(null);

                resource.Looted();

                _database.MakeResourceFree(resource);

                unit.UnBusy();

                if (_resources.Count >= 3 && _priorityChanged == false)
                {
                    SpawnNewUnit();
                    SendResources(_unitCost);
                }                
            }           
        }
    }

    public Flag CreateFlag()
    {
        Flag flag;

        Quaternion flagRotation = Quaternion.Euler(-90, 200, 0);

        flag = Instantiate(_flagPrefab, transform.position, flagRotation);

        FlagCreated = true;

        return flag;
    }

    public int GetResourcesCount()
    {
        return _resources.Count;
    }

    private void BuildNewTower(Flag flag)
    {
        if (_units.Count > 1)
        {
            _priorityChanged = true;

            foreach (Unit unit in _units)
            {
                if (unit.Busy)
                {
                    StartCoroutine(StartBuilding(unit, flag));

                    break;
                }
            }
        }
    }

    private void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }

    private void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    private void ReturnToTower(Unit unit)
    {
        unit.MoveToTower(this);
    }

    private void SpawnNewUnit()
    {
        _spawner.Spawn();
    }

    private void SendResources(int cost)
    {
        _resources.RemoveRange(0, cost);

        ResourceCountChanged?.Invoke();
    }

    private bool TryFindUnit(Unit unit)
    {
        foreach (Unit minion in _units)
        {
            if (minion == unit)
            {
                return true;
            }           
        }

        return false;
    }

    private void LootResources()
    {
        foreach (Resource resource in _database.GetFreeResources())
        {
            if (resource.Taked)
                continue;

            foreach (Unit unit in _units)
            {
                if (unit.Busy == false && unit.IsBuilder == false)
                {
                    unit.ResourceTaked += ReturnToTower;

                    unit.MoveToResource(resource);

                    unit.TargetReceived += _database.MakeResourceBusy;

                    resource.Picked();

                    break;
                }
            }
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

    private IEnumerator StartBuilding(Unit unit, Flag flag)
    {
        int buildingCost = 5;

        while (_resources.Count < buildingCost)
            yield return null;

        unit.SetUnitBuidler();

        while (unit.Busy)
        {
            yield return null;
        }

        unit.MoveToFlag(flag);

        SendResources(buildingCost);

        while (unit.transform.position != flag.transform.position)
        {
            if (flag == null)
                break;

            yield return null;
        }

        Tower newTower = Instantiate(_towerPrefab, new Vector3(flag.transform.position.x, 0, flag.transform.position.z), Quaternion.Euler(0, _towerRotationY, 0));

        Destroy(flag.gameObject);

        RemoveUnit(unit);

        newTower.AddUnit(unit);

        unit.UnBusy();
        unit.UnSetUnitBuilder();

        _priorityChanged = false;
    }
}