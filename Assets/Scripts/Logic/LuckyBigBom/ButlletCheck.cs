using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class ButlletCheck : MonoBehaviour
{
    [SerializeField]
    private Camera camera3D;
    [SerializeField]
    private GameObject Boms;
    //相机初始位置
    //private Vector3 initCameraPos;

    private int soldierNum = 0;
    private TagType tagType = TagType.None;

    private void Start()
    {
        //initCameraPos = Camera.main.transform.position;
    }

    //子弹碰撞检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("碰撞啦---" + collision.collider.name);
        gameObject.SetActive(false);
        ContactPoint2D[] points = collision.contacts;
        if (points.Length > 0)
        {
            Boms.transform.position = _2dPosTo3d(points[0].point);
            Boms.SetActive(true);
            ShakeScreen();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3);
            foreach (var cdl in colliders)
            {
                if (cdl.tag == TagType.Soldier.ToString())//士兵
                {
                    cdl.transform.DOLocalMoveY(162, 3);
                    soldierNum++;
                    tagType = TagType.Soldier;
                }
                else if (cdl.tag == TagType.Tower.ToString())//城堡
                {
                    soldierNum = 2;
                    tagType = TagType.Tower;
                }
                else if (cdl.tag == TagType.Wall.ToString())//城墙
                {
                    tagType = TagType.Wall;
                }
            }
        }
    }

    //2d坐标转3d
    Vector3 _2dPosTo3d(Vector3 pos)
    {
        Vector3 scr = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        scr.z = 0;
        scr.z = Mathf.Abs(camera3D.transform.position.z - pos.z + 11); //加10  为了让特性更大
        return camera3D.ScreenToWorldPoint(scr);

        //这种写法也可以
        //Vector3 worldPoint;
        //RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform as RectTransform,
        //    Camera.main.WorldToScreenPoint(pos),camera3D, out worldPoint);
        //return worldPoint;
    }

    //显示结束面板
    void ShowResultTable()
    {
        Boms.SetActive(false);
        object[] data = { soldierNum>=2, tagType };
        EventHandler.ExcuteEvent(EventHandlerType.Success, data);
        soldierNum = 0;
        tagType = TagType.None;
    }

    //震屏效果
    void ShakeScreen()
    {
        //设置主摄像机在2秒内 位置上和角度上 进项对应的变化，从而产生震动的效果
       // Camera.main.DOShakePosition(1.5f, new Vector3(10, 12, 0)).SetEase(Ease.Linear);
            //.OnComplete(() => Camera.main.transform.position = initCameraPos);
        Camera.main.DOShakeRotation(1.2f, new Vector3(12, 10, 0)).SetEase(Ease.Linear).OnComplete(ShowResultTable);
    }
}
