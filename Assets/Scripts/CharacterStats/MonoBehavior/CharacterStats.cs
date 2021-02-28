using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    //第一个int为当前的血量，第二个int为满的血量，这个事件是用来更新血条的
    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterData_SO templateData;//模板数据
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    private AttackData_SO BaseAttackData;//基础的攻击数据

    private RuntimeAnimatorController baseAnimator;

    [Header("Weapon")]
    public Transform weaponSlot;//模型上存放武器的slot的transform

    [HideInInspector]
    public bool isCritical;//是否暴击了

    //如果当前的模板数据不为空，就生成一个模板数据赋值给characterData
    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        //生成一个attackData赋值给BaseAttackData，用来保存基础的attackData
        BaseAttackData = Instantiate(attackData);

        //存放一开始的animator，在卸下武器的时候切换成这个animator
        baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get{if (characterData != null) return characterData.maxHealth; else return 0;}
        set{characterData.maxHealth = value;}
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BasDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }
    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacter, CharacterStats defener)
    {
        int damage = Mathf.Max( attacter.CurrentDamage() - defener.CurrentDefence , 0 );
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        
        //如果攻击者暴击了,被攻击者就触发Hit，播放hit的动画
        if(attacter.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //Update UI,添加一个“？”是判断是否为空，如果不为空才执行事件里面的方法
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
       
        //经验update,当当前角色死亡后，攻击者就更新经验值
        if(CurrentHealth <= 0 )
        {
            attacter.characterData.UpdateExp(characterData.killPoint);
        }
    }

    public void TakeDamage(int damage,CharacterStats defener)
    {
        int currentDamage = Mathf.Max( damage - defener.CurrentDefence , 0 );
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);

        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage,attackData.maxDamage);
        if(isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            
        }
        return (int)coreDamage;
    }
    #endregion

    #region Equip Weapon

    //切换武器，先卸下武器然后装备上传输进来的武器
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    //装备武器，并且更新角色的攻击属性attackData
    public void EquipWeapon(ItemData_SO weapon)
    {
        //为了防止报错，先判断weapon的weaponPrefab不为空
        if(weapon.weaponPrefab !=null )
        {
            //在对应的位置上生成武器
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }
        //更新属性,拿到weapon的属性，用weapon的相关属性替换掉player的相关属性
        attackData.ApplyWeaponData(weapon.weaponData);
        //切换为持剑动画
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
    }

    //卸下装备，并且更新角色的攻击属性attackData
    public void UnEquipWeapon()
    {
        if(weaponSlot.transform.childCount != 0 )
        {
            //循环weaponSlot下的子物体并且删除
            for(int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(BaseAttackData);
        //卸下武器切换动画
        GetComponent<Animator>().runtimeAnimatorController = baseAnimator;
    }

    #endregion

    #region Apply Data Change
    public void ApplyHealth(int amount)
    {
        if(CurrentHealth + amount <= MaxHealth)
        {
            CurrentHealth += amount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }

    #endregion
}
