using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f;

    //this������ŵ�����Ҫ��չ���࣬�������this�������Transform����Ҫ��չ���࣬����֮����Ҫ���õı���
   public static bool IsFacingTarget(this Transform transform,Transform target)
   {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot( transform.forward , vectorToTarget );

        return dot > dotThreshold;//�жϣ����dot>0.5,�ͱ���player��enemy������Χ�ڣ�����true
   } 
}
