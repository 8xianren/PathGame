using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public HexCell cellPrefab;

    private HexMesh hexMesh;

    private new BoxCollider collider;
    HexCell[] cells;
    public  TextMeshProUGUI cellLabelPrefab;

    Canvas gridCanvas;

    void Awake()
    {
        cells = new HexCell[width * height];

        gridCanvas = GetComponentInChildren<Canvas>();
        hexMesh = GetComponentInChildren<HexMesh>();

        for (int z = 0, i = 0; z < height; ++z)
        {
            for (int x = 0; x < width; ++x)
            {
                CreateCell(x, z, i++);
            }
        }

        collider = gameObject.AddComponent<BoxCollider>();

        collider.size = new Vector3(
            (width + 2) * HexMetrics.innerRadius * 2f,
            0f,
            (height + 2) * HexMetrics.outerRadius * 1.5f
        );

        collider.center = new Vector3(
            (width ) * HexMetrics.innerRadius,
            0.1f,
            (height ) * HexMetrics.outerRadius * 0.75f
        );
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;

        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);



        TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        


    }
    // Start is called before the first frame update
    void Start()
    {
        hexMesh.Triangulate(cells);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
