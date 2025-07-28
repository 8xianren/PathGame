using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// 道具类型枚举
public enum ItemType
{
    b1,  // 生命药水
    b2,    // 法力药水
    b3,    // 速度提升
    t1,        // 护盾
    t2,          // 金币
    t3,

    w1,
    w2,
    w3            // 钥匙
}

// 道具数据类
[System.Serializable]
public class ItemData
{
    public ItemType type;
    public Sprite icon;
    public int maxStack = 1; // 最大堆叠数量
}

// 背包管理器
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public Action<float, int> speedUpAction;

    public Action<Vector3, Material, HexMetrics.HexOwner> changeMat;

    [Header("道具设置")]
    public List<ItemData> itemDatabase = new List<ItemData>();

    [Header("UI设置")]
    public Transform inventoryPanel;
    public GameObject itemSlotPrefab;
    public int maxSlots = 7;

    public float cellsizeW = 94f; // 槽位宽度
    public float cellsizeH = 139f; // 槽位高度
    public float paddingW = 159f; // 槽位间距宽度
    public float paddingH = 10f; // 槽位间距高度

    public int left = 94; // 左边距
    public int right = 10; // 右边距
    public int top = 82; // 上边距
    public int bottom = 10; // 下边距

    public GameObject attack;


    [Header("特效设置")]
    public GameObject comboPlayerEat;

    public GameObject comboPlayerCrash;
    public GameObject comboPlayerCollect;
    public GameObject comboPlayerSpeed; // 组合效果特效

    private List<ItemType> collectedItems = new List<ItemType>();
    private Dictionary<ItemType, int> itemCounts = new Dictionary<ItemType, int>();

    public List<Material> materials;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化物品计数
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            itemCounts[type] = 0;
        }
    }

    void Start()
    {
        GridLayoutGroup grid = inventoryPanel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(cellsizeW, cellsizeH);  // 槽位大小
        grid.spacing = new Vector2(paddingW, paddingH);   // 槽位间距
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = 1;  // 单行显示

        grid.padding = new RectOffset(left, right, top, bottom); // 设置边距
    }

    // 添加道具到背包
    public bool AddItem(ItemType itemType, Collider collector)
    {
        if (collector.CompareTag("Player")) // 如果是玩家碰撞
        {
            // 通过单例访问
            if (collectedItems.Count >= maxSlots)
            {
                //Debug.Log("背包已满！");


                return false;
            }

            collectedItems.Add(itemType);
            PlayerManager.Instance.PlayerInstance.GetComponent<PlayerController>().PlayGet();
            itemCounts[itemType]++;


            GameObject newCollected=  Instantiate(comboPlayerCollect, collector.transform.position, Quaternion.identity);
            StartCoroutine(DestroyAfterDelay(newCollected, 2f));

            UpdateInventoryUI();
            
            CheckForCombosCrash(itemType, collector.transform.position, collector.gameObject);

            CheckForCombosEat(itemType,collector.transform.position, collector.gameObject);

            return true;
        }
        return false;


    }

    private void CheckForCombosEat(ItemType itemType, Vector3 itemPosition, GameObject obj)
    {
        int group = (int)itemType / 3;
        int num = (int)itemType % 3;
        bool isTrigger = false;

        if (num == 0)
        {
            ItemType n1 = itemType + 1;
            ItemType n2 = itemType + 2;

            if (itemCounts.ContainsKey(n1) && itemCounts.ContainsKey(n2))
            {
                if (itemCounts[n1] > 0 && itemCounts[n2] > 0)
                {
                    itemCounts[n1] -= 1;
                    itemCounts[n2] -= 1;
                    itemCounts[itemType] -= 1;

                    RemoveThreeItems(itemType, n1, n2);
                    isTrigger = true;

                }






            }
        }
        else if (num == 1)
        {
            ItemType n0 = itemType - 1;
            ItemType n2 = itemType + 1;

            if (itemCounts.ContainsKey(n0) && itemCounts.ContainsKey(n2))
            {
                if (itemCounts[n0] > 0 && itemCounts[n2] > 0)
                {
                    itemCounts[n0] -= 1;
                    itemCounts[n2] -= 1;
                    itemCounts[itemType] -= 1;

                    RemoveThreeItems(itemType, n0, n2);
                    isTrigger = true;

                }
            }


        }
        else
        {
            ItemType n0 = itemType - 2;
            ItemType n1 = itemType - 1;

            if (itemCounts.ContainsKey(n0) && itemCounts.ContainsKey(n1))
            {
                if (itemCounts[n0] > 0 && itemCounts[n1] > 0)
                {
                    itemCounts[n0] -= 1;
                    itemCounts[n1] -= 1;
                    itemCounts[itemType] -= 1;

                    RemoveThreeItems(itemType, n0, n1);
                    isTrigger = true;

                }
            }

        }
        if (!isTrigger)
        {
            return;

        }

        GameObject newEatEffect = Instantiate(comboPlayerEat, itemPosition, Quaternion.identity);
        newEatEffect.transform.localScale *= 10f;
        StartCoroutine(DestroyAfterDelay(newEatEffect, 3f));




        UpdateInventoryUI();
        attack.SetActive(true);

        StartCoroutine(Bomb());
            
        

    }


    private IEnumerator Bomb()
    {
        yield return new WaitForSeconds(2f);

        attack.SetActive(false);
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }


    void Update()
    {
        if (collectedItems.Count == maxSlots)
        {
            //Debug.Log("背包已满，无法添加更多道具！");
            speedUpAction?.Invoke(1.75f, 5);
            collectedItems.Clear(); // 清空背包

            PlayerManager.Instance.PlayerInstance.GetComponent<PlayerController>().PlaySpeedUp();
        }
    }

    // 检查组合效果
    private void CheckForCombosCrash(ItemType itemType, Vector3 itemPosition, GameObject obj)
    {
        if (itemCounts[itemType] >= 3)
        {

            // 触发效果
            TriggerComboEffectCrash(obj, itemPosition);
            

            // 移除三个相同的道具
            RemoveItemsOfType(itemType, 3);
        }
    }


    private void RemoveThreeItems(ItemType itemType0, ItemType itemType1, ItemType itemType2)
    {
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType0)
            {
                collectedItems.RemoveAt(i);


                break;
            }
        }
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType1)
            {
                collectedItems.RemoveAt(i);


                break;
            }
        }
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType2)
            {
                collectedItems.RemoveAt(i);
                

                break;
            }
        }

    }

    // 移除指定类型的道具
    private void RemoveItemsOfType(ItemType itemType, int count)
    {
        int removed = 0;

        // 从后往前移除
        for (int i = collectedItems.Count - 1; i >= 0; i--)
        {
            if (collectedItems[i] == itemType)
            {
                collectedItems.RemoveAt(i);
                removed++;

                if (removed >= count) break;
            }
        }

        itemCounts[itemType] -= count;
        UpdateInventoryUI();
    }

    // 触发组合效果
    private void TriggerComboEffectCrash(GameObject obj, Vector3 itemPosition)
    {
        //Debug.Log($"触发组合效果: {itemType} x3!");

        if (obj.CompareTag("Player"))
        {
            if (comboPlayerCrash)
            {
                GameObject newCrash = Instantiate(comboPlayerCrash, itemPosition, Quaternion.identity);
                newCrash.transform.localScale *= 40;
                StartCoroutine(DestroyAfterDelay(newCrash, 3f));
                PlaySoundCrash();


                //Debug.Log("crash");

                changeMat?.Invoke(itemPosition, materials[0], HexMetrics.HexOwner.Player);
                
                
                


            }
        }

        // 播放特效
            

        // 根据道具类型执行不同效果


    }

    

    // 更新背包UI
    public void UpdateInventoryUI()
    {
        // 清除所有现有的UI槽
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // 创建新的UI槽
        GridLayoutGroup grid = inventoryPanel.GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        grid.constraintCount = 1; // 单行显示

        // 创建新的UI槽
        for (int i = 0; i < collectedItems.Count; i++)
        {
            // 实例化槽位并正确设置父对象
            GameObject slot = Instantiate(itemSlotPrefab, inventoryPanel);
            slot.transform.SetParent(inventoryPanel, false);

            // 确保立即更新布局
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                inventoryPanel.GetComponent<RectTransform>()
            );

            // 设置槽位内容
            ItemSlot slotScript = slot.GetComponent<ItemSlot>();
            ItemData data = itemDatabase.Find(item => item.type == collectedItems[i]);

            if (data != null)
            {
                slotScript.SetItem(data.icon, collectedItems[i]);
            }
        }
    }

    public int GetItemCount(ItemType type)
    {
        itemCounts.TryGetValue(type, out int count);
        return count;

    }

    private void PlaySoundCrash()
    {
        
        PlayerManager.Instance.PlayerInstance.GetComponent<PlayerController>().PlayPong();
        // 播放碰撞音效
        // 可以在这里添加音频播放逻辑
        //Debug.Log("播放碰撞音效");

    }
}