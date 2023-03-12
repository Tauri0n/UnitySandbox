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

    private List<Vector3> _vertices;
    private List<Vector3> _normals;

    public List<Vector3> Vertices
    {
        get
        {
            List<Vector3> result = new();
            Transform tf = transform;
            foreach (Vector3 vertex in _vertices)
            {
                result.Add(tf.TransformPoint(vertex) - tf.parent.position);
            }
            return result;
        }
        set
        {
            _vertices = new List<Vector3>();
            _normals = new List<Vector3>();
            Vector3 direction = value[value.Count - 1] - value[0];
            Vector3 normal = new Vector3(direction.y, direction.x, 0).normalized;
            for (int i = 0; i < value.Count; i++)
            {
                _vertices.Add(value[i]);
                _vertices.Add(value[i]);
                _normals.Add(normal);
                direction = value[i == 0 ? ^1 : i - 1] - value[i];
                normal = new Vector3(direction.y, direction.x, 0).normalized;
                _normals.Add(normal);
            }
        }
    }

    public List<Vector3> Normals
    {
        get
        {
            List<Vector3> result = new();
            foreach (Vector3 vertex in _normals)
            {
                result.Add(vertex);
            }
            return result;
        }
    }

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