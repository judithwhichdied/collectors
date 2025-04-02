using UnityEngine;
using System.Collections;
using System;

public class UnitMover : MonoBehaviour
{
    private float _speed = 2f;
    private float _rotateSpeed = 180f;

    public void StartMoving(Transform position, Action callback = null)
    {
        SetDirection(position.position);

        StartCoroutine(Move(position, callback));
    } 

    private IEnumerator Move(Transform target, Action callback = null)
    {
        while (target != null && transform.position.x != target.position.x && transform.position.z != target.position.z)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, _speed * Time.deltaTime);

            yield return null;
        }

        callback?.Invoke();
    }

    private void SetDirection(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        Quaternion toRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotateSpeed);
    }
}