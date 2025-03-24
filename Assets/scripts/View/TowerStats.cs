using TMPro;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    [SerializeField] private Tower _tower;
    [SerializeField] private TextMeshProUGUI _resourceCountView;

    private void Start()
    {
        ShowResourceCount();
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
        _resourceCountView.text = _tower.GetResourcesCount().ToString();
    }
}