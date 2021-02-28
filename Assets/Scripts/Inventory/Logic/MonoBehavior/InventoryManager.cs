using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : Singleton<InventoryManager>
{


    public class DragData
    {
        //ԭ����holder
        public SlotHolder originaHolder;
        
        //ԭ����parent
        public RectTransform originalParent;
    }


    //TODO:������ģ�����ڱ�������
    [Header("Inventory Data")]
    public InventoryData_SO inventoryData;//�������
    public InventoryData_SO actionData;
    public InventoryData_SO equipmentData;

    [Header("Containers")]
    public ContainerUI inventoryBagUI;//��汳��UI
    public ContainerUI actionUI;
    public ContainerUI equipmentUI;

    [Header("Drag Canvas")]
    public Canvas dragCanvas;
    public DragData currentDrag;

    [Header("UI Panel")]
    public GameObject bagPanel;
    public GameObject statsPanel;
    bool bagIsOpen;
    bool statsIsOpen;

    [Header("Stats Text")]
    public Text healthText;
    public Text attackText;

    [Header("Tooltip")]
    public ItemTooltip tooltip;

    //һ��ʼ��ˢ��UI
    private void Start()
    {
        inventoryBagUI.RefreshUI();
        actionUI.RefreshUI();
        equipmentUI.RefreshUI();
       
    }
    private void Update()
    {
        bagIsOpen = bagPanel.activeInHierarchy;
        statsIsOpen = statsPanel.activeInHierarchy;
        if (Input.GetKeyDown(KeyCode.B))
        {
            bagIsOpen = !bagIsOpen;
            bagPanel.SetActive(bagIsOpen);

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            statsIsOpen = !statsIsOpen;
            statsPanel.SetActive(statsIsOpen);
        }

        UpdateStatsText(GameManager.Instance.playerStats.CurrentHealth,
            GameManager.Instance.playerStats.attackData.minDamage,
            GameManager.Instance.playerStats.attackData.maxDamage);
    }

    public void UpdateStatsText(int health , int minAttack,int maxAttack)
    {
        healthText.text = health.ToString();
        attackText.text = minAttack + "����" + maxAttack;
    }

    #region ��3��UI��������ק��Ʒ�Ƿ���slot��Χ��
    public bool CheckInInventoryUI(Vector3 position)
    {
        for (int i = 0; i < inventoryBagUI.slotHolders.Length; i++)
        {
            RectTransform t = inventoryBagUI.slotHolders[i].transform as RectTransform;

            //�ж��ڸ�����posiiton�Ƿ���t�ķ�Χ�ڣ�Ҳ���Ǹ��ӵķ�Χ��
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInActionUI(Vector3 position)
    {
        for (int i = 0; i < actionUI.slotHolders.Length; i++)
        {
            RectTransform t = actionUI.slotHolders[i].transform as RectTransform;

            //�ж��ڸ�����posiiton�Ƿ���t�ķ�Χ�ڣ�Ҳ���Ǹ��ӵķ�Χ��
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckInEquipmentUI(Vector3 position)
    {
        for (int i = 0; i < equipmentUI.slotHolders.Length; i++)
        {
            RectTransform t = equipmentUI.slotHolders[i].transform as RectTransform;

            //�ж��ڸ�����posiiton�Ƿ���t�ķ�Χ�ڣ�Ҳ���Ǹ��ӵķ�Χ��
            if (RectTransformUtility.RectangleContainsScreenPoint(t, position))
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}
