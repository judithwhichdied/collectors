using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlagFollower : MonoBehaviour
{
    [SerializeField] private Input _input;
    [SerializeField] private Camera _camera;

    private bool _canFollow = true;

    private void OnEnable()
    {
        _input.FlagPlaced += Stop;
        _input.FlagTaked += Follow;
    }

    private void OnDisable()
    {
        _input.FlagPlaced -= Stop;
        _input.FlagTaked -= Follow;
    }

    private void Follow(Flag flag)
    {
        StartCoroutine(StartFollowing(flag));
    }

    private void Stop(Flag flag)
    {
        _canFollow = false;
    }

    private IEnumerator StartFollowing(Flag flag)
    {
        Ray ray;

        while (_canFollow)
        {
            ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray : ray, out RaycastHit hit) && hit.collider)
            {
                flag.transform.position = hit.point;
            }

            yield return null;
        }

        _canFollow = true;
    }
}