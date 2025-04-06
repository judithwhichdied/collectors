using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private UnitSpawner _spawner;

    [SerializeField] private Flag _flagPrefab;
    [SerializeField] private FlagPlacer _placer;
    [SerializeField] private TowerBuilder _builder;
    [SerializeField] private Scanner _scanner;

    private ResourceDataBase _database;

    private List<Resource> _resources = new List<Resource>();
    private List<Unit> _units = new List<Unit>();

    private int _unitCost = 3;
    private int _startUnitCount = 3;

    private WaitForSeconds _wait = new WaitForSeconds(1.5f); 

    private bool _priorityChanged = false;

    public event Action ResourceCountChanged;

    public bool FlagCreated { get; private set; } = false;

    public int ResourcesCount => _resources.Count;

    private void Start()
    {
        for (int i = 0; i < _startUnitCount; i++)
            _spawner.Spawn();

        StartCoroutine(StartLooting());
    }

    private void OnEnable()
    {
        _spawner.Spawned += AddUnit;
        _placer.FlagPlaced += BuildTower;
    }

    private void OnDisable()
    {
        _spawner.Spawned -= AddUnit;
        _placer.FlagPlaced -= BuildTower;
    }

    private void OnDestroy()
    {
        _scanner.Scanned -= _database.SortResources;
    }

    public void Initialize(ResourceDataBase database, int startUnitCount)
    {
        _database = database;
        _startUnitCount = startUnitCount;
        _scanner.Scanned += _database.SortResources;
    }

    public void ChangeUnitCount(int newUnitCount = 0)
    {
        _startUnitCount = newUnitCount;
    }

    public Flag CreateFlag()
    {
        Flag flag;

        flag = Instantiate(_flagPrefab, transform.position, _flagPrefab.transform.rotation);

        FlagCreated = true;

        return flag;
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    private void BuildTower(Flag flag)
    {
        if (_units.Count <= 1)
        {
            return;
        }

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

    private void RemoveUnit(Unit unit)
    {
        _units.Remove(unit);
    }

    private void ReturnToTower(Unit unit)
    {
        unit.MoveToTower(transform);

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

    private IEnumerator StartLooting()
    {
        while (enabled)
        {
            yield return _wait;

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

        unit.MoveToFlag(flag.transform);
        unit.OnFlagPosition += Build;
        
        SendResources(buildingCost);

        RemoveUnit(unit);
       
        _priorityChanged = false;

        void Build()
        {
            _builder.BuildTower(unit, flag);
            unit.OnFlagPosition -= Build;
        };
    }
}