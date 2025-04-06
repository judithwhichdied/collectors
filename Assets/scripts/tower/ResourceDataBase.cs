using System.Collections.Generic;
using UnityEngine;

public class ResourceDataBase
{
    private List<Resource> _busyResources = new List<Resource>();
    private List<Resource> _freeResources = new List<Resource>();

    public bool TryGetResources()
    {
        return _freeResources.Count > 0;
    }

    public Resource GetFreeResource()
    {
        Resource resource = _freeResources[Random.Range(0, _freeResources.Count)];

        MakeResourceBusy(resource);

        return resource;
    }

    public void MakeResourceBusy(Resource resource)
    {
        _busyResources.Add(resource);
        _freeResources.Remove(resource);
    }

    public void RemoveResource(Resource resource)
    {       
        _busyResources.Remove(resource);
    }

    public void SortResources(List<Resource> allResources)
    {
        foreach (Resource resource in allResources)
        {
            if (_freeResources.Contains(resource) || _busyResources.Contains(resource))
            {
                continue;
            }

            _freeResources.Add(resource);
        }
    }
}