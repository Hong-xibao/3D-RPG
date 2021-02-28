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
                //如果是同场景传送，将当前激活的场景名字和传送目标点的标签传入到协程中
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;

            case TransitionPoint.TransitionType.DifferentScene:
                //跨场景传送
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));

                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {
        //保存player数据
        SaveManager.Instance.SavePlayerData();

        //如果当前正在使用的场景不同于传进来的场景名字
        if (SceneManager.GetActiveScene().name != sceneName)
        {


            //yield return 意思就是等待yield return 指定的事件完成，完成后执行后面的事件
            //后台预先加载sceneName这个场景
            yield return SceneManager.LoadSceneAsync(sceneName);

            //生成player
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            //生成player后并读取player的数据
            SaveManager.Instance.LoadPlayerData();

            //上面两个yield return 执行完成后，才把场景加载出来
            yield break;
        }
        else
        {
            //拿到player
            player = GameManager.Instance.playerStats.gameObject;

            //拿到player的agent，在传送的时候停止agent
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;

            //传送,传送结束后启用agent
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;

            yield return null;
        }

    }

    //得到目标点函数
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

    //加载主场景
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    //(continue)加载游戏
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    //开始新游戏到第一个Game场景
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    //加载场景的协程
    IEnumerator LoadLevel(string scene)
    {
        //生成一个SceneFader这个prefab，然后赋值给fade这个变量。
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (scene!="")
        {
            //开始fadeOut这个协程
            yield return StartCoroutine(fade.FadeOut(2f));

            //加载场景
            yield return SceneManager.LoadSceneAsync(scene);

            //生成玩家,位置和旋转为Game关卡的传送门前的一个点的位置
            yield return player = Instantiate( playerPrefab,GameManager.Instance.GetEntrance().position , GameManager.Instance.GetEntrance().rotation );

            //保存游戏
            SaveManager.Instance.SavePlayerData();

            //在上面各种加载完成后执行fadeIn这个协程
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

    //人物死亡的时候执行
    public void EndNotify()
    {
        if(fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());

        }
      
    }
}
