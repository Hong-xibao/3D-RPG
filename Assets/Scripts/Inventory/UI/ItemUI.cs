using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemUI : MonoBehaviour
{
    public Image icon = null;

    public Text amout = null;

    public InventoryData_SO inventoryData_Bag { get; set; }
    public int Index { get; set; } = -1;//��Ϊ�Ǵ�0��ʼ��Ϊ�˷�ֹ��λ


    //��ͼƬ�����ָ���
    //�õ���Ʒ�����ݺ�����
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

    //��UI��ͼƬ��Ӧ����Ʒ����
    public ItemData_SO GetItem()
    {
        return inventoryData_Bag.items[Index].itemData;
    }
}
