using System;
using UnityEngine;
using DG.Tweening;
using UnityEditor.iOS;

public class Unit : MonoBehaviour
{
    private float _time = 5f;
    private float _childPositionY = 1f;
    private float _childMoveTime = 1f;
    private float _rotateSpeed = 180f;

    private Resource _targetResource;

    public event Action<Unit> Died;
    public event Action<Unit> ResourceTaked;

    public event Action Runned;
    public event Action Looted;

    public bool Busy { get; private set; } = false;

    private void Start()
    {
        Debug.Log(Busy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resource) && resource == _targetResource)
        {
            if (resource.transform.parent == null)
                TakeResource(resource);
        }
    }

    public void MoveToResource(Resource resource)
    {
        Busy = true;

        _targetResource = resource;

        transform.DOMove(resource.transform.position, _time);

        SetDirection(resource.transform.position);

        Runned?.Invoke();
    }

    public void MoveToTower(Vector3 towerPosition)
    {
        transform.DOMove(towerPosition, _time);

        SetDirection(towerPosition);

        Runned?.Invoke();
    }

    public void UnBusy()
    {
        Busy = false;
    }

    private void SetDirection(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotateSpeed);
    }

    private void TakeResource(Resource resource)
    {
        resource.transform.SetParent(transform);

        resource.transform.DOMoveY(_childPositionY, _childMoveTime);

        ResourceTaked?.Invoke(this);
        Looted?.Invoke();
    }
}