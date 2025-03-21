using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{   
    public List<Rigidbody> Scan()
    {
        float extentX = 7.5f;

        Collider[] hits = Physics.OverlapBox(transform.position, new Vector3(extentX, transform.position.y, extentX));

        List<Rigidbody> resources = new List<Rigidbody>();

        foreach (Collider hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.attachedRigidbody.gameObject.TryGetComponent<Resource>(out _) && hit.attachedRigidbody.gameObject.GetComponent<Resource>().Taked == false)
            {
                resources.Add(hit.attachedRigidbody);
            }
        }

        return resources;
    }
}