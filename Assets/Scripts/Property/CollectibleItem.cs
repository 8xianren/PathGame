using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private Transform cameraTransform;
    // Start is called before the first frame update

    public ItemType itemType; // 物品类型
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (InventoryManager.Instance.AddItem(itemType, other))
            {
                // 通过单例访问
                Collect();
            }




        }
        if (other.CompareTag("AI0"))
        {
            if (AIManager.Instance.AIInstances[0].GetComponent<Package>().AddItem(itemType, other))
            {
                Collect();
            }
            
        }
        if (other.CompareTag("AI1"))
        {
            if (AIManager.Instance.AIInstances[1].GetComponent<Package>().AddItem(itemType, other))
            {
                Collect();
            }
        }
        if (other.CompareTag("AI2") )
        { 
            if (AIManager.Instance.AIInstances[2].GetComponent<Package>().AddItem(itemType, other))
            {
                Collect();
            }
        }
        
    }

    void Collect()
    {
        
        // 通过单例访问
        TimerManager.Instance.ItemCollected();
        

        // 收集效果（粒子/音效等）
        HexCoordinates hex = HexMetrics.FromPosition(transform.position);
        int ox = hex.X + hex.Z / 2;
        int oz = hex.Z;
        TimerManager.Instance.bitMap[ox][oz] = false;
        Destroy(gameObject);
    }

    void Update()
    {
        cameraTransform = Camera.main.transform;

        // 始终面向摄像机
        transform.LookAt(cameraTransform);

    }
}
