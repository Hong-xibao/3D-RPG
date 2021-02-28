using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;//获取生命条的prefab
    public Transform barPoint;//用来定位生命条UI的位置的变量
    public bool healthBarAlwaysVisible;//血条是否是一直可见的
    public float healthBarVisibleTime;//血条可视化时间
    private float remainingTime;//血条剩余的可视化时间

    Image healthSilder;//滑动条，HealthBarHolder这个prefab中的CurrentHealth这个可以滑动改变血量的Image
    Transform UIBar;//生成prefab后，获得坐标位置和barPoint保持一致
    Transform cam;//存放摄像机的位置，为了让血条一直面向camera

    CharacterStats currentStates;

    private void Awake()
    {
        //一开始就获得enemy的当前state并且将更新血量条的方法添加到event中
        currentStates = GetComponent<CharacterStats>();

        currentStates.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    //当人物启动的时候触发的函数
    private void OnEnable()
    {
        //获得场景中的camera
        cam = Camera.main.transform;

        //遍历当前场景中的所有Canvas
        foreach ( Canvas canvas in FindObjectsOfType<Canvas>() )
        {
            //如果当前的canvas的renderMode（渲染模式）为WorldSpace
            if(canvas.renderMode == RenderMode.WorldSpace)
            {
                //生成prefab并且定位到canvas中,然后将该生成的Prefab的transform赋值给UIBar，这样就拿到了血条的坐标参数
                UIBar = Instantiate(healthUIPrefab, canvas.transform).transform;

                //取得UIBar中的第一个子物体( currentHealth（iamge）)
                healthSilder = UIBar.GetChild(0).GetComponent<Image>();

                //设置是否一直可见
                UIBar.gameObject.SetActive(healthBarAlwaysVisible);
            }
        }
    }

    //更新血条的函数
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        //如果当前血量小于等于0，就销毁UIBar
        if (currentHealth <= 0) 
            Destroy(UIBar.gameObject);
        
        //每次受到伤害的时候血条一定是可见的
        UIBar.gameObject.SetActive(true);

        //UI剩下的可视化时间赋值
        remainingTime = healthBarVisibleTime;

        //设置血量滑动条的大小（slider ）
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSilder.fillAmount = sliderPercent;
    }

    /*LateUpdate是自带的函数，是在上一帧渲染后再执行，人物先移动，然后血条UI更上，不会造成血条闪烁的问题
     * 摄像机跟随应该在这个函数中
       血条UI跟随enemy移动
    */
    private void LateUpdate()
    {
        if(UIBar!=null)
        {
            UIBar.position = barPoint.position;

            //血条的UI方向为摄像机的 （- forward）方向,让血条面对摄像机
            UIBar.forward = -cam.forward;

            //当血条UI可视化时间<=0并且不是长时间显示时，血条UI设置为不可见
            if (remainingTime <= 0 && !healthBarAlwaysVisible)
            {
                UIBar.gameObject.SetActive(false);
            }
            else
            {
                remainingTime -= Time.deltaTime;
            }
        }
    }
}
