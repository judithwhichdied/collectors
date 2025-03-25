using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDataBase : MonoBehaviour
{
    [SerializeField] private Scanner _scanner;

    private float _scannerDelay = 1f;

    private List<Resource> _allResources = new List<Resource>();
    private List<Resource> _busyResources = new List<Resource>();
    private List<Resource> _freeResources = new List<Resource>();

    private void Start()
    {
        StartCoroutine(StartScanning());
    }

    public List<Resource> GetFreeResources()
    {
        return new List<Resource>(_freeResources);
    }

    public void MakeResourceBusy(Resource resource)
    {
        _busyResources.Add(resource);
        _freeResources.Remove(resource);
    }

    public void MakeResourceFree(Resource resource)
    {
        _busyResources.Remove(resource);
        _freeResources.Add(resource);
    }

    private void SortResources()
    {
        foreach (Resource resource in _allResources)
        {
            if (resource.Taked)
            {
                if (_busyResources.Contains(resource))
                {
                    continue;
                }
                else
                {
                    _busyResources.Add(resource);
                }
            }
            else
            {
                if (_freeResources.Contains(resource))
                {
                    continue;
                }
                else
                {
                    _freeResources.Add(resource);
                }
            }
        }
    }

    private IEnumerator StartScanning()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(_scannerDelay);

            _allResources.AddRange(_scanner.Scan());

            SortResources();
        }
    }
}