using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    private UnitMover _mover;

    private Resource _takedResource;

    private float _childPositionY = 1f;
    private float _childMoveTime = 1f;
  
    public event Action<Unit> ResourceTaked;
    public event Action<Unit, Resource> InTower;
    public event Action OnFlagPosition;

    public event Action Runned;
    public event Action Looted;
    public event Action Stopped;

    public bool Busy { get; private set; } = false;
    public bool IsBuilder { get; private set; } = false;

    private void Awake()
    {
        _mover = GetComponent<UnitMover>();
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
                _mover.StartMoving(resource.transform, Take);

                Runned?.Invoke();
            }                          
        }

        void Take()
        {
            TakeResource(resource);
        }
    }
 
    public void MoveToTower(Tower tower)
    {
        _mover.StartMoving(tower.transform, InBase);

        Runned?.Invoke();
    }

    public void MoveToFlag(Flag flag)
    {
        Busy = true;

        _mover.StartMoving(flag.transform, OnFlag);

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

    private void OnFlag()
    {
        OnFlagPosition?.Invoke();
    }

    private void InBase()
    {
        InTower?.Invoke(this, _takedResource);
    }

    private void TakeResource(Resource resource)
    {
        resource.transform.SetParent(transform);

        resource.transform.DOMoveY(_childPositionY, _childMoveTime);

        _takedResource = resource;

        ResourceTaked?.Invoke(this);
        Looted?.Invoke();
    }    
}