using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;//���к�UI����������������õ�ͷ�ļ�


[RequireComponent(typeof(ItemUI))]
//�̳�3���ӿڣ���ק�Ŀ�ʼ����ק�Ĺ��̣�������ק
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //��õ�ǰ���ض����ItemUI
    ItemUI currentItemUI;

    //����������������������ʱʹ��
    SlotHolder currentHolder;
    SlotHolder targetHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();//��ø�����SlotHolder���
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //����һ���µ�DragData���� Ϊ��
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();

        //��ֵ������ԭʼ������---ÿ����ק��inventoryManager���ᱣ��ԭ�е�����

        //��parent���õ�SlotHolder
        InventoryManager.Instance.currentDrag.originaHolder = GetComponentInParent<SlotHolder>();

        //�õ�parent��transform
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;

        //��¼ԭʼ��Ϣ����ʼ��ק�˵���û�ط��ţ����ǾͷŻ�ԭ���ĵط������Եü�¼ԭʼ����Ϣ��

        //������ק�����parent
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //�������λ���ƶ�
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //������Ʒ ��������

        //�Ƿ�ָ��UI��Ʒ,���Ҹ�UI�Ǹ���
        if(EventSystem.current.IsPointerOverGameObject())
        {
            //����InventoryManager�еļ�ⷽ��������������ͷŵ�λ���Ƿ���������UI�����ڵĸ��ӷ�Χ֮��
            if( 
                InventoryManager.Instance.CheckInActionUI(eventData.position)||
                InventoryManager.Instance.CheckInEquipmentUI(eventData.position)||
                InventoryManager.Instance.CheckInInventoryUI(eventData.position)
              )
            {
                //�ж���������������Ƿ���SlotHolder
                if(eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    //�õ�������ڵ�slotHolder
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }
              switch(targetHolder.slotType)
                {
                    case SlotType.BAG:
                        SwapItem();//������Ʒ
                        break;
                    case SlotType.WEAPON:
                        if (currentItemUI.inventoryData_Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                        {
                            SwapItem();//������Ʒ
                        }
                        break;
                    case SlotType.ARMOR:
                        if (currentItemUI.inventoryData_Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                        {
                            SwapItem();//������Ʒ
                        }
                        break;
                    case SlotType.ACTION:
                        if(currentItemUI.inventoryData_Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                        {
                            SwapItem();//������Ʒ
                        }
                        
                        break;
                }

                //����holder
                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }

  
        //�������õ�ǰ��ק����ĸ���
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);

        RectTransform t = transform as RectTransform;

        t.offsetMax = -Vector2.one * 5;
        t.offsetMin = Vector2.one * 5;
    }

    //������Ʒ
    public void SwapItem()
    {
        //�õ�Ŀ��item��inventoryItem(��ǰ��Ϣ��Ŀ�������)
        var targetItem = targetHolder.itemUI.inventoryData_Bag.items[targetHolder.itemUI.Index];

        //����ǰ������ק��holder��inventoryItem��ֵ��tempItem
        var tempItem = currentHolder.itemUI.inventoryData_Bag.items[currentHolder.itemUI.Index];
        
        //�ж�Ŀ������ݺ�tempItem�����Ƿ�һ����һ����isSameItem����Ϊtrue
        bool isSameItem = tempItem.itemData == targetItem.itemData;

        //�ж��Ƿ�����ͬ����Ʒ�����ж��Ƿ��ǿɶѵ���
        if(isSameItem&& targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;

            //����ק��data���������
            tempItem.itemData = null;
            tempItem.amount = 0;
        }
        else
        {
            currentHolder.itemUI.inventoryData_Bag.items[currentHolder.itemUI.Index] = targetItem;
            targetHolder.itemUI.inventoryData_Bag.items[targetHolder.itemUI.Index] = tempItem;
        }
    }
}
