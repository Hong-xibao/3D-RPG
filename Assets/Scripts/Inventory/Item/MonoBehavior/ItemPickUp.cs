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
            //����Ʒ��������ӵ�����
            InventoryManager.Instance.inventoryData.AddItem(itemData, itemData.itemAmount);
            //��Ʒ��ӵ��������ˢ�±���
            InventoryManager.Instance.inventoryBagUI.RefreshUI();
            //װ������
            //��GameManager�Ͽ��Ի��player���Ϲ��ص�characterStats����ű�
            //GameManager.Instance.playerStats.EquipWeapon(itemData);

            Destroy(gameObject);
        }
    }


}
