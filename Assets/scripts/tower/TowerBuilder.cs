using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [SerializeField] private Tower _towerPrefab;

    private int _towerRotationY = 180;

    public void BuildTower(Unit unit, Flag flag)
    {
        Tower newTower = Instantiate(_towerPrefab, new Vector3(flag.transform.position.x, 0, flag.transform.position.z), Quaternion.Euler(0, _towerRotationY, 0));

        newTower.StartUnitCount = 0;

        Destroy(flag.gameObject);

        newTower.AddUnit(unit);

        unit.UnBusy();
        unit.UnSetUnitBuilder();        
    }
}