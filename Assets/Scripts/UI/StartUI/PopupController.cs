using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField] private GameObject popupWindow; // 弹出窗口对象
    [SerializeField] private Button openButton;     // 打开按钮
    [SerializeField] private Button closeButton;   // 关闭按钮

    void Start()
    {
        // 默认隐藏窗口
        popupWindow.SetActive(false);

        // 绑定按钮事件
        openButton.onClick.AddListener(OpenPopup);
        closeButton.onClick.AddListener(ClosePopup);
    }

    private void OpenPopup()
    {
        popupWindow.SetActive(true); // 显示窗口
    }

    private void ClosePopup()
    {
        popupWindow.SetActive(false); // 隐藏窗口
    }

}
