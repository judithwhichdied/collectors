using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Flag : MonoBehaviour
{
    private MeshRenderer _renderer;

    private Color _redColor = new Color(1, 0, 0, 0.5f);

    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void ChangeColorToRed()
    {
        _renderer.material.color = new Color(1, 0, 0, 0.5f);
    }

    public void ChangeColorToBlack()
    {
        _renderer.material.color = Color.black;
    }
}
