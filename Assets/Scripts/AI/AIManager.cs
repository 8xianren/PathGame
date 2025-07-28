using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance { get; private set; }
    [Header("AI设置")]
    [SerializeField] private List<GameObject> AIPrefabs; // 拖入3D人物预制体
    [SerializeField] private float scaleAI = 4f;

    [SerializeField] private List<Material> coverMaterials; // 覆盖材质列表

    
    private List<Vector2Int> spawnPositions = new List<Vector2Int>{
        new Vector2Int(0, 15),
        new Vector2Int(15, 0),
        new Vector2Int(15, 15)
    }; // 默认出生位置

    
    
    public List<GameObject> AIInstances { get; private set; } = new List<GameObject>(3);

    // Start is called before the first frame update
    void Start()
    {
        SpawnAI();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void Awake()
    {
        // 单例模式初始化
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject); // 跨场景保留
        }
    }


    public void SpawnAI()
    {
        // 实例化玩家
        
        AIGetInfo info = GameObject.Find("HexGrid").GetComponent<AIGetInfo>();
        for (int i = 0; i < 3; i++)
        {
            GameObject AIInstance = Instantiate(
                AIPrefabs[i],
                info.GetCellPosition(spawnPositions[i]),
                Quaternion.identity
            );
            AIInstance.transform.localScale = AIPrefabs[i].transform.localScale * scaleAI;
            AIInstance.AddComponent<Rigidbody>(); // 添加刚体组件

            AIInstance.AddComponent<CapsuleCollider>(); // 添加碰撞器组件


            AIInstance.AddComponent<AIController>(); // 添加玩家控制脚本


            AIInstance.GetComponent<AIController>().coverMaterial = coverMaterials[i]; // 设置覆盖材质
            AIInstance.name = "AICharacter" + i; // 设置实例名称

            string tagName = "AI" + i.ToString(); // 设置标签
            
            AIInstance.tag = tagName; // 设置标签

            AIInstance.AddComponent<Enemy>();
            AIInstance.layer = 6;

            AIInstance.AddComponent<Package>();

            AIInstances.Add(AIInstance);
            //Debug.Log("生成 AI" + i);
        }
        
        
       
    }


    

}