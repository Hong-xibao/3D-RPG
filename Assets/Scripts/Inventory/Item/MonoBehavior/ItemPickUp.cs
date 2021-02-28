 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemData_SO itemData;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //将物品（自身）添加到背包
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            //物品添加到背包后就刷新背包
            InventoryManager.Instance.inventoryBagUI.RefreshUI();
            //装备武器
            //在GameManager上可以获得player身上挂载的characterStats这个脚本
            //GameManager.Instance.playerStats.EquipWeapon(itemData);

            Destroy(gameObject);
        }
    }


}
