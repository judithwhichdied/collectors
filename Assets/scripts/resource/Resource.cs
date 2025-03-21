using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private float _startPositionY = 0.1f;

    public event Action<Resource> Collected;

    public event Action IncorrectSpawned;

    public bool Taked { get; private set; } = false;

    private void OnEnable()
    {
        Taked = false;

        transform.position = new Vector3(transform.position.x, _startPositionY, transform.position.z);
    }

    public void Picked()
    {
        Taked = true;
    }

    public void UnPicked()
    {
        Taked = false;
    }

    public void Looted()
    {
        Collected?.Invoke(this);
    }
}