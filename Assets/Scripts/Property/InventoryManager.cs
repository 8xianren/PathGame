using System.Collections.Generic;
using UnityEngine;

// 道具类型枚举
public enum ItemType
{
    HealthPotion,  // 生命药水
    ManaPotion,    // 法力药水
    SpeedBoost,    // 速度提升
    Shield,        // 护盾
    Coin,          // 金币
    Key            // 钥匙
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
    
    [Header("道具设置")]
    public List<ItemData> itemDatabase = new List<ItemData>();
    
    [Header("UI设置")]
    public Transform inventoryPanel;
    public GameObject itemSlotPrefab;
    public int maxSlots = 7;
    
    [Header("特效设置")]
    public GameObject comboEffectPrefab;
    
    private List<ItemType> collectedItems = new List<ItemType>();
    private Dictionary<ItemType, int> itemCounts = new Dictionary<ItemType, int>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    
    // 添加道具到背包
    public bool AddItem(ItemType itemType)
    {
        if (collectedItems.Count >= maxSlots)
        {
            Debug.Log("背包已满！");
            return false;
        }
        
        collectedItems.Add(itemType);
        itemCounts[itemType]++;
        
        UpdateInventoryUI();
        CheckForCombos(itemType);
        
        return true;
    }
    
    // 检查组合效果
    private void CheckForCombos(ItemType itemType)
    {
        if (itemCounts[itemType] >= 3)
        {
            // 触发效果
            TriggerComboEffect(itemType);
            
            // 移除三个相同的道具
            RemoveItemsOfType(itemType, 3);
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
    private void TriggerComboEffect(ItemType itemType)
    {
        Debug.Log($"触发组合效果: {itemType} x3!");
        
        // 播放特效
        if (comboEffectPrefab)
        {
            Instantiate(comboEffectPrefab, Vector3.zero, Quaternion.identity);
        }
        
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
        for (int i = 0; i < collectedItems.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, inventoryPanel);
            ItemSlot slotScript = slot.GetComponent<ItemSlot>();
            
            // 查找道具数据
            ItemData data = itemDatabase.Find(item => item.type == collectedItems[i]);
            
            if (data != null)
            {
                slotScript.SetItem(data.icon, collectedItems[i]);
            }
        }
    }
}