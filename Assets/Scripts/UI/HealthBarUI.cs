using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;//��ȡ��������prefab
    public Transform barPoint;//������λ������UI��λ�õı���
    public bool healthBarAlwaysVisible;//Ѫ���Ƿ���һֱ�ɼ���
    public float healthBarVisibleTime;//Ѫ�����ӻ�ʱ��
    private float remainingTime;//Ѫ��ʣ��Ŀ��ӻ�ʱ��

    Image healthSilder;//��������HealthBarHolder���prefab�е�CurrentHealth������Ի����ı�Ѫ����Image
    Transform UIBar;//����prefab�󣬻������λ�ú�barPoint����һ��
    Transform cam;//����������λ�ã�Ϊ����Ѫ��һֱ����camera

    CharacterStats currentStates;

    private void Awake()
    {
        //һ��ʼ�ͻ��enemy�ĵ�ǰstate���ҽ�����Ѫ�����ķ�����ӵ�event��
        currentStates = GetComponent<CharacterStats>();

        currentStates.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    //������������ʱ�򴥷��ĺ���
    private void OnEnable()
    {
        //��ó����е�camera
        cam = Camera.main.transform;

        //������ǰ�����е�����Canvas
        foreach ( Canvas canvas in FindObjectsOfType<Canvas>() )
        {
            //�����ǰ��canvas��renderMode����Ⱦģʽ��ΪWorldSpace
            if(canvas.renderMode == RenderMode.WorldSpace)
            {
                //����prefab���Ҷ�λ��canvas��,Ȼ�󽫸����ɵ�Prefab��transform��ֵ��UIBar���������õ���Ѫ�����������
                UIBar = Instantiate(healthUIPrefab, canvas.transform).transform;

                //ȡ��UIBar�еĵ�һ��������( currentHealth��iamge��)
                healthSilder = UIBar.GetChild(0).GetComponent<Image>();

                //�����Ƿ�һֱ�ɼ�
                UIBar.gameObject.SetActive(healthBarAlwaysVisible);
            }
        }
    }

    //����Ѫ���ĺ���
    private void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        //�����ǰѪ��С�ڵ���0��������UIBar
        if (currentHealth <= 0) 
            Destroy(UIBar.gameObject);
        
        //ÿ���ܵ��˺���ʱ��Ѫ��һ���ǿɼ���
        UIBar.gameObject.SetActive(true);

        //UIʣ�µĿ��ӻ�ʱ�丳ֵ
        remainingTime = healthBarVisibleTime;

        //����Ѫ���������Ĵ�С��slider ��
        float sliderPercent = (float)currentHealth / maxHealth;
        healthSilder.fillAmount = sliderPercent;
    }

    /*LateUpdate���Դ��ĺ�����������һ֡��Ⱦ����ִ�У��������ƶ���Ȼ��Ѫ��UI���ϣ��������Ѫ����˸������
     * ���������Ӧ�������������
       Ѫ��UI����enemy�ƶ�
    */
    private void LateUpdate()
    {
        if(UIBar!=null)
        {
            UIBar.position = barPoint.position;

            //Ѫ����UI����Ϊ������� ��- forward������,��Ѫ����������
            UIBar.forward = -cam.forward;

            //��Ѫ��UI���ӻ�ʱ��<=0���Ҳ��ǳ�ʱ����ʾʱ��Ѫ��UI����Ϊ���ɼ�
            if (remainingTime <= 0 && !healthBarAlwaysVisible)
            {
                UIBar.gameObject.SetActive(false);
            }
            else
            {
                remainingTime -= Time.deltaTime;
            }
        }
    }
}
