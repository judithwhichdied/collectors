using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private float _startPositionY = 0.1f;

    public event Action<Resource> Collected;

    private void OnEnable()
    {
        transform.position = new Vector3(transform.position.x, _startPositionY, transform.position.z);
    }

    public void Looted()
    {
        Collected?.Invoke(this);
    }
}