using System;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    private float _time = 5f;
    private float _childPositionY = 1f;
    private float _childMoveTime = 1f;
    private float _rotateSpeed = 180f;
   
    private Resource _targetResource;

    public event Action<Unit> Died;
    public event Action<Unit> ResourceTaked;
    public event Action<Resource> TargetReceived;

    public event Action Runned;
    public event Action Looted;
    public event Action Stopped;

    public bool Busy { get; private set; } = false;
    public bool IsBuilder { get; private set; } = false;

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
        if (IsBuilder == false)
        {
            Busy = true;

            if (resource == null)
            {
                UnBusy();
            }
            else
            {
                _targetResource = resource;

                TargetReceived?.Invoke(resource);

                transform.DOMove(resource.transform.position, _time).SetEase(Ease.Linear);

                SetDirection(resource.transform.position);

                Runned?.Invoke();
            }                          
        }
    }

    public void MoveToTower(Tower tower)
    {
        transform.DOMove(tower.transform.position, _time).SetEase(Ease.Linear);

        SetDirection(tower.transform.position);

        Runned?.Invoke();
    }

    public void MoveToFlag(Flag flag)
    {
        Busy = true;

        transform.DOMove(flag.transform.position, _time).SetEase(Ease.Linear);

        SetDirection(flag.transform.position);

        Runned?.Invoke();
    }

    public void UnBusy()
    {
        Busy = false;

        Stopped?.Invoke();
    }

    public void SetUnitBuidler()
    {
        IsBuilder = true;
    }

    public void UnSetUnitBuilder()
    {
        IsBuilder = false;
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