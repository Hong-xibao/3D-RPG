using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "New Inventory" , menuName = "Inventory/Inventory Data") ]
public class InventoryData_SO : ScriptableObject
{
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(ItemData_SO newItemData,int amount )
    {
        bool found = false;
        //判断是否堆叠，如果可以堆叠，就判断列表当中是否有相同的物品，如果有相同的物品，就加上捡起来物品的数量
        if(newItemData.stackable)
        {
            foreach (var item in items)
            {
                if(item.itemData == newItemData)
                {
                    item.amount += amount;
                    found = true;
                    break;
                }
            }
        }
        //如果背包中没有相同的东西，就把物品放到最前空着的位置
        for (int i = 0; i < items.Count; i++)
        {
            //当前背包位置为空，并且found为false（没找到相同的物品）
            if(items[i].itemData == null && !found)
            {
                items[i].itemData = newItemData;
                items[i].amount = amount;
                break;
            }
        }
    }
}

[System.Serializable]
public class InventoryItem
{
    public ItemData_SO itemData;
    public int amount;//物品数量
}

