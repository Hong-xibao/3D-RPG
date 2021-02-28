using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float PushForce;//�����ƿ�player����

    public void PushOff()
    {
        if(attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //���ɵķ���
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            
            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * PushForce;
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");
        }
    }
}
