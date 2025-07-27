using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private Transform cameraTransform;
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
            
        }
    }

    void Collect()
    {
        // 通过单例访问
        TimerManager.Instance.ItemCollected();

        // 收集效果（粒子/音效等）
        Destroy(gameObject);
    }

    void Update()
    {
        cameraTransform = Camera.main.transform;

        // 始终面向摄像机
        transform.LookAt(cameraTransform);

    }
}
