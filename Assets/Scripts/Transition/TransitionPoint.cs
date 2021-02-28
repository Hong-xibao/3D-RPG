using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    //���������ͣ�ͬ�������ͣ���ͬ��������
    public enum TransitionType
    {
        SameScene,DifferentScene
    }

    [Header("Transition Info")]
    public string sceneName;//��Ŵ���Ŀ��㳡������
    public TransitionType transitionType;//����������
    public TransitionDestination.DestinationTag destinationTag;//�յ㴫���ű�ǩ

    private bool canTrans;//�Ƿ���Դ���

    private void Update()
    {
        //������E�������ܴ��͵�ʱ��
        if( Input.GetKeyDown(KeyCode.E) && canTrans )
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    //��player���봥�����ӵķ�Χ�����ܴ��ͣ����˷�Χ���ܴ���
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}
