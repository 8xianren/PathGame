using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{
    Mesh hexMesh;

    List<Vector3> vertices;

    List<int> triangles;


    private Material mat;

    private List<Vector2> uvs = new List<Vector2>();

    private List<Color> colors;



    public void Awake()
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hex Mesh";
        MeshRenderer myRender = gameObject.GetComponent<MeshRenderer>();
        //Debug.Log("Material Loaded: " + myRender.material.name);

        //mat = myRender.material = Resources.Load<Material>("Materials/Stylize Snow");
        //Debug.Log("Material Loaded: " + myRender.material.name);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        colors = new List<Color>();



    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void Triangulate(HexCell[] cells)
    {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        for (int i = 0; i < cells.Length; i++)
        {
            Triangulate(cells[i]);
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.colors = colors.ToArray();

        /*
        hexMesh.uv = uvs.ToArray();
        hexMesh.RecalculateTangents();
        */

        hexMesh.RecalculateNormals();
    }

    public void Triangulate(HexCell cell)
    {
        Vector3 center = cell.transform.localPosition;
        for (int i = 0; i < 6; i++)
        {
            AddTriangle(
                center,
                center + HexMetrics.corners[i],
                center + HexMetrics.corners[i + 1]
            );
            AddTriangleColor(cell.color);

            /*
            uvs.Add(new Vector2(0, 0));
            AddUVs(center, i);
            AddUVs(center, i);
            */
        }
    }

    void AddTriangleColor(Color color)
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void AddUVs(Vector3 center, int i)
    {
        Vector3 corner = center + HexMetrics.corners[i];
        Vector2 uv = CalculateHexUV(corner, center, i);
        uvs.Add(uv);
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private Vector2 CalculateHexUV(Vector3 position, Vector3 center, int cornerIndex)
    {
        // 方法1：平铺UV（适合无缝纹理）
        Vector3 relativePos = position - center;
        return new Vector2(
            relativePos.x / (2f * HexMetrics.outerRadius) + 0.5f,
            relativePos.z / (2f * HexMetrics.outerRadius) + 0.5f
        ).normalized;
    }

    
}
