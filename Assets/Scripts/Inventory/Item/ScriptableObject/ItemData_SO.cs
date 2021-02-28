using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����һ��ö�������������Ʒ�����ͣ�Useable��ʹ�õģ�Weapon��������Armor��װ�������ף�
public enum ItemType { Useable,Weapon,Armor}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    //��Ʒ����
    public ItemType itemType;

    //��Ʒ����
    public string itemName;

    //��Ʒ��ͼ��
    public Sprite itemIcon;

    //��Ʒ������
    public int itemAmount;

    //��Ʒ������
    [TextArea]
    public string descrpiting = "";

    public bool stackable;   //�Ƿ���Զѵ�

    [Header("Useable Item")]
    public UseabelItemData_SO useableData;

    [Header("Weapon")]
    public GameObject weaponPrefab;//��player�������ɵ�prefab
    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}
