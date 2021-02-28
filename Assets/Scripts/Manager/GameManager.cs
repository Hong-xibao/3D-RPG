using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    private CinemachineFreeLook followCamera;//解决跨场景传送后相机丢失player的问题

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //定义一个列表用来存放继承IEndGameObserver接口的类
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;

        //找到场景中的freelook camera
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if(followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    //敌人生成就加入列表，死亡在列表中删除
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //广播
    public void NotifyObservers()
    {
        //列表中循环每一个观察者
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    //获得入口的Transform
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            //当传送门的标签为ENTER标签的时候返回该点的transform
            if(item.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }
        }
        return null;
    }
}
