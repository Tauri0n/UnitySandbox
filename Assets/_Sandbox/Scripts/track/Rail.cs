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
        List<Vector3> vertices = new();

        foreach (Vector3 position in positions)
        {
            vertices.Add(position);
            vertices.Add(position);
        }

        startProfile.Vertices = new List<Vector3>(vertices);
        endProfile.Vertices = new List<Vector3>(vertices);

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
        List<Vector3> vertices = new();

        foreach (Vector3 position in positions)
        {
            vertices.Add(position);
            vertices.Add(position);
        }

        for (int i = 0; i < resolution; i++)
        {

            Profile instance = Instantiate(this.profile, transform.position, transform.rotation);
            instance.Rail = this;
            instance.transform.parent = transform;
            instance.Vertices = new List<Vector3>(vertices);
            profiles.Add(instance);
        }

        profiles.Add(endProfile);
    }


    public void BuildMesh()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new();
        foreach (Profile profile in profiles)
        {
            List<Vector3> profileVertices = profile.Vertices;
            vertices.AddRange(profileVertices);
        }

        mesh.vertices = vertices.ToArray();


        List<int> triangles = new();
        List<Vector3> normals = new();
        for (int i = 0; i < profiles.Count - 1; i++)
        {
            AddTrianglesBetweenProfiles(triangles, normals, positionCount * 2, i);
        }
        
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();

        meshFilter.mesh = mesh;
    }


    private void AddTrianglesBetweenProfiles(List<int> triangles, List<Vector3> normals, int count, int profileIndex)
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