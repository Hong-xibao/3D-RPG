using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{
    public Image icon = null;

    public Text amout = null;

    public InventoryData_SO inventoryData_Bag { get; set; }
    public int Index { get; set; } = -1;//因为是从0开始，为了防止错位


    //将图片和数字更新
    //拿到物品的数据和数量
    public void SetUpItemUI(ItemData_SO item,int itemAmount)
    {
        if(itemAmount == 0)
        {
            inventoryData_Bag.items[Index].itemData = null;
            icon.gameObject.SetActive(false);
            return;
        }

        if (item != null)
        {
            icon.sprite = item.itemIcon;
            amout.text = itemAmount.ToString();
            icon.gameObject.SetActive(true);
        }
        else
            icon.gameObject.SetActive(false);
    }

    //将UI中图片对应的物品返回
    public ItemData_SO GetItem()
    {
        return inventoryData_Bag.items[Index].itemData;
    }
}
