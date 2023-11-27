using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class FrustrumCreator : MonoBehaviour
{
    public class FrustumPlane
    {
        public Vector3 vertexA;
        public Vector3 vertexB;
        public Vector3 vertexC;

        public Vector3 normal;
    }

    public FrustumPlane nearPlane = new FrustumPlane();
    public FrustumPlane farPlane = new FrustumPlane();

    public FrustumPlane leftPlane = new FrustumPlane();
    public FrustumPlane rightPlane = new FrustumPlane();
    public FrustumPlane upPlane = new FrustumPlane();
    public FrustumPlane downPlane = new FrustumPlane();

    public float fov;
    private float vFov;

    public float farDist;
    public float nearDist;

    public int screenWidth;
    public int screenHeight;
    private float aspectRatio;

    public List<BoundingBox> boundinToCheck = new List<BoundingBox>();
    public List<FrustumPlane> planes = new List<FrustumPlane>();

    //El offset desde la posicion en la que esta
    private Vector3 nearCenter;
    private Vector3 farCenter;

    private Vector3 farUpRightV;
    private Vector3 farUpLeftV;
    private Vector3 farDownRightV;
    private Vector3 farDownLeftV;

    private Vector3 nearUpRightV;
    private Vector3 nearUpLeftV;
    private Vector3 nearDownRightV;
    private Vector3 nearDownLeftV;

    private List<Vector3> vertexList = new List<Vector3>();

    void Start()
    {
        AddVerticesToList();
        AddPlanesToList();
    }

    void Update()
    {
        UpdateVertex();
        UpdatePlanes();

        aspectRatio = (float)screenWidth / (float)screenHeight;

        vFov = fov / aspectRatio;

        nearCenter = transform.position + transform.forward * nearDist;
        farCenter = transform.position + transform.forward * farDist;

        //Calculo las posiciones de los v�rtices del near plane
        nearUpLeftV = new Vector3(Mathf.Tan((-fov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.x, Mathf.Tan((vFov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.y, nearCenter.z);
        nearUpRightV = new Vector3(Mathf.Tan((fov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.x, Mathf.Tan((vFov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.y, nearCenter.z);
        nearDownLeftV = new Vector3(Mathf.Tan((-fov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.x, Mathf.Tan((-vFov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.y, nearCenter.z);
        nearDownRightV = new Vector3(Mathf.Tan((fov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.x, Mathf.Tan((-vFov / 2) * Mathf.Deg2Rad) * nearDist + nearCenter.y, nearCenter.z);

        //Calculo las posiciones de los v�rtices del far plane
        farUpLeftV = new Vector3(Mathf.Tan((-fov / 2) * Mathf.Deg2Rad) * farDist + farCenter.x, Mathf.Tan((vFov / 2) * Mathf.Deg2Rad) * farDist + farCenter.y, farCenter.z);
        farUpRightV = new Vector3(Mathf.Tan((fov / 2) * Mathf.Deg2Rad) * farDist + farCenter.x, Mathf.Tan((vFov / 2) * Mathf.Deg2Rad) * farDist + farCenter.y, farCenter.z);
        farDownLeftV = new Vector3(Mathf.Tan((-fov / 2) * Mathf.Deg2Rad) * farDist + farCenter.x, Mathf.Tan((-vFov / 2) * Mathf.Deg2Rad) * farDist + farCenter.y, farCenter.z);
        farDownRightV = new Vector3(Mathf.Tan((fov / 2) * Mathf.Deg2Rad) * farDist + farCenter.x, Mathf.Tan((-vFov / 2) * Mathf.Deg2Rad) * farDist + farCenter.y, farCenter.z);

        CheckObsjectsInFrustum();
    }

    void AddVerticesToList()
    {
        //Triangulo superior
        vertexList.Add(transform.position);
        vertexList.Add(farUpRightV);
        vertexList.Add(farUpLeftV);

        //Triangulo derecho
        vertexList.Add(transform.position);
        vertexList.Add(farUpRightV);
        vertexList.Add(farDownRightV);

        //Triangulo inferior
        vertexList.Add(transform.position);
        vertexList.Add(farDownRightV);
        vertexList.Add(farDownLeftV);

        //Triangulo izquierdo
        vertexList.Add(transform.position);
        vertexList.Add(farUpLeftV);
        vertexList.Add(farDownLeftV);

        //Triangulo del far plane
        vertexList.Add(farUpRightV);
        vertexList.Add(farDownRightV);
        vertexList.Add(farDownLeftV);

        //Triangulo del near plane
        vertexList.Add(nearUpRightV);
        vertexList.Add(nearDownRightV);
        vertexList.Add(nearDownLeftV);
    }

    void UpdateVertex()
    {
        //Update triangulo superior
        vertexList[0] = nearUpRightV;
        vertexList[1] = farUpRightV;
        vertexList[2] = farUpLeftV;

        //Update triangulo derecho
        vertexList[3] = nearUpRightV;
        vertexList[4] = farUpRightV;
        vertexList[5] = farDownRightV;

        //Update triangulo inferior
        vertexList[6] = nearDownLeftV;
        vertexList[7] = farDownRightV;
        vertexList[8] = farDownLeftV;

        //Update triangulo izquierdo
        vertexList[9] = nearDownLeftV;
        vertexList[10] = farUpLeftV;
        vertexList[11] = farDownLeftV;

        //Update triangulo del far plane
        vertexList[12] = farUpRightV;
        vertexList[13] = farDownRightV;
        vertexList[14] = farDownLeftV;

        //Update triangulo del near plane
        vertexList[15] = nearUpRightV;
        vertexList[16] = nearDownRightV;
        vertexList[17] = nearDownLeftV;
    }

    void DrawFrustum()
    {
        Gizmos.DrawLine(nearUpRightV, farUpRightV);
        Gizmos.DrawLine(nearUpLeftV, farUpLeftV);
        Gizmos.DrawLine(farUpRightV, farUpLeftV);
        Gizmos.DrawLine(nearUpRightV, nearUpLeftV);

        Gizmos.DrawLine(nearDownRightV, farDownRightV);
        Gizmos.DrawLine(nearDownLeftV, farDownLeftV);
        Gizmos.DrawLine(farDownRightV, farDownLeftV);
        Gizmos.DrawLine(nearDownRightV, nearDownLeftV);

        Gizmos.DrawLine(nearDownRightV, nearUpRightV);
        Gizmos.DrawLine(nearDownLeftV, nearUpLeftV);
        Gizmos.DrawLine(farDownRightV, farUpRightV);
        Gizmos.DrawLine(farDownLeftV, farUpLeftV);
    }

    void AddPlanesToList()
    {
        planes.Add(upPlane);
        planes.Add(rightPlane);
        planes.Add(downPlane);
        planes.Add(leftPlane);

        planes.Add(farPlane);
        planes.Add(nearPlane);
    }

    void UpdatePlanes()
    {
        Vector3 point = transform.position + transform.forward * ((farDist - nearDist) / 2 + nearDist); //Punto en el centro de la figura

        for (int i = 0; i < planes.Count; i++)
        {
            planes[i].vertexA = vertexList[i * 3 + 0];
            planes[i].vertexB = vertexList[i * 3 + 1];
            planes[i].vertexC = vertexList[i * 3 + 2];

            Vector3 vectorAB = planes[i].vertexB - planes[i].vertexA;
            Vector3 vectorAC = planes[i].vertexC - planes[i].vertexA;

            Vector3 normalPlane = Vector3.Cross(vectorAB, vectorAC).normalized; //Calcula la normal con producto cruz y la normaliza

            //Verifica la orientaci�n y la cambia en caso de que no sea hacia el centro
            Vector3 vectorToPlane = point - planes[i].vertexA;
            float distanceToPlane = Vector3.Dot(vectorToPlane, normalPlane); //Si > 0 apuntan hacia el mismo lado (el centro), sino no

            if (distanceToPlane > 0.0f) //Si es mayor que cero mantengo la direcci�n porque esta hacia el centro
            {
                planes[i].normal = normalPlane;
            }
            else //Si apunta hacia diferente lado la multiplico por -1 para invertir su direcci�n porque significa que estaba hacia afuera
            {
                planes[i].normal = normalPlane * -1;
            }
        }
    }

    float PlanePointDistance(FrustumPlane plane, Vector3 pointToCheck)
    {
        float dist = Vector3.Dot(plane.normal, (pointToCheck - plane.vertexA));
        return dist;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        DrawFrustum();

        Gizmos.color = Color.red;
        for (int i = 0; i < planes.Count; i++)
        {
            Gizmos.DrawLine(planes[i].vertexA, planes[i].vertexA + planes[i].normal * 2);
        }
    }

    bool CheckObsjectsInFrustum()
    {
        for (int i = 0; i < boundinToCheck.Count; i++)
        {
            if (IsObjectFrustum(i))
            {
                Debug.Log(boundinToCheck[i].name + " bounding esta adentro");

                if (IsModelInFrustum(i))
                {
                    Debug.Log(boundinToCheck[i].name + " object esta adentro");
                    boundinToCheck[i].gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    Debug.Log(boundinToCheck[i].name + " object esta afuera");
                    boundinToCheck[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
                }

            }
            else
            {
                Debug.Log(boundinToCheck[i].name + " bounding esta afuera");
                boundinToCheck[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        return true;
    }

    bool IsObjectFrustum(int i)
    {
        for (int k = 0; k < planes.Count; k++) //Recorrer hasta planes.count
        {
            if (!IsObjectInPlane(i, k)) //Si esta afuera de un plano esta afuera del frustum
            {
                return false;
            }
        }

        return true;
    }

    bool IsObjectInPlane(int i, int k)
    {
        for (int j = 0; j < boundinToCheck[i].Vertices.Count; j++)
        {
            if (PlanePointDistance(planes[k], boundinToCheck[i].Vertices[j]) > 0.0f)
            {
                return true;
                //Si hay un punto dentro, entonces esta dentro
            }
        }
        return false;
    }

    bool IsModelInFrustum(int i)
    {
        Mesh mesh = boundinToCheck[i].gameObject.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int j = 0; j < triangles.Length; j += 3)
        {
            Vector3 vertex1 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 vertex2 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 vertex3 = transform.TransformPoint(vertices[triangles[i + 2]]);

            for (int k = 0; k < planes.Count; k++) //Recorrer hasta planes.count
            {
                if (PlanePointDistance(planes[k], vertex1) > 0.0f || PlanePointDistance(planes[k], vertex2) > 0.0f || PlanePointDistance(planes[k], vertex3) > 0.0f)
                {
                    return true;
                    //Si hay un punto dentro, entonces esta dentro
                }
            }
        }

        return false;
    }
}
