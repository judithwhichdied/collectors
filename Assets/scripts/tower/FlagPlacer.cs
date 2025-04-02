using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private Input _input;
    [SerializeField] private Camera _camera;

    private Flag _flag;

    public event Action<Flag> FlagTaked;
    public event Action<Flag> FlagPlaced;

    private void OnEnable()
    {
        _input.Clicked += OnClick;
    }

    private void OnDisable()
    {
        _input.Clicked -= OnClick;
    }

    private void OnClick()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, out RaycastHit hit) && hit.collider)
        {
            if (hit.collider.TryGetComponent(out Tower tower))
            {
                if (tower.FlagCreated == false)
                {
                    _flag = tower.CreateFlag();
                }

                if (_flag != null)
                {
                    _flag.ChangeColor(Color.red);

                    FlagTaked?.Invoke(_flag);
                }
            }

            if (hit.collider.TryGetComponent<Ground>(out _))
            {
                if (_flag != null)
                {
                    _flag.transform.position = hit.point;
                    _flag.ChangeColor(Color.black);

                    FlagPlaced?.Invoke(_flag);
                }
            }
        }
    }
}