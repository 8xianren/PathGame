using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lr;

    public HexCoordinates coordinates;
    void Start()
    {
        lr = gameObject.AddComponent<LineRenderer>();

        // 基本设置
        lr.material = new Material(Shader.Find("Unlit/Color")) { color = Color.black };
        lr.widthMultiplier = 0.2f;
        lr.positionCount = 7;
        
        // 转换坐标并设置
        UpdateLine();
    }

    void UpdateLine()
    {
        // 将本地坐标转换为世界坐标
        Vector3 v0 = transform.TransformPoint(HexMetrics.corners[1]);
        Vector3 v1 = transform.TransformPoint(HexMetrics.corners[2]);
        Vector3 v2 = transform.TransformPoint(HexMetrics.corners[3]);
        Vector3 v3 = transform.TransformPoint(HexMetrics.corners[4]);
        Vector3 v4 = transform.TransformPoint(HexMetrics.corners[5]);
        Vector3 v5 = transform.TransformPoint(HexMetrics.corners[6]);



        lr.SetPosition(0, v0);
        lr.SetPosition(1, v1);
        lr.SetPosition(2, v2);
        lr.SetPosition(3, v3);
        lr.SetPosition(4, v4);
        lr.SetPosition(5, v5);
        lr.SetPosition(6, v0); // 闭合线段
         // 闭合线段
        lr.loop = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
