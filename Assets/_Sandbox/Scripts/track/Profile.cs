using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Profile : MonoBehaviour
{
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    public Rail Rail { get; set; }

    private List<Vector3> vertices;

    public List<Vector3> Vertices
    {
        get
        {
            List<Vector3> result = new();
            foreach (Vector3 vertex in vertices)
            {
                result.Add(transform.TransformPoint(vertex) - transform.parent.position);
            }

            return result;
        }
        set { vertices = value; }
    }

    public List<Vector3> Normals { set; get; }

    private void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void Update()
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        if (position.Equals(lastPosition) && rotation.Equals(lastRotation)) return;
        lastPosition = position;
        lastRotation = rotation;
        Rail.BuildMesh();
    }
}