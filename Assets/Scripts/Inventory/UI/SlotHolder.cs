using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { BAG,WEAPON,ARMOR,ACTION}
public class SlotHolder : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public SlotType slotType;
    public ItemUI itemUI;// ÿ�������õ������itemUI

    public void OnPointerClick(PointerEventData eventData)
    {
        //�����˫����ʱ��ʹ�ÿ���ʹ�õĵ���
       if(eventData.clickCount%2 == 0)
        {
            UseItem();
        }
    }
    //ʹ�õ���
    public void UseItem()
    {
        if(itemUI.GetItem() != null)
        {
            //�ж�Ҫʹ�õ���Ʒ��useable����
            if (itemUI.GetItem().itemType == ItemType.Useable && itemUI.inventoryData_Bag.items[itemUI.Index].amount > 0)
            {
                //���½�ɫ��״̬���ظ������������ֵ����Ʒ�ܻظ���Ѫ��
                GameManager.Instance.playerStats.ApplyHealth(itemUI.GetItem().useableData.healthPoint);
                //ʹ�ú��ñ�������Ʒ��������
                itemUI.inventoryData_Bag.items[itemUI.Index].amount -= 1;
            }
      
        }
        //����UI��ʾ
        UpdateItem();
    }

    public void UpdateItem()
    {
        switch (slotType)
        {
            //��slotType��BAGʱ����InventoryManager�е�inventoryData��Bag��������ݿ⴫�����������itemUI��inventoryData��
            case SlotType.BAG:

                itemUI.inventoryData_Bag = InventoryManager.Instance.inventoryData;

                break;

            case SlotType.WEAPON:

                itemUI.inventoryData_Bag = InventoryManager.Instance.equipmentData;

                // װ��/�л� ����
                //�ж�bag��������Ƿ�Ϊ��
                if( itemUI.inventoryData_Bag.items[itemUI.Index].itemData != null )
                {
                    GameManager.Instance.playerStats.ChangeWeapon(itemUI.inventoryData_Bag.items[itemUI.Index].itemData);
                }
                else
                {
                    //���������;ö�û�˺��ж������
                    GameManager.Instance.playerStats.UnEquipWeapon();
                }

                break;

            case SlotType.ARMOR://����

                itemUI.inventoryData_Bag = InventoryManager.Instance.equipmentData;
                //���ɷ��Ӵ���

                break;

            case SlotType.ACTION:

                itemUI.inventoryData_Bag = InventoryManager.Instance.actionData;

                break;
        }

        //���õ��ı��������ݿ��items�б����ҵ�����ͬ����Ŷ�Ӧ�����壬Ȼ��ֵ��item
        var item = itemUI.inventoryData_Bag.items[itemUI.Index];

        //����itemUI�ű��еĺ���������������
        itemUI.SetUpItemUI(item.itemData, item.amount);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //�ж���굱ǰ��ͣ�ĸ����Ƿ��ж���
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

    //��slotholder�رյ�ʱ��Ҳ���Ǳ����رյ�ʱ�򣬽�tooltipҲ�ر�
    private void OnDisable()
    {
        InventoryManager.Instance.tooltip.gameObject.SetActive(false);
    }
}
