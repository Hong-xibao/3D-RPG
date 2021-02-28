using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{
    public GameObject playerPrefab;

    public SceneFader sceneFaderPrefab;

    bool fadeFinished;

    GameObject player;
    NavMeshAgent playerAgent;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        Debug.Log(transitionPoint.sceneName);

        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                //�����ͬ�������ͣ�����ǰ����ĳ������ֺʹ���Ŀ���ı�ǩ���뵽Э����
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;

            case TransitionPoint.TransitionType.DifferentScene:
                //�糡������
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));

                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //����player����
        SaveManager.Instance.SavePlayerData();

        //�����ǰ����ʹ�õĳ�����ͬ�ڴ������ĳ�������
        if (SceneManager.GetActiveScene().name != sceneName)
        {


            //yield return ��˼���ǵȴ�yield return ָ�����¼���ɣ���ɺ�ִ�к�����¼�
            //��̨Ԥ�ȼ���sceneName�������
            yield return SceneManager.LoadSceneAsync(sceneName);

            //����player
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //����player�󲢶�ȡplayer������
            SaveManager.Instance.LoadPlayerData();

            //��������yield return ִ����ɺ󣬲Űѳ������س���
            yield break;
        }
        else
        {
            //�õ�player
            player = GameManager.Instance.playerStats.gameObject;

            //�õ�player��agent���ڴ��͵�ʱ��ֹͣagent
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;

            //����,���ͽ���������agent
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;

            yield return null;
        }

    }

    //�õ�Ŀ��㺯��
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        foreach (var i in entrances)
        {
            if (i.destinationTag == destinationTag)
            {
                return i;
            }
        }

        return null;
    }

    //����������
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    //(continue)������Ϸ
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //��ʼ����Ϸ����һ��Game����
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    //���س�����Э��
    IEnumerator LoadLevel(string scene)
    {
        //����һ��SceneFader���prefab��Ȼ��ֵ��fade���������
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (scene!="")
        {
            //��ʼfadeOut���Э��
            yield return StartCoroutine(fade.FadeOut(2f));

            //���س���
            yield return SceneManager.LoadSceneAsync(scene);

            //�������,λ�ú���תΪGame�ؿ��Ĵ�����ǰ��һ�����λ��
            yield return player = Instantiate( playerPrefab,GameManager.Instance.GetEntrance().position , GameManager.Instance.GetEntrance().rotation );

            //������Ϸ
            SaveManager.Instance.SavePlayerData();

            //��������ּ�����ɺ�ִ��fadeIn���Э��
            yield return StartCoroutine(fade.FadeIn(2f));

            yield break;
        }

    }
    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    //����������ʱ��ִ��
    public void EndNotify()
    {
        if(fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());

        }
      
    }
}
