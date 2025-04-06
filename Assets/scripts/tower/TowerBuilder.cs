using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] private Tower _towerPrefab;
    [SerializeField] private Tower _startedTower;

    private ResourceDataBase _database = new();

    private int _towerRotationY = 180;

    private int _startUnitCount = 3;

    private void Awake()
    {
        _startedTower.Initialize(_database, _startUnitCount);
    }

    public void BuildTower(Unit unit, Flag flag)
    {
        Tower newTower = Instantiate(_towerPrefab, new Vector3(flag.transform.position.x, 0, flag.transform.position.z), Quaternion.Euler(0, _towerRotationY, 0));

        newTower.Initialize(_database, 0);

        Destroy(flag.gameObject);

        newTower.AddUnit(unit);

        unit.UnBusy();
        unit.UnSetUnitBuilder();        
    }
}