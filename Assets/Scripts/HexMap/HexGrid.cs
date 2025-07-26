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
    public TextMeshProUGUI cellLabelPrefab;

    Canvas gridCanvas;

    Color colorPlayer;

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
            (width) * HexMetrics.innerRadius,
            0.1f,
            (height) * HexMetrics.outerRadius * 0.75f
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

        cell.color = Color.white; // 设置默认颜色

    }
    // Start is called before the first frame update
    void Start()
    {
        hexMesh.Triangulate(cells);

        PlayerController player = FindObjectOfType<PlayerController>();
        colorPlayer = player.coverColor;
        if (player != null)
        {
            player.OnGroundPos += HandlePlayerOnGround;
        }

    }

    private void HandlePlayerOnGround(Transform playerTransform)
    {
        Vector3 playerPosition = playerTransform.position;
        playerPosition = transform.InverseTransformPoint(playerPosition);
        HexCoordinates hexCoords = HexMetrics.FromPosition(playerPosition);

        int ox = hexCoords.X + hexCoords.Z / 2;
        int oz = hexCoords.Z;
        int index = oz * width + ox;
        Debug.Log("Player is on hex: " + hexCoords + " at index: " + index);

        //HexMesh hexMesh = GetComponentInChildren<HexMesh>();
        CoverColor(index, colorPlayer);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CoverColor(int index, Color color)
    {
        if (index < 0 || index >= width * height)
        {
            Debug.LogWarning("Index out of range for CoverColor: " + index);
            return;
        }

        HexCell cell0 = cells[index];
        cell0.color = color;
        hexMesh.Triangulate(cells);

    }
    
    void OnDestroy()
    {
        // 取消订阅防止内存泄漏
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.OnGroundPos -= HandlePlayerOnGround;
        }
    }
}
