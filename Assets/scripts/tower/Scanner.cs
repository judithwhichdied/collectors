using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class Scanner : MonoBehaviour
{
    private WaitForSeconds _wait = new WaitForSeconds(1f);

    public event Action<List<Resource>> Scanned;

    private void Start()
    {
        StartCoroutine(StartScanning());
    }

    private List<Resource> Scan()
    {
        float extentX = 7.5f;

        Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(extentX, transform.position.y, extentX));

        List<Resource> resources = new List<Resource>();

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.attachedRigidbody.gameObject.TryGetComponent(out Resource resource))
            {
                resources.Add(resource);
            }
        }

        return resources;
    }

    private IEnumerator StartScanning()
    {
        List<Resource> resources;

        while (enabled)
        {
            yield return _wait;

            resources = Scan();

            Scanned?.Invoke(resources);
        }
    }
}