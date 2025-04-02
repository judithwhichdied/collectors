using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Flag : MonoBehaviour
{
    private MeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColor(Color color)
    {
        _renderer.material.color = color;
    }
}
