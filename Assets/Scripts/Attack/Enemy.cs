using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public bool isStunned = false;
    
    public void Stun()
    {
        if(!isStunned)
        {
            StartCoroutine(StunRoutine());
        }
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;


        // 禁用移动逻辑（根据你的实现）
        // GetComponent<EnemyMovement>().enabled = false;
        //gameObject.GetComponent<AIController>().moveSpeed = 0f;
        gameObject.GetComponent<AIController>().enabled = false;
        yield return new WaitForSeconds(2f);

        gameObject.GetComponent<AIController>().enabled = true;
        
        //gameObject.GetComponent<AIController>().moveSpeed = 20f;
        // 恢复状态


        isStunned = false;
    }
}
