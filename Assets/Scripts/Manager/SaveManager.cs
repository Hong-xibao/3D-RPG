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

    //��������player������
    public void SavePlayerData()
    {
        //�����������player���ݵ�scriptObject����ֵʹ��data������
        Save( GameManager.Instance.playerStats.characterData , GameManager.Instance.playerStats.characterData.name );
    }
    //��������player������
    public void LoadPlayerData()
    {
        //���ص�������player���ݵ�scriptObject����ֵ��data������
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }


    //�洢�ķ���
    public void Save(Object data,string key)
    {
        //�����dataת���json,�ڶ�����������Ҫ��Ҫд�ĺÿ�һ��
        var jsonData = JsonUtility.ToJson(data,true);

        //��playerPrefs���������key��jsonData�������ӵ�һ�𱣴浽������
        PlayerPrefs.SetString(key, jsonData);

        //����ǰ����ʹ�õĳ������ֱ���
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);

        //���Ҫʹ��save�����������
        PlayerPrefs.Save();
    }

    //���صķ���
    public void Load(Object data, string key)
    {
        //�жϹؼ�ֵkey���Ƿ�����ֵ
        if(PlayerPrefs.HasKey(key))
        {
            //��playerPrefs���õ���Ӧkey�е�ֵ��Ȼ�����ֵд�ص�data��
            JsonUtility.FromJsonOverwrite( PlayerPrefs.GetString(key) , data );
        }
    }
}
