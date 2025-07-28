using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEditor.Animations;
using UnityEngine;

public class HexGrid : MonoBehaviour
{

    public int width = 6;
    public int height = 6;

    public GameObject cellPrefab;



    private new BoxCollider collider;
    public GameObject[] cells;
    //public TextMeshProUGUI cellLabelPrefab;

    //Canvas gridCanvas;

    public float myScale = 1008f;
    public Dictionary<GameObject, HexMetrics.HexOwner> cellsHash = new Dictionary<GameObject, HexMetrics.HexOwner>();

    public List<List<HexMetrics.HexOwner>> cellOwners;

    void Awake()
    {
        cells = new GameObject[width * height];
        cellOwners = HexMetrics.cellOwners;

        for (int x = 0; x < width; ++x)
        {
            cellOwners.Add(new List<HexMetrics.HexOwner>());
            for (int z = 0; z < height; ++z)
            {
                cellOwners[x].Add(HexMetrics.HexOwner.origin);
            }
        }

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


        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].transform.localScale = new Vector3(myScale, myScale, myScale);
        }
    }




    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        cells[i] = Instantiate<GameObject>(cellPrefab);
        cells[i].transform.SetParent(transform, false);
        cells[i].transform.localPosition = position;
        cells[i].transform.localScale = new Vector3(myScale, myScale, myScale);

        HexCell hexcell = cells[i].GetComponent<HexCell>();
        hexcell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);


        //TextMeshProUGUI label = Instantiate<TextMeshProUGUI>(cellLabelPrefab);
        //label.rectTransform.SetParent(gridCanvas.transform, false);
        //label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        //label.text = cell.coordinates.ToStringOnSeparateLines();



        cellsHash.Add(cells[i], HexMetrics.HexOwner.origin);

        //cellOwners[x][z] = HexMetrics.HexOwner.origin;
    }
    // Start is called before the first frame update
    void Start()
    {


        PlayerController player = FindObjectOfType<PlayerController>();

        if (player != null)
        {

            player.OnGroundPos += HandlePlayerOnGround;
        }

        AIController[] aiControllers = FindObjectsOfType<AIController>();

        for (int i = 0; i < aiControllers.Length; i++)
        {
            AIController aiController = aiControllers[i];
            if (aiController != null)
            {
                aiController.AIOnGroundPos += HandleAIOnGround;
            }
            AIManager.Instance.AIInstances[i].GetComponent<Package>().changeMat += HandleCrashEffect;

        }

        InventoryManager.Instance.changeMat += HandleCrashEffect;

    }

    private void HandleCrashEffect(Vector3 pos, Material mat, HexMetrics.HexOwner owner)
    {

        List<Vector2Int> needTobePrintCells = HexMetrics.GetLinkedCells(pos);

        

        foreach (var Child in needTobePrintCells)
        {
            int x = Child.x;
            int z = Child.y;

            

            cellOwners[x][z] = owner;

            int index = z * width + x;

            CoverMaterial(index, mat);
            cellsHash[cells[index]] = owner;

        }
        
        
    }

    private void HandleAIOnGround(Transform playerTransform, Material mat, HexMetrics.HexOwner owner)
    {
        Vector3 playerPosition = playerTransform.position;
        playerPosition = transform.InverseTransformPoint(playerPosition);
        HexCoordinates hexCoords = HexMetrics.FromPosition(playerPosition);

        int ox = hexCoords.X + hexCoords.Z / 2;
        int oz = hexCoords.Z;
        int index = oz * width + ox;

        if (index < 0 || index >= width * height || ox < 0 || ox >= width || oz < 0 || oz >= height)
        {
            //Debug.LogWarning("Index out of range for HandleAIOnGround: " + index);
            return;
        }
        //Debug.Log("AI is on hex: " + hexCoords + " at index: " + index);

        //HexMesh hexMesh = GetComponentInChildren<HexMesh>();

        /*
        if (cellOwners[ox][oz] == owner)
        {
            Debug.LogWarning("Owner already set for this cell: " + owner);
            return;
        }
        */



        CoverMaterial(index, mat);
        //Debug.Log("AI CoverMaterial at index: " + index + " with owner: " + owner);

        cellsHash[cells[index]] = owner;

        cellOwners[ox][oz] = owner;
    }


    private Vector2Int TransCoordsTo(Transform playerTransform)
    {
        Vector3 playerPosition = playerTransform.position;
        playerPosition = transform.InverseTransformPoint(playerPosition);
        HexCoordinates hexCoords = HexMetrics.FromPosition(playerPosition);

        int ox = hexCoords.X + hexCoords.Z / 2;
        int oz = hexCoords.Z;

        return new Vector2Int(ox, oz);
    }

    



    private void HandlePlayerOnGround(Transform playerTransform, Material mat, HexMetrics.HexOwner owner)
    {
        Vector3 playerPosition = playerTransform.position;
        playerPosition = transform.InverseTransformPoint(playerPosition);
        HexCoordinates hexCoords = HexMetrics.FromPosition(playerPosition);

        int ox = hexCoords.X + hexCoords.Z / 2;
        int oz = hexCoords.Z;
        int index = oz * width + ox;

        if (index < 0 || index >= width * height || ox < 0 || ox >= width || oz < 0 || oz >= height)
        {
            //Debug.LogWarning("Index out of range for HandlePlayerOnGround: " + index);
            return;
        }
        
        /*
        if (cellOwners[ox][oz] == owner)
        {
            //Debug.LogWarning("Owner already set for this cell: " + owner);
            return;

        }
        */


        CoverMaterial(index, mat);

        cellsHash[cells[index]] = owner;

        cellOwners[ox][oz] = owner;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CoverMaterial(int index, Material mat)
    {
        if (index < 0 || index >= width * height)
        {
            Debug.LogWarning("Index out of range for CoverMaterial: " + index);
            return;
        }

        GameObject cell0 = cells[index];
        cell0.GetComponent<Renderer>().material = mat;
        

    }

    void OnDestroy()
    {
        // 取消订阅防止内存泄漏
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.OnGroundPos -= HandlePlayerOnGround;

        }
        AIController[] aiControllers = FindObjectsOfType<AIController>();

        for (int i = 0; i < aiControllers.Length; i++)
        {
            AIController aiController = aiControllers[i];
            if (aiController != null)
            {
                aiController.AIOnGroundPos -= HandleAIOnGround;
            }
            AIManager.Instance.AIInstances[i].GetComponent<Package>().changeMat -= HandleCrashEffect;
        }

        InventoryManager.Instance.changeMat -= HandleCrashEffect;
    }

    
}
