using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RankScores : MonoBehaviour
{
    [SerializeField] private Button closeButton;   // 关闭按钮
    [SerializeField] private GameObject popupWindow;
    public string gameSceneName = "GameScene";

    void Start()
    {
        // 默认隐藏窗口
        

        // 绑定按钮事件
        
        closeButton.onClick.AddListener(ClosePopup);
    }

    

    private void ClosePopup()
    {
        popupWindow.SetActive(false); // 隐藏窗口
        SceneManager.LoadScene(gameSceneName);
    }
}
