using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//泛型单例模式
public class Singleton<T> : MonoBehaviour where T:Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get { return instance; }
    }

    //在子类继承和重写的awake函数
    protected virtual  void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
          
    }

    //判断是否生成了instance
    public static bool IsInitialized
    {
        //如果instance不为空，则当前单例模式已经生成过了
        get { return instance != null; }
    }

    protected virtual void OnDestroy()
    {
        //当前instance被销毁了，就设置为null
        if(instance == this)
        {
            instance = null;
        }
    }
}
