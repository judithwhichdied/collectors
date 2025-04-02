using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{
    private const string MouseClick = "MouseClick";

    [SerializeField] private PlayerInput _input;

    private InputAction _action;

    public event Action Clicked;

    private void Awake()
    {
        _action = _input.actions.FindAction(MouseClick);
    }

    private void OnEnable()
    {
        _action.performed += Click;
    }

    private void OnDisable()
    {
        _action.performed -= Click;
    }

    private void Click(InputAction.CallbackContext context)
    {
        Clicked?.Invoke();
    }
}