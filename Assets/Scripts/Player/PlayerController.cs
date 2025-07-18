using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private FloatingJoystick joystick; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joystick = FindObjectOfType<FloatingJoystick>();
    }

    void FixedUpdate()
    {
        
        Vector2 inputDirection = new Vector2(
            joystick.Horizontal, 
            joystick.Vertical
        ).normalized;

        
        if (inputDirection != Vector2.zero)
        {
            
            Vector3 moveVector = new Vector3(
                inputDirection.x,
                0,
                inputDirection.y
            ) * moveSpeed * Time.fixedDeltaTime;

            
            rb.MovePosition(rb.position + moveVector);

            
            Quaternion targetRotation = Quaternion.LookRotation(moveVector);
            rb.MoveRotation(Quaternion.Slerp(
                rb.rotation, 
                targetRotation, 
                rotationSpeed * Time.fixedDeltaTime
            ));
        }
    }
}
