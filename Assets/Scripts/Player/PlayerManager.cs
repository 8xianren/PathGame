using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [Header("玩家设置")]
    [SerializeField] private GameObject playerPrefab; // 拖入3D人物预制体
    [SerializeField] private Vector3 spawnPosition = new Vector3(8, 40, 0); // 默认出生位置
    [SerializeField] private float scalePlayer = 2f;

    public Material coverMaterial; // 覆盖材质
    // 当前玩家实例
    public GameObject PlayerInstance { get; private set; }

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

    public Vector3 GetSpwanPostion()
    {
        return PlayerManager.Instance.spawnPosition;



    }

    public void SpawnPlayer()
    {
        // 如果已有玩家实例则先销毁
        if (PlayerInstance != null)
        {
            Destroy(PlayerInstance);
        }

        // 实例化玩家
        PlayerInstance = Instantiate(
            playerPrefab,
            spawnPosition,
            Quaternion.identity
        );
        PlayerInstance.transform.localScale = playerPrefab.transform.localScale * scalePlayer;
        PlayerInstance.AddComponent<Rigidbody>(); // 添加刚体组件

        PlayerInstance.AddComponent<CapsuleCollider>(); // 添加碰撞器组件


        PlayerInstance.AddComponent<PlayerController>(); // 添加玩家控制脚本


        PlayerInstance.GetComponent<PlayerController>().coverMaterial = coverMaterial; // 设置覆盖材质
        PlayerInstance.name = "PlayerCharacter"; // 设置实例名称
        PlayerInstance.tag = "Player"; // 设置标签
        
    }

    public void SetPlayerPosition(Vector3 newPosition)
    {
        if (PlayerInstance != null)
        {
            PlayerInstance.transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("玩家实例不存在，无法设置位置");
        }
    }

    
}
