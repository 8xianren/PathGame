using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    public GameObject target;
    public float speed = 10f;
    public float rotateSpeed = 200f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 2秒后自动销毁防止卡顿
        Destroy(gameObject, 5f);
        Vector3 direction = target.transform.position - rb.position;
        float dis = direction.magnitude;
        speed = dis / 2.5f;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 计算方向
        Vector3 direction = target.transform.position - rb.position;

        direction.Normalize();

        // 计算旋转
        Vector3 rotateAmount = Vector3.Cross(transform.forward, direction);
        rb.angularVelocity = rotateAmount * rotateSpeed;

        rb.velocity = transform.forward * speed;
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject == target)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                PlayerManager.Instance.PlayerInstance.GetComponent<PlayerController>().Stayed();
                Destroy(gameObject);
                return;
            }
            target.GetComponent<Enemy>().Stun();
            Destroy(gameObject);
        }
    }
}
