using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public Button[] buttons;  // 在Inspector中分配三个按钮
    private GameObject player;  // 玩家位置
    public GameObject missilePrefab;

    void Start()
    {
        player = PlayerManager.Instance.PlayerInstance;

        for (int i = 0; i < 3; ++i)
        {
            int index = i;
            buttons[index].onClick.AddListener(() =>
        {
            FindObjectOfType<ButtonController>().FireAtTarget(AIManager.Instance.AIInstances[index]);
        });
        }


    }

    public void FireAtTarget(GameObject obj)
    {
        // 禁用所有按钮
        

        // 创建导弹
        GameObject missile = Instantiate(
            missilePrefab,
            player.transform.position + 10*Vector3.up,
            Quaternion.identity
        );

        PlayerManager.Instance.PlayerInstance.GetComponent<PlayerController>().PlayEat();

        missile.transform.localScale *= 5f;

        // 设置跟踪目标
        missile.GetComponent<HomingMissile>().target = obj;
    }

    void Update()
    {

    }
}
