using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    private CinemachineFreeLook followCamera;//����糡�����ͺ������ʧplayer������

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //����һ���б�������ż̳�IEndGameObserver�ӿڵ���
    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;

        //�ҵ������е�freelook camera
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if(followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    //�������ɾͼ����б��������б���ɾ��
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    //�㲥
    public void NotifyObservers()
    {
        //�б���ѭ��ÿһ���۲���
        foreach(var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    //�����ڵ�Transform
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            //�������ŵı�ǩΪENTER��ǩ��ʱ�򷵻ظõ��transform
            if(item.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }
        }
        return null;
    }
}
