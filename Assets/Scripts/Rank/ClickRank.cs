using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickRank : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    public void TogglePanel()
    {
        if (panel != null)
        {
            // 切换 Panel 的激活状态
            bool isActive = panel.activeSelf;
            panel.SetActive(!isActive);
        }
    }
}
