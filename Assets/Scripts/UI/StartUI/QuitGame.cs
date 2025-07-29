using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // 当按钮被点击时调用此方法
    public void OnQuitButtonClicked()
    {
        // 调试信息，在控制台可见
        Debug.Log("退出游戏按钮被点击");
        
        // 实际退出应用的代码
        QuitApplication();
    }
    
    public void QuitApplication()
    {
        #if UNITY_EDITOR
        // 在编辑器中停止播放模式
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 在构建的应用中退出程序
        Application.Quit();
        #endif
    }
}