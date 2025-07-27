using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AIController : MonoBehaviour
{

    [Header("移动设置")]
    public float moveSpeed = 20f;         // 移动速度
    public float rotationSpeed = 15f;    // 旋转速度
    public float acceleration = 10f;     // 加速度
    public float deceleration = 15f;     // 减速度
    public float maxSlopeAngle = 45f;    // 最大可攀爬坡度

    [Header("动画设置")]
    public float animationBlendSpeed = 8f; // 动画混合速度

    // 内部组件引用
    public Animator aiAnimator; // AI角色的动画控制器
                                // Start is called before the first frame update
    private Rigidbody rb;


    // 移动状态
    private Vector3 moveDirection;
    private float currentSpeed;

    private CapsuleCollider myCapsuleCollider;
    public event Action<Transform, Material, HexMetrics.HexOwner> OnGroundPos;

    //public event Action<Transform, Material> OnGroundPosTex;

    //public Color coverColor = Color.green; // 覆盖颜色

    public Material coverMaterial; // 覆盖材质

    private HexMetrics.HexOwner owner;

    private AIGetInfo info;
    private List<List<HexMetrics.HexOwner>> CellList;

    private Dictionary<HexMetrics.HexOwner, int> Owner2Idx = new Dictionary<HexMetrics.HexOwner, int>();
    private Vector2Int index;
    private Vector2Int target_pos = new Vector2Int(-1, -1);
    private Vector2Int last_index = new Vector2Int(-1, -1);
    private static readonly List<HexMetrics.HexOwner> OwnerList = new List<HexMetrics.HexOwner>
    {
        HexMetrics.HexOwner.AI0,
        HexMetrics.HexOwner.AI1,
        HexMetrics.HexOwner.AI2
    };
    private const int width = 16;

    void Start()
    {
        // 获取组件引用
        aiAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        info = GameObject.Find("HexGrid").GetComponent<AIGetInfo>();

        // 配置刚体物理属性
        if (rb != null)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        myCapsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        myCapsuleCollider.center = new Vector3(0, 1f, 0);
        myCapsuleCollider.radius = 0.4f;
        myCapsuleCollider.height = 2.5f;

        string name = gameObject.name;
        char lastChar = name[name.Length - 1];        // 取最后一个字符
        int index = lastChar - '0';                   // 转成int（适合数字字符）
        owner = OwnerList[index];
        int i = 0;
        foreach (HexMetrics.HexOwner o in System.Enum.GetValues(typeof(HexMetrics.HexOwner)))
        {
            if (o != owner)
            {
                Owner2Idx[o] = i;
                i++;
            }
        }
        CellList = info.GetCellList();
    }

    // Update is called once per frame
    void Update()
    {
        index = info.GetCellIndex(transform.position);
        // CellList = info.GetCellList();
        if ((target_pos.x == -1) || (target_pos == index) || (last_index != index))
            SetTarget();
        if (CellList[index.x][index.y] != owner)
            CellList[index.x][index.y] = owner;
        // Debug.Log("目标坐标" + target_pos);
        // Debug.Log("当前坐标" + index);
        if (target_pos != index)
        {
            MoveToTargetPosition(info.GetCellPosition(target_pos));
        }
        last_index = index;
    }

    private void SetTarget()
    {
        List<float> distance = new List<float> { float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity };
        List<Vector2Int> tp = Enumerable.Repeat(Vector2Int.zero, 4).ToList();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                Vector2Int idx = new Vector2Int(x, z);
                float dis = info.GetDistanceToCell(index, idx);
                if (Owner2Idx.ContainsKey(CellList[x][z]))
                {
                    int p = Owner2Idx[CellList[x][z]];
                    if (distance[p] > dis)
                    {
                        distance[p] = dis;
                        tp[p] = idx;
                    }
                }

            }
        }
        target_pos = tp[Owner2Idx[HexMetrics.HexOwner.origin]];
        Debug.Log("角色ID" + owner + "目标坐标"+target_pos);
        // if (owner == HexMetrics.HexOwner.AI0)
        // {
        //     Debug.Log("目标坐标"+target_pos);
        // }
        // target_pos = new Vector2Int(10, 10);
    }

    public void MoveToTargetPosition(Vector3 targetPosition)
    {
        // 计算移动方向
        moveDirection = (targetPosition - transform.position).normalized;

        // 计算目标速度
        float targetSpeed = moveSpeed;

        // 更新当前速度
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        // 移动AI角色
        rb.MovePosition(transform.position + moveDirection * currentSpeed * Time.deltaTime);

        // 旋转AI角色朝向目标位置
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 更新动画状态
        aiAnimator.SetFloat("Speed", currentSpeed);
        
        OnGroundPos?.Invoke(transform, coverMaterial, owner);

    }
}
