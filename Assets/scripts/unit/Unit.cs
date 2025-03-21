using System;
using UnityEngine;
using DG.Tweening;

public class Unit : MonoBehaviour
{
    private float _time = 5f;
    private float _childPositionY = 1f;
    private float _childMoveTime = 1f;
    private float _rotateSpeed = 180f;

    public bool Busy { get; private set; } = false;

    public event Action<Unit> Died;
    public event Action<Unit> ResourceTaked;

    public event Action Runned;
    public event Action Stopped;
    public event Action Looted;

    private void Start()
    {
        Debug.Log(Busy);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Resource resource))
        {
            if (resource.transform.parent == null)
                TakeResource(resource);
        }
    }

    public void MoveToPoint(Vector3 position)
    {
        Busy = true;

        transform.DOMove(position, _time);

        SetDirection(position);

        Runned?.Invoke();
    }

    public void UnBusy()
    {
        Busy = false;

        Stopped?.Invoke();
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