using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Rail : MonoBehaviour
{
    private int lastResolution = 1;
    [SerializeField] private int resolution = 1;
    [SerializeField] private GameObject start;
    [SerializeField] private Profile profile;
    [SerializeField] private GameObject end;

    private Profile startProfile;
    private Profile endProfile;
    private List<Profile> profiles = new List<Profile>();

    [SerializeField] private MeshFilter meshFilter;

    private LineRenderer lineRenderer;
    private int positionCount;

    // Start is called before the first frame update
    void Start()
    {
        startProfile = start.GetComponent<Profile>();
        startProfile.Rail = this;
        endProfile = end.GetComponent<Profile>();
        endProfile.Rail = this;
        lineRenderer = GetComponent<LineRenderer>();
        positionCount = lineRenderer.positionCount;
        Vector3[] positions = new Vector3[positionCount];
        lineRenderer.GetPositions(positions);

        startProfile.Vertices = new List<Vector3>(positions);
        endProfile.Vertices = new List<Vector3>(positions);

        BuildProfiles();
        BuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastResolution != resolution)
        {
            if (resolution < 1) resolution = 1;
            if (resolution > 100) resolution = 100;
            BuildProfiles();
            BuildMesh();
            lastResolution = resolution;
        }
    }

    private void BuildProfiles()
    {
        profiles.Clear();
        for (int i = transform.childCount - 1; i >= 3; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        profiles.Add(startProfile);

        Vector3[] positions = new Vector3[positionCount];
        lineRenderer.GetPositions(positions);


        for (int i = 0; i < resolution; i++)
        {
            Profile instance = Instantiate(profile, transform.position, transform.rotation);
            instance.Rail = this;
            instance.transform.parent = transform;
            instance.Vertices = new List<Vector3>(positions);
            profiles.Add(instance);
        }

        profiles.Add(endProfile);
    }


    public void BuildMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new();
        List<Vector3> normals = new();
        foreach (Profile part in profiles)
        {
            vertices.AddRange(part.Vertices);
            normals.AddRange(part.Normals);
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();

        List<int> triangles = new();
        for (int i = 0; i < profiles.Count - 1; i++)
        {
            AddTrianglesBetweenProfiles(triangles, positionCount * 2, i);
        }
        
        mesh.triangles = triangles.ToArray();
        

        meshFilter.mesh = mesh;
    }


    private void AddTrianglesBetweenProfiles(List<int> triangles, int count, int profileIndex)
    {
        for (int vertexIndex = 0; vertexIndex < count; vertexIndex++) //Iteration durch die Dreiecke
        {
            int currentIndex = vertexIndex + profileIndex * count;
            if (vertexIndex % 2 == 0) //bei den rechten Dreiecken
            {
                triangles.Add(currentIndex);
                triangles.Add(vertexIndex == 0 ? profileIndex * count + count * 2 - 1 : currentIndex + count - 1);
                triangles.Add(currentIndex + count);
                continue;
            }

            // bei den linken Dreiecken
            triangles.Add(currentIndex);
            triangles.Add(currentIndex + count);
            triangles.Add(vertexIndex == count - 1 ? profileIndex * count : currentIndex + 1);
        }
    }
}