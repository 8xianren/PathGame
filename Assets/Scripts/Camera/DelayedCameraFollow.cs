using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedCameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;//player 
    
    [Header("延迟设置")]
    public float followDelay = 1f; // 延迟时间(秒)
    public AnimationCurve followCurve; // 跟随的缓动曲线
    
    [Header("位置设置")]
    public Vector3 positionOffset = new Vector3(0, 5f, -5f); // 相对于目标的偏移量
    
    private Vector3 initialPosition; // 初始位置
    private float timer = 0f; // 计时器
    private bool startFollowing = false; // 开始跟随标志
    
    void Start()
    {
        if (target == null) 
        {
            Debug.LogWarning("摄像机跟随目标未设置！");
            return;
        }
        
        // 记录初始位置
        initialPosition = transform.position;
        
        // 初始化缓动曲线
        if (followCurve.length == 0)
        {
            followCurve = new AnimationCurve(
                new Keyframe(0f, 0f), 
                new Keyframe(1f, 1f)
            );
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        if (!startFollowing)
        {
            // 更新计时器
            timer += Time.deltaTime;
            
            // 达到延迟时间后开始跟随
            if (timer >= followDelay)
            {
                startFollowing = true;
                timer = 0f; // 重置计时器
            }
        }
        else
        {
            // 跟随目标
            FollowTarget();
        }
    }
    
    void FollowTarget()
    {
        // 计算目标位置
        Vector3 targetPosition = target.position + positionOffset;
        
        // 应用缓动移动
        float t = Mathf.Clamp01(timer / followDelay);
        float curveValue = followCurve.Evaluate(t);
        transform.position = Vector3.Lerp(initialPosition, targetPosition, curveValue);
        
        // 始终看向目标
        transform.LookAt(target);
        
        timer += Time.deltaTime;
    }
}