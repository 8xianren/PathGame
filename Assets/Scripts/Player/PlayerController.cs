using System;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
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
    private Animator animator;
    private Rigidbody rb;
    private FloatingJoystick joystick;
    private Transform mainCamera;

    // 移动状态
    private Vector3 moveDirection;
    private float currentSpeed;
    private float targetSpeed;
    private bool isGrounded = true;
    private bool isMoving;

    private CapsuleCollider myCapsuleCollider;
    private float playerScale;

    public event Action<Transform, Material, HexMetrics.HexOwner> OnGroundPos;

    //public event Action<Transform, Material> OnGroundPosTex;

    public Color coverColor = Color.green; // 覆盖颜色

    public Material coverMaterial; // 覆盖材质

    private HexMetrics.HexOwner owner;
    public UnityEngine.UI.Button jButton; // 跳跃按钮（如果需要）

    void Start()
    {
        // 获取组件引用
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main.transform;

        jButton = GameObject.Find("JumpButton").GetComponent<Button>();
        // 查找场景中的摇杆
        joystick = FindObjectOfType<FloatingJoystick>();
        if (joystick == null)
        {
            Debug.LogError("未找到摇杆组件！请确保场景中有摇杆对象");
        }

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
        myCapsuleCollider.height = 2f;

        jButton.onClick.AddListener(Jump); // 绑定跳跃按钮事件
                                           //jButton.onClick.Invoke();

        owner = HexMetrics.HexOwner.Player; // 设置默认所有者为玩家

    }

    void Update()
    {
        CheckGroundStatus();
        UpdateAnimations();
        
    }

    void FixedUpdate()
    {
        HandleMovement();
        
    }

    private void CheckGroundStatus()
    {
        // 使用射线检测地面状态
        RaycastHit hit;
        float rayLength = 0.2f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength))
        {
            isGrounded = true;


            OnGroundPos?.Invoke(transform, coverMaterial, owner);

            // 检查坡度是否可攀爬
                /*
                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle > maxSlopeAngle)
                {
                    isGrounded = false;
                }
                */
        }
        else
        {
            isGrounded = false;
        }

        // 设置动画参数
        animator.SetBool("IsGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        // 获取摇杆输入
        Vector2 joystickInput = new Vector2(joystick.Horizontal, joystick.Vertical);

        // 计算移动方向和速度
        if (joystickInput.magnitude > 0.05f)
        {
            // 计算相机相对移动方向
            Vector3 cameraForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
            moveDirection = joystickInput.y * cameraForward + joystickInput.x * mainCamera.right;

            // 标准化方向向量
            moveDirection.Normalize();

            // 计算目标速度
            targetSpeed = moveSpeed * joystickInput.magnitude;

            // 平滑旋转角色朝向移动方向
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            }

            isMoving = true;
        }
        else
        {
            // 没有输入时停止移动
            targetSpeed = 0f;
            isMoving = false;
        }

        // 平滑速度变化
        if (isMoving)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // 应用移动
        Vector3 moveVelocity = moveDirection * currentSpeed;

        // 保持Y轴速度（重力）
        if (isGrounded)
        {
            moveVelocity.y = rb.velocity.y;
        }
        else
        {
            // 在空中时保留垂直速度
            moveVelocity.y = rb.velocity.y;
        }

        // 应用速度到刚体
        rb.velocity = moveVelocity;
    }

    private void UpdateAnimations()
    {
        // 设置动画参数
        float animSpeed = Mathf.Clamp01(currentSpeed / moveSpeed);
        animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsMoving", isMoving);
    }

    // 摇杆事件处理（可选）
    public void OnPointerDown()
    {
        // 当按下摇杆时触发
        animator.SetBool("IsMoving", true);
    }

    public void OnPointerUp()
    {
        // 当松开摇杆时触发
        animator.SetBool("IsMoving", false);
    }

    // 跳跃功能
    public void Jump()
    {
        
        if (isGrounded)
        {
            
            animator.SetTrigger("Jump");
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }

    // 在Inspector中显示调试信息
    /*
    void OnDrawGizmos()
    {
        // 绘制移动方向
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + moveDirection * 2f);

        // 绘制地面检测射线
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * 0.2f);
    }
    */
}