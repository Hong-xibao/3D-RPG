using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//创建一个枚举类型来存放物品的类型，Useable：使用的，Weapon：武器，Armor：装备（盔甲）
public enum ItemType { Useable,Weapon,Armor}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class ItemData_SO : ScriptableObject
{
    //物品类型
    public ItemType itemType;

    //物品名字
    public string itemName;

    //物品的图标
    public Sprite itemIcon;

    //物品的数量
    public int itemAmount;

    //物品的详情
    [TextArea]
    public string descrpiting = "";

    public bool stackable;   //是否可以堆叠

    [Header("Useable Item")]
    public UseabelItemData_SO useableData;

    [Header("Weapon")]
    public GameObject weaponPrefab;//在player身上生成的prefab
    public AttackData_SO weaponData;
    public AnimatorOverrideController weaponAnimator;
}
