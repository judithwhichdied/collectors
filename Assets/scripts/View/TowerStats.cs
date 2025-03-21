using TMPro;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    [SerializeField] private Tower _tower;
    [SerializeField] private TextMeshProUGUI _tmp;

    private void Start()
    {
        _tmp.text = _tower.GetResourcesCount().ToString();
    }

    private void OnEnable()
    {
        _tower.ResourceCountChanged += ShowResourceCount;
    }

    private void OnDisable()
    {
        _tower.ResourceCountChanged -= ShowResourceCount;
    }

    private void ShowResourceCount()
    {
        _tmp.text = _tower.GetResourcesCount().ToString();
    }
}