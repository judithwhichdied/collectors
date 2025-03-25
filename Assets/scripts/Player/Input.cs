using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
    private const string MouseClick = "MouseClick";

    [SerializeField] private PlayerInput _input;

    private InputAction _action;

    private Camera _camera;
    private Flag _flag;

    public event Action<Flag> FlagTaked;
    public event Action<Flag> FlagPlaced;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        _camera = Camera.main;

        _action = _input.actions.FindAction(MouseClick);
    }

    private void OnEnable()
    {
        _action.performed += ChooseTower;
    }

    private void OnDisable()
    {
        _action.performed -= ChooseTower;
    }

    private void ChooseTower(InputAction.CallbackContext context)
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, out RaycastHit hit) && hit.collider)
        {
            if (hit.collider.gameObject.TryGetComponent(out Tower tower))
            {
                if (tower.FlagCreated == false)
                {
                    _flag = tower.CreateFlag();
                }

                if (_flag != null)
                {
                    _flag.ChangeColorToRed();

                    FlagTaked?.Invoke(_flag);
                }
            }
            
            if (hit.collider.gameObject.TryGetComponent<Ground>(out _))
            {
                if (_flag != null)
                {
                    _flag.transform.position = hit.point;
                    _flag.ChangeColorToBlack();

                    FlagPlaced?.Invoke(_flag);
                }  
            }
        }      
    }
}