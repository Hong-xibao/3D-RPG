using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName="";

    public string SceneName
    {
        get
        {
            return PlayerPrefs.GetString(sceneName);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

   
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }
    }

    //用来保存player的数据
    public void SavePlayerData()
    {
        //保存的数据是player数据的scriptObject，键值使用data的名字
        Save( GameManager.Instance.playerStats.characterData , GameManager.Instance.playerStats.characterData.name );
    }
    //用来加载player的数据
    public void LoadPlayerData()
    {
        //加载的数据是player数据的scriptObject，键值是data的名字
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }


    //存储的方法
    public void Save(Object data,string key)
    {
        //传入的data转变成json,第二个参数问你要不要写的好看一点
        var jsonData = JsonUtility.ToJson(data,true);

        //用playerPrefs这个方法将key和jsonData数据连接到一起保存到磁盘中
        PlayerPrefs.SetString(key, jsonData);

        //将当前正在使用的场景名字保存
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);

        //最后要使用save这个方法保存
        PlayerPrefs.Save();
    }

    //加载的方法
    public void Load(Object data, string key)
    {
        //判断关键值key上是否有数值
        if(PlayerPrefs.HasKey(key))
        {
            //在playerPrefs中拿到对应key中的值，然后把数值写回到data中
            JsonUtility.FromJsonOverwrite( PlayerPrefs.GetString(key) , data );
        }
    }
}
