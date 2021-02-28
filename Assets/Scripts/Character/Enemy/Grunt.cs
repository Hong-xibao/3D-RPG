using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float PushForce;//技能推开player的力

    public void PushOff()
    {
        if(attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //击飞的方向
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * PushForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
