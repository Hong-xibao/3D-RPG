using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}

//当角色添加了这个脚本后，如果没有NavMeshAgent组件，就会自动添加NavMeshAgent组件
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator animator;
    protected CharacterStats characterStats;
    private Collider coll;

    [Header("Basic Settings")]
    public bool isGuard;//是否是站桩
    public float sightRadius;//视线范围
    public float lookAtTime;//巡逻一次停下来的时间
    private float remainLookAtTime;//巡逻后停下来剩下的时间
    private float speed;//存放默认的速度
    protected GameObject attackTarget;
    private float lastAttackTime;//下次攻击的时间
    private Quaternion guardRotation;//存放敌人站桩时的rotation

    [Header("Patrol State")]
    public float patrolRange;//巡逻范围
    private Vector3 patrolPoint;//存放巡逻的位置
    private Vector3 guardPos;//存放站桩位置，当站桩的enemy离开后再返回到指定的站桩位置

    //bool 配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead=false;

    private void Awake()
    {
       
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;

        remainLookAtTime = lookAtTime;
    }
    private void Start()
    {
    
        if(isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewPatrolPoint();
        }
        //FIXME:场景切换后修改掉
        GameManager.Instance.AddObserver(this);
    }

    //切换场景时启用
    //void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        else
        GameManager.Instance.RemoveObserver(this);
    }


    private void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        if(!playerDead)
        {
            SwitchStates();

            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
       
    
    }

    //切换动画机状态
    void SwitchAnimation()
    {
        animator.SetBool("Walk",isWalk);
        animator.SetBool("Chase",isChase);
        animator.SetBool("Follow",isFollow);
        animator.SetBool("Critical", characterStats.isCritical);
        animator.SetBool("Death", isDead);
    }

    //切换角色状态
    void SwitchStates()
    {
        if(isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        //当enemy检测到player
        else if(FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
          
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:

                isChase = false;
                //当角色当前的位置不同于站桩的位置,移动到站桩的位置
                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    
                    //Vector3.SqrMagnitude计算两个三维向量的差值,当小于agente.stoppingDistance时就表明到达了站桩位置
                    if ( Vector3.SqrMagnitude( guardPos - transform.position ) <= agent.stoppingDistance )
                    {
                        isWalk = false;

                        //到达位置后，缓慢旋转到原来的rotation
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation , 0.01f);
                    }
                }
                break;

            case EnemyStates.PATROL:

                isChase = false;
                agent.speed = speed * 0.5f;

                //当到达巡逻点
                if (Vector3.Distance(patrolPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if(remainLookAtTime>0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewPatrolPoint();
                    }
                  
                }
                else
                {
                    isWalk = true;
                    agent.destination = patrolPoint;
                }

                break;

            case EnemyStates.CHASE:

                isWalk = false;
                isChase = true;
                agent.speed = speed;//切换到chase状态就提升速度到默认速度
                //当没有检测到player时
                if (!FoundPlayer())
                {
                    isFollow = false;
                    
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }    
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
               
                //如果player在普通攻击和技能攻击范围内
                if(TargetInAttackRange()||TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    //如果下次攻击的时间<0，就更新冷却时间
                    if(lastAttackTime<0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断,如果随机出来的数字小于暴击率就返回true表明暴击了
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;

                        //执行攻击
                        Attack();
                    }
                }

                break;

            case EnemyStates.DEAD:

                coll.enabled = false;
                //缩小agent的范围让死后的敌人不成为障碍物
                agent.radius = 0;
                
                Destroy(gameObject, 2f);
               
                break;
        }
    }

    void Attack()
    {
        //面向player
        transform.LookAt(attackTarget.transform);
        
        //切换动画及状态
        if(TargetInAttackRange())
        {
            animator.SetTrigger("Attack");
            
        }
        if(TargetInSkillRange())
        {
            animator.SetTrigger("Skill");
           
        }

    }

    //检测player函数
    bool FoundPlayer()
    {
        //�ڵ��˵�positionΪ���ģ�sightRadiusΪ�뾶�����ײ�壬����һ������
        var colliders = Physics.OverlapSphere(transform.position,sightRadius);
        //�ڼ�⵽����ײ��������һ��һ���鿴��ȥ���ж��Ƿ����player��ǩ
        foreach(var target in colliders)
        {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;

        return false;
    }

    //判断普通攻击目标是否在范围内
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        }
        else return false;
    }
    //判断技能攻击目标是否在范围内
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        }
        else return false;
    }

    //获得新的巡逻点
    void GetNewPatrolPoint()
    {
        
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange,patrolRange);
        Vector3 randomPoint = new Vector3(
            guardPos.x + randomX,
            transform.position.y,
            guardPos.z + randomZ);

        NavMeshHit hit;
        //先判断，如果randomPoint是在areaMask=1（即walkAble)的区域内，则将返回hit的position
        patrolPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;


    }

    //画可视线
    public void OnDrawGizmosSelected()
    {
       
        Gizmos.color = Color.blue;
       
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }


    //Animation event
    //攻击目标造成伤害
   void Hit()
    {
        //当目标不为空且目标在敌人的攻击范围内时
        if( attackTarget!=null && transform.IsFacingTarget(attackTarget.transform) )
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats,targetStats);

        }
    }

    //当player死亡时，执行该函数
    public void EndNotify()
    {
        playerDead = true;
        animator.SetBool("Win", true);  
        isChase = false;
        isWalk = false;
        attackTarget = null;

    }
}
