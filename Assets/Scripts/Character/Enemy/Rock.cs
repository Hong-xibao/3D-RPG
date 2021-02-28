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
    public GameObject breakEffect;//ʯͷ�������Ч

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;

        rockStates = RockStates.HitPlayer;//һ��ʼʯͷ��״̬Ϊ����player
        FlyToTarget();
    }

    private void FixedUpdate()
    {
        //����Ҷ㿪ʯͷ�˵�Զ�̹�������ʯͷ���ٶ�С��1ʱ���ͽ���ʯͷ��״̬�޸�ΪHitNothing
        if(rb.velocity.sqrMagnitude<1f)
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()
    {
        //����ɫ����ʯͷ��Զ���빥����Χ����ʯͷ����ʯͷǰ�߳���Χ����targetΪ�գ����Ե�Ϊnullʱ����������player
        if(target == null )
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        //���Ȼ��Ŀ��ķ���Ȼ��Ϊ��rock���ӵ�ʱ����������һ��������һ�����ϵķ���
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
                    //����
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    //ѣ��
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    //�����˺�
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage
                        (
                        damage,
                        other.gameObject.GetComponent<CharacterStats>()
                        );

                    rockStates = RockStates.HitNothing;
                }
                break;

            case RockStates.HitEnemy:

                //�ж��Ƿ���Golem����ű����,���������ű��ͱ�����ʯͷ��
                if(other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//����ʯͷ������Ч
                    Destroy(gameObject);
                }

                break;
        }
    }
}
