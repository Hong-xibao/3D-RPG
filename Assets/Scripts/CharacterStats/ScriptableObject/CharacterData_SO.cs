using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在create菜单当中创建子集菜单
[CreateAssetMenu(fileName = "New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;//基础防御
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;//击杀敌人后得到的经验点数

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//每次升级提升的百分比

    //计每次升级后，下次升级所需经验 = 上次升级所需经验*（ 1+ LevelMultiplier ） 
    public float LevelMultiplier
    {
        get
        {
            return 1 + (currentLevel - 1) * levelBuff;
        }
    }

    //更新经验值
    public void UpdateExp(int point)
    {
        currentExp += point;
        if( currentExp >= baseExp )
        {
            LevelUp();
        }
    }

    //所有提升的数据方法
    private void LevelUp()
    {
        //等级+1，但是大小限制在最小和最高等级之间
        currentLevel = Mathf.Clamp(currentLevel+1,1,maxLevel);
        //计算下次升级所需要的经验值
        baseExp += (int)(baseExp * LevelMultiplier);
        currentExp = 0;

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LEVEL UP!：   currentLevel：" + currentLevel + "     Max Health:" + maxHealth);
    }
}
