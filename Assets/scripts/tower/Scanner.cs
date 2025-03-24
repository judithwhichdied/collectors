using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{   
    public List<Resource> Scan()
    {
        float extentX = 7.5f;

        Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(extentX, transform.position.y, extentX));

        List<Rigidbody> rigidbodies = new List<Rigidbody>();

        List<Resource> resources = new List<Resource>();

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.attachedRigidbody.gameObject.TryGetComponent(out Resource resource) && resource.Taked == false)
            {
                rigidbodies.Add(hit.attachedRigidbody);
            }
        }

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody.TryGetComponent(out Resource resource))
            {
                resources.Add(resource);
            }
        }

        return resources;
    }
}