using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Rock : MonoBehaviour
{
    public enum RockStates{ HitPlayer , HitEnemy , HitNothing }
    public RockStates rockStates ;

    private Rigidbody rb;

    [Header("Basic Settings")]
    public float force;
    public GameObject target;
    private Vector3 direction;
    public int damage;
    public GameObject breakEffect;//石头破碎的特效

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;

        rockStates = RockStates.HitPlayer;//一开始石头的状态为攻击player
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        //当玩家躲开石头人的远程攻击，当石头的速度小于1时，就将该石头的状态修改为HitNothing
        if(rb.velocity.sqrMagnitude<1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        //当角色进入石头人远距离攻击范围后，在石头人扔石头前走出范围导致target为空，所以当为null时，继续丢给player
        if(target == null )
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        //首先获得目标的方向，然后为了rock在扔的时候像抛物线一样，加上一个向上的方向
        direction = (target.transform.position - transform.position + Vector3.up).normalized;

        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    
    private void OnCollisionEnter(Collision other)
    {
           switch(rockStates)
           {

            case RockStates.HitPlayer:
                if(other.gameObject.CompareTag("Player"))
                {
                    //击退
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    //眩晕
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    //产生伤害
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage
                        (
                        damage,
                        other.gameObject.GetComponent<CharacterStats>()
                        );

                    rockStates = RockStates.HitNothing;
                }
                break;

            case RockStates.HitEnemy:

                //判断是否有Golem这个脚本组件,如果有这个脚本就表明是石头人
                if(other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//生成石头破碎特效
                    Destroy(gameObject);
                }

                break;
        }
    }
}
