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
    [SerializeField] private FlagPlacer _placer;
    [SerializeField] private TowerBuilder _builder;

    private List<Resource> _resources = new List<Resource>();
    private List<Unit> _units = new List<Unit>();

    public int StartUnitCount = 3;

    private int _unitCost = 3;
    private float _unitDelay = 1.5f;

    private bool _priorityChanged = false;

    public event Action ResourceCountChanged;

    public event Action<List<Resource>> Scanned;

    public bool FlagCreated { get; private set; } = false;

    public int ResourcesCount => _resources.Count;

    private void Start()
    {
        for (int i = 0; i < StartUnitCount; i++)
            _spawner.Spawn();

        StartCoroutine(StartScanning());
        StartCoroutine(StartLooting());
    }

    private void OnEnable()
    {
        _spawner.Spawned += AddUnit;
        _spawner.Removed += RemoveUnit;
        _placer.FlagPlaced += BuildNewTower;
    }

    private void OnDisable()
    {
        _spawner.Spawned -= AddUnit;
        _spawner.Removed -= RemoveUnit;
        _placer.FlagPlaced -= BuildNewTower;
    }

    public Flag CreateFlag()
    {
        Flag flag;

        Quaternion flagRotation = Quaternion.Euler(-90, 200, 0);

        flag = Instantiate(_flagPrefab, transform.position, flagRotation);

        FlagCreated = true;

        return flag;
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
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

    private void ReturnToTower(Unit unit)
    {
        unit.MoveToTower(this);

        unit.InTower += CollectResource;
        unit.ResourceTaked -= ReturnToTower;
    }

    private void SpawnNewUnit()
    {
        _spawner.Spawn();
    }

    private void CollectResource(Unit unit, Resource resource)
    {
        _resources.Add(resource);

        ResourceCountChanged?.Invoke();

        resource.transform.SetParent(null);

        resource.Looted();

        _database.RemoveResource(resource);

        unit.UnBusy();
        unit.InTower -= CollectResource;

        if (_resources.Count >= _unitCost && _priorityChanged == false)
        {
            SpawnNewUnit();
            SendResources(_unitCost);
        }
    }

    private void SendResources(int cost)
    {
        _resources.RemoveRange(0, cost);

        ResourceCountChanged?.Invoke();
    }
   
    private void LootResources()
    {
        if (_database.TryGetResources())
        {
            Unit unit = TryGetUnit();

            if (unit != null)
            {
                unit.ResourceTaked += ReturnToTower;

                unit.MoveToResource(_database.GetFreeResource());
            }
        }
    }

    private Unit TryGetUnit()
    {
        foreach(Unit unit in _units)
        {
            if (unit.Busy == false && unit.IsBuilder == false)
                return unit;
        }

        return null;
    }

    private IEnumerator StartScanning()
    {
        WaitForSeconds wait = new WaitForSeconds(1);

        List<Resource> resources = new List<Resource>();

        while (enabled)
        {
            yield return wait;

            resources = _scanner.Scan();

            Scanned?.Invoke(resources);
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
        unit.OnFlagPosition += Build;
        
        SendResources(buildingCost);

        RemoveUnit(unit);
       
        _priorityChanged = false;

        void Build()
        {
            _builder.BuildTower(unit, flag);
        };
    }
}