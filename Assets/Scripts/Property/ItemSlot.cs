using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Image iconImage;
    //public GameObject countPanel;
    //public TextMeshProUGUI countText;
    
    private ItemType currentItemType;
    
    public void SetItem(Sprite icon, ItemType type)
    {
        iconImage.sprite = icon;
        currentItemType = type;

        // 显示堆叠数量（如果有）
        int count = InventoryManager.Instance.GetItemCount(type);
        
        /*
        if (count > 1)
        {
            countPanel.SetActive(true);
            countText.text = count.ToString();
        }
        else
        {
            countPanel.SetActive(false);
        }
        */
    }
    
    // 点击道具槽（可选功能）
    public void OnClick()
    {
        // 实现道具使用功能
        Debug.Log($"使用了道具: {currentItemType}");
    }
}
