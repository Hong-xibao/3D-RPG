using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { BAG,WEAPON,ARMOR,ACTION}
public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;// 每个格子拿到自身的itemUI

    public void OnPointerClick(PointerEventData eventData)
    {
        //当鼠标双击的时候，使用可以使用的道具
       if(eventData.clickCount%2 == 0)
        {
            UseItem();
        }
    }
    //使用道具
    public void UseItem()
    {
        if(itemUI.GetItem() != null)
        {
            //判断要使用的物品是useable类型
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.inventoryData_Bag.items[itemUI.Index].amount > 0)
            {
                //更新角色的状态：回复生命，传入的值是物品能回复的血量
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);
                //使用后让背包的物品数量减少
                itemUI.inventoryData_Bag.items[itemUI.Index].amount -= 1;
            }
      
        }
        //更新UI显示
        UpdateItem();
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            //当slotType是BAG时，将InventoryManager中的inventoryData（Bag）这个数据库传输过来保存在itemUI的inventoryData中
            case SlotType.BAG:

                itemUI.inventoryData_Bag = InventoryManager.Instance.inventoryData;

                break;

            case SlotType.WEAPON:

                itemUI.inventoryData_Bag = InventoryManager.Instance.equipmentData;

                // 装备/切换 武器
                //判断bag里的数据是否为空
                if( itemUI.inventoryData_Bag.items[itemUI.Index].itemData != null )
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.inventoryData_Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    //当武器的耐久度没了后就卸下武器
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }

                break;

            case SlotType.ARMOR://盾牌

                itemUI.inventoryData_Bag = InventoryManager.Instance.equipmentData;
                //自由发挥创意

                break;

            case SlotType.ACTION:

                itemUI.inventoryData_Bag = InventoryManager.Instance.actionData;

                break;
        }

        //从拿到的背包的数据库的items列表中找到格子同样序号对应的物体，然后赋值给item
        var item = itemUI.inventoryData_Bag.items[itemUI.Index];

        //调用itemUI脚本中的函数用来更新数据
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //判断鼠标当前悬停的格子是否有东西
        if(itemUI.GetItem())
        {
            InventoryManager.Instance.tooltip.SetUpTooltip(itemUI.GetItem());
            InventoryManager.Instance.tooltip.gameObject.SetActive(true);
           
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }

    //当slotholder关闭的时候，也就是背包关闭的时候，将tooltip也关闭
    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }
}
