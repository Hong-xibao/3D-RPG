using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;//所有和UI的组件产生互动调用的头文件


[RequireComponent(typeof(ItemUI))]
//继承3个接口，拖拽的开始，拖拽的过程，结束拖拽
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //获得当前挂载对象的ItemUI
    ItemUI currentItemUI;

    //这两个变量用来交换格子时使用
    SlotHolder currentHolder;
    SlotHolder targetHolder;

    private void Awake()
    {
        currentItemUI = GetComponent<ItemUI>();
        currentHolder = GetComponentInParent<SlotHolder>();//获得父级的SlotHolder组件
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //声明一个新的DragData对象 为空
        InventoryManager.Instance.currentDrag = new InventoryManager.DragData();

        //赋值，保存原始的数据---每次拖拽，inventoryManager都会保管原有的数据

        //从parent中拿到SlotHolder
        InventoryManager.Instance.currentDrag.originaHolder = GetComponentInParent<SlotHolder>();

        //得到parent的transform
        InventoryManager.Instance.currentDrag.originalParent = (RectTransform)transform.parent;

        //记录原始信息（开始拖拽了但是没地方放，于是就放回原来的地方，所以得记录原始的信息）

        //设置拖拽物体的parent
        transform.SetParent(InventoryManager.Instance.dragCanvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //跟随鼠标位置移动
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //放下物品 交换数据

        //是否指向UI物品,并且该UI是格子
        if(EventSystem.current.IsPointerOverGameObject())
        {
            //调用InventoryManager中的检测方法，检测鼠标最后释放的位置是否在这三个UI区域内的格子范围之内
            if( 
                InventoryManager.Instance.CheckInActionUI(eventData.position)||
                InventoryManager.Instance.CheckInEquipmentUI(eventData.position)||
                InventoryManager.Instance.CheckInInventoryUI(eventData.position)
              )
            {
                //判断鼠标进入的物体上是否有SlotHolder
                if(eventData.pointerEnter.gameObject.GetComponent<SlotHolder>())
                {
                    //得到鼠标所在的slotHolder
                    targetHolder = eventData.pointerEnter.gameObject.GetComponent<SlotHolder>();
                }
                else
                {
                    targetHolder = eventData.pointerEnter.gameObject.GetComponentInParent<SlotHolder>();
                }
              switch(targetHolder.slotType)
                {
                    case SlotType.BAG:
                        SwapItem();//交换物品
                        break;
                    case SlotType.WEAPON:
                        if (currentItemUI.inventoryData_Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Weapon)
                        {
                            SwapItem();//交换物品
                        }
                        break;
                    case SlotType.ARMOR:
                        if (currentItemUI.inventoryData_Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Armor)
                        {
                            SwapItem();//交换物品
                        }
                        break;
                    case SlotType.ACTION:
                        if(currentItemUI.inventoryData_Bag.items[currentItemUI.Index].itemData.itemType == ItemType.Useable)
                        {
                            SwapItem();//交换物品
                        }
                        
                        break;
                }

                //更新holder
                currentHolder.UpdateItem();
                targetHolder.UpdateItem();
            }
        }

  
        //重新设置当前拖拽对象的父级
        transform.SetParent(InventoryManager.Instance.currentDrag.originalParent);

        RectTransform t = transform as RectTransform;

        t.offsetMax = -Vector2.one * 5;
        t.offsetMin = Vector2.one * 5;
    }

    //交换物品
    public void SwapItem()
    {
        //得到目标item的inventoryItem(当前信息和目标的数量)
        var targetItem = targetHolder.itemUI.inventoryData_Bag.items[targetHolder.itemUI.Index];

        //将当前正在拖拽的holder的inventoryItem赋值给tempItem
        var tempItem = currentHolder.itemUI.inventoryData_Bag.items[currentHolder.itemUI.Index];
        
        //判断目标的数据和tempItem数据是否一样，一样则isSameItem变量为true
        bool isSameItem = tempItem.itemData == targetItem.itemData;

        //判断是否是相同的物品并且判断是否是可堆叠的
        if(isSameItem&& targetItem.itemData.stackable)
        {
            targetItem.amount += tempItem.amount;

            //将拖拽的data和数量清空
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
