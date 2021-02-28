using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private CharacterStats characterStats;

    private GameObject attackTarget;
    private float lastAttackTime;

    private bool isDead;

    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }

    //当人物在场景中启用的时候，注册这两个方法
    private void OnEnable()
    {

        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    private void Start()
    {
        //一开始就让player拿到自己的data
        SaveManager.Instance.LoadPlayerData();
    }

    //当角色消失，也就是切换到其他场景的时候
    private void OnDisable()
    {
        if (!MouseManager.IsInitialized) return;

        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }





    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        //当角色死亡后就广播告知所有挂载了EndGameObserver接口的enemy
        if(isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    //动画切换函数,用来更新数值
    private void SwitchAnimation()
    {
        animator.SetFloat("Speed",agent.velocity.sqrMagnitude);
        animator.SetBool("Death",isDead);
    }

    //移动到目标点函数
    public void MoveToTarget(Vector3 target)
    {
        //停止所有Coroutine（协程）,用来打断攻击
        StopAllCoroutines();

        if (isDead) return;

        agent.stoppingDistance = stopDistance;

            //移动的时候agent不停止
            agent.isStopped = false;

            agent.destination = target;
       
       
    }

    //攻击事件
    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;

            //计算攻击是否暴击
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;

            //执行协程
            StartCoroutine(MoveToAttackTarget());
        }
    }

    //协程
    //移动到攻击的目标
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        //将player转向攻击的目标
        transform.LookAt(attackTarget.transform);

       
        //当player和攻击的目标相差远，就先往目标移动，进入player的攻击范围内后再攻击
        while( Vector3.Distance( attackTarget.transform.position , transform.position ) > characterStats.attackData.attackRange )
        {
            agent.destination = attackTarget.transform.position;
            //return后返回继续执行该函数
            yield return null;
        }

        agent.isStopped = true;

        //attack

        //攻击冷却时间结束了
        if(lastAttackTime<0)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("Critical",characterStats.isCritical);
            //重置攻击冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        //如果攻击的是可被攻击得物体，如rock
        if(attackTarget.CompareTag("Attackable"))
        {
            //判断是否带有Rock这个代码脚本，如果有就表明attackTarget为rock
            if(attackTarget.GetComponent<Rock>())
            {
                //将石头的状态改为攻击敌人的状态
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;

                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                //给石头添加一个player前面的方向的力
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
      
    }
}
