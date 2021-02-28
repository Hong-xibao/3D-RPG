using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��create�˵����д����Ӽ��˵�
[CreateAssetMenu(fileName = "New Data",menuName ="Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefence;//��������
    public int currentDefence;

    [Header("Kill")]
    public int killPoint;//��ɱ���˺�õ��ľ������

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//ÿ�����������İٷֱ�

    //��ÿ���������´��������辭�� = �ϴ��������辭��*�� 1+ LevelMultiplier �� 
    public float LevelMultiplier
    {
        get
        {
            return 1 + (currentLevel - 1) * levelBuff;
        }
    }

    //���¾���ֵ
    public void UpdateExp(int point)
    {
        currentExp += point;
        if( currentExp >= baseExp )
        {
            LevelUp();
        }
    }

    //�������������ݷ���
    private void LevelUp()
    {
        //�ȼ�+1�����Ǵ�С��������С����ߵȼ�֮��
        currentLevel = Mathf.Clamp(currentLevel+1,1,maxLevel);
        //�����´���������Ҫ�ľ���ֵ
        baseExp += (int)(baseExp * LevelMultiplier);
        currentExp = 0;

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;

        Debug.Log("LEVEL UP!��   currentLevel��" + currentLevel + "     Max Health:" + maxHealth);
    }
}
