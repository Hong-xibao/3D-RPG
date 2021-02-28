using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;


public class MouseManager : Singleton<MouseManager>
{
    

    RaycastHit hitInfo;//保存射线碰撞到的物体的相关信息

    public Texture2D point, doorway, attack, target, arrow;

    //创建event
    public event Action<Vector3> OnMouseClicked;
    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture();

        if (InteractWithUI()) return;

        MouseControl();
        
    }

    //设置鼠标的贴图
    void SetCursorTexture()
    {
        //返回一个从摄像机发出到屏幕中的一个点的射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //如果射线ray与任何碰撞体相交，则返回true，如果为true，则hit info将包含有光碰撞体撞击位置的更多信息。
        if (Physics.Raycast(ray,out hitInfo))
        {
            //切换鼠标贴图
            switch(hitInfo.collider.gameObject.tag)
            {
                 /*
                  * 当碰撞的物体为Ground时
                    1.设置cursor为target。
                    2.因为设置图片为32*32所以设置偏移Vector2(16,16)。
                    3.CursorMode.Auto意思是：自动却换鼠标的模式。
                 */
                case "Ground":
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break; 
                case "Item":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
             
                default:
                    Cursor.SetCursor( arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }    
        }
    }

    //鼠标的控制
    void MouseControl()
    {
        //鼠标左键点击并且射线在有碰撞的时候
        if(Input.GetMouseButtonDown(0)&&hitInfo.collider!=null)
        {
            //射线碰撞到的物体的Tag为Ground时
            if(hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                //“？”表示？前为空不报错，不为空就执行Invoke
                //hitInfo.point为射线命中碰撞体的撞击点（vector3)
                //鼠标点击后，就会执行所有加入到OnMouseClicked里面的这些函数方法。
                OnMouseClicked?.Invoke(hitInfo.point);
              
            }

            //当射线碰撞到的物体的Tag为Enemy时，执行OnEnemyClicked中的所有函数
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }  
            if (hitInfo.collider.gameObject.CompareTag("Item"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

        }
    }

    //判断是否在与UI面板有互动
    bool InteractWithUI()
    {
        //判断是否是跟UI互动，并且下面得有物品
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else return false;
    }
}
