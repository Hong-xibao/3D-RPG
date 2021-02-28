using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    //��һ��intΪ��ǰ��Ѫ�����ڶ���intΪ����Ѫ��������¼�����������Ѫ����
    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterData_SO templateData;//ģ������
    public CharacterData_SO characterData;
    public AttackData_SO attackData;

    private AttackData_SO BaseAttackData;//�����Ĺ�������

    private RuntimeAnimatorController baseAnimator;

    [Header("Weapon")]
    public Transform weaponSlot;//ģ���ϴ��������slot��transform

    [HideInInspector]
    public bool isCritical;//�Ƿ񱩻���

    //�����ǰ��ģ�����ݲ�Ϊ�գ�������һ��ģ�����ݸ�ֵ��characterData
    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        //����һ��attackData��ֵ��BaseAttackData���������������attackData
        BaseAttackData = Instantiate(attackData);

        //���һ��ʼ��animator����ж��������ʱ���л������animator
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
        
        //��������߱�����,�������߾ʹ���Hit������hit�Ķ���
        if(attacter.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }

        //Update UI,���һ�����������ж��Ƿ�Ϊ�գ������Ϊ�ղ�ִ���¼�����ķ���
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth,MaxHealth);
       
        //����update,����ǰ��ɫ�����󣬹����߾͸��¾���ֵ
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

    //�л���������ж������Ȼ��װ���ϴ������������
    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);
    }

    //װ�����������Ҹ��½�ɫ�Ĺ�������attackData
    public void EquipWeapon(ItemData_SO weapon)
    {
        //Ϊ�˷�ֹ�������ж�weapon��weaponPrefab��Ϊ��
        if(weapon.weaponPrefab !=null )
        {
            //�ڶ�Ӧ��λ������������
            Instantiate(weapon.weaponPrefab, weaponSlot);
        }
        //��������,�õ�weapon�����ԣ���weapon����������滻��player���������
        attackData.ApplyWeaponData(weapon.weaponData);
        //�л�Ϊ�ֽ�����
        GetComponent<Animator>().runtimeAnimatorController = weapon.weaponAnimator;
    }

    //ж��װ�������Ҹ��½�ɫ�Ĺ�������attackData
    public void UnEquipWeapon()
    {
        if(weaponSlot.transform.childCount != 0 )
        {
            //ѭ��weaponSlot�µ������岢��ɾ��
            for(int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData.ApplyWeaponData(BaseAttackData);
        //ж�������л�����
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
