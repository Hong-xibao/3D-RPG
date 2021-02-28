using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    //传送门类型：同场景传送，不同场景传送
    public enum TransitionType
    {
        SameScene,DifferentScene
    }

    [Header("Transition Info")]
    public string sceneName;//存放传送目标点场景名字
    public TransitionType transitionType;//传送门类型
    public TransitionDestination.DestinationTag destinationTag;//终点传送门标签

    private bool canTrans;//是否可以传送

    private void Update()
    {
        //当按下E键并且能传送的时候
        if( Input.GetKeyDown(KeyCode.E) && canTrans )
        {
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    //当player进入触发盒子的范围，就能传送，出了范围不能传送
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
