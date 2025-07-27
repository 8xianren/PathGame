using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

    [Header("Settings")]
    public float warmupDuration = 30f;
    public float gameDuration = 120f;
    public float spawnInterval = 2f;
    public int maxItems = 45;

    public int startIndex = 3;

    public int endIndex = 11;

    [Header("References")]
    public TextMeshProUGUI timerDisplay;
    public GameObject[] itemPrefabs;
    //public Vector3 spawnAreaSize = new Vector3(10, 0, 10);

    private float currentTime;
    private bool isSpawningItems;
    private int activeItems;

    public float maJiangScale = 80f; // 麻将缩放比例

    public int n;

    public bool[][] bitMap;

    public int mapWidth = 16;
    public int mapHeight = 16;


    void Awake()
    {
        
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 如果希望跨场景保持，可以取消下面注释
            // DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        maJiangScale = 250f; // 麻将缩放比例
        bitMap = new bool[mapWidth][];
        for (int i = 0; i < mapWidth; i++)
        {
            bitMap[i] = new bool[mapHeight];
        }
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                bitMap[i][j] = false;
            }
        }
        n = itemPrefabs.Length;
        currentTime = 0f;
        timerDisplay.text = FormatTime(currentTime);
        StartCoroutine(GameTimer());
    }


    IEnumerator GameTimer()
    {
        // 第一阶段：前30秒
        while (currentTime < warmupDuration)
        {
            currentTime += Time.deltaTime;
            timerDisplay.text = FormatTime(currentTime);
            yield return null;
        }

        // 第二阶段：开始生成道具
        isSpawningItems = true;
        StartCoroutine(SpawnItems());

        // 第三阶段：30-120秒游戏阶段
        while (currentTime < gameDuration)
        {
            currentTime += Time.deltaTime;
            timerDisplay.text = FormatTime(currentTime);
            yield return null;
        }

        // 游戏结束
        GameEnd();
    }

    IEnumerator SpawnItems()
    {
        while (isSpawningItems)
        {
            while (activeItems < maxItems)
            {
                int tmpX = Random.Range(startIndex, endIndex);
                int tmpZ = Random.Range(startIndex, endIndex);
                if (bitMap[tmpX][tmpZ]) continue;

                bitMap[tmpX][tmpZ] = true;

                int itemIndex = Random.Range(0, n);
                GameObject itemPrefab = itemPrefabs[itemIndex];

                Vector3 position;
                position.x = (tmpX + tmpZ * 0.5f - tmpZ / 2) * (HexMetrics.innerRadius * 2f);
                position.y = 5f;
                position.z = tmpZ * (HexMetrics.outerRadius * 1.5f);

                itemPrefab.transform.localScale = new Vector3(maJiangScale, maJiangScale, maJiangScale);

                

                

                GameObject newItem = Instantiate(itemPrefab, position, Quaternion.identity);
                
                newItem.AddComponent<CollectibleItem>();
                newItem.AddComponent<BoxCollider>();
                newItem.GetComponent<BoxCollider>().isTrigger = true;
                newItem.GetComponent<BoxCollider>().size = new Vector3(0.024f, 0.03f, 0.01f);
                newItem.GetComponent<BoxCollider>().center = new Vector3(0, 0f, 0);

                


                activeItems++;
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void ItemCollected()
    {
        activeItems--;
    }

    void GameEnd()
    {
        isSpawningItems = false;
        timerDisplay.text = "游戏结束!";
        timerDisplay.color = Color.red;
        
        // 游戏结束逻辑
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    // 添加公共属性以便其他脚本访问状态
    public bool IsWarmupPhase => currentTime < warmupDuration;
    public bool IsGameActive => currentTime < gameDuration && !IsWarmupPhase;
    public float CurrentTime => currentTime;
}
