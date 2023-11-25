using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    [SerializeField] private List<Vector3> vertices = new List<Vector3>();
    public struct MyBounds
    {
        private Vector3 center;
        private Vector3 extents;
        private Vector3 minimum;
        private Vector3 maximum;
       
        public Vector3 center
        {
            get
            {
                return center;
            }

            set
            {
                center = value;
            }
        }

        public Vector3 size
        {
            get
            {
                return extents * 2f;
            }

            set
            {
                extents = value * 0.5f;
            }
        }

        public MyBounds(Vector3 center, Vector3 size, Vector3 minimum, Vector3 maximum)
        {
            center = center;
            extents = size * 0.5f;
            this.minimum = minimum;
            this.maximum = maximum;
        }

        public Vector3 GetMin()
        {
            return minimum;
        }

        public Vector3 GetMax()
        {
            return maximum;
        }
    }

    private MyBounds boundingBox;

    private void Start()
    {
        SetVertices();
    }

    private void Update()
    {
        SetVertices();
    }

    void SetVertices()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            meshFilter.mesh.GetVertices(vertices);

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = transform.TransformPoint(vertices[i]);
            }

            boundingBox = CalculateBoundingBox(vertices);
        }
    }

    public List<Vector3> GetVertices()
    {
        return vertices;
    }

    MyBounds CalculateBoundingBox(List<Vector3> vertexList)
    {
        if (vertexList.Count == 0)
        {
            return new MyBounds(transform.position, Vector3.zero, Vector3.zero, Vector3.zero);
        }

        Vector3 min = vertexList[0];
        Vector3 max = vertexList[0];

        for (int i = 1; i < vertexList.Count; i++)
        {
            min = Vector3.Min(min, vertexList[i]);
            max = Vector3.Max(max, vertexList[i]);
        }

        Vector3 size = max - min;
        Vector3 center = (max + min) / 2;

        return new MyBounds(center, size, min, max);
    }

    public Vector3[] GetBoundsVertices()
    {
        Vector3[] vertices = new Vector3[8];
        vertices[0] = boundingBox.GetMin();
        vertices[1] = boundingBox.GetMax();
        vertices[2] = new Vector3(boundingBox.GetMin().x, boundingBox.GetMax().y, boundingBox.GetMax().z);
        vertices[3] = new Vector3(boundingBox.GetMax().x, boundingBox.GetMin().y, boundingBox.GetMax().z);
        vertices[4] = new Vector3(boundingBox.GetMin().x, boundingBox.GetMin().y, boundingBox.GetMax().z);
        vertices[5] = new Vector3(boundingBox.GetMin().x, boundingBox.GetMax().y, boundingBox.GetMin().z);
        vertices[6] = new Vector3(boundingBox.GetMax().x, boundingBox.GetMin().y, boundingBox.GetMin().z);
        vertices[7] = new Vector3(boundingBox.GetMax().x, boundingBox.GetMax().y, boundingBox.GetMin().z);

        return vertices;
    }

    void DrawBoundingBox()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boundingBox.center, boundingBox.size);

        for (int i = 0; i < 8; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(GetBoundsVertices()[i], new Vector3(0.2f, 0.2f, 0.2f));
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            DrawBoundingBox();
        }
    }
}