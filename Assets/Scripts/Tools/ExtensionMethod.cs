using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod 
{
    private const float dotThreshold = 0.5f;

    //this后面跟着的类是要扩展的类，如下面的this后面的是Transform类是要扩展的类，逗号之后是要调用的变量
   public static bool IsFacingTarget(this Transform transform,Transform target)
   {
        var vectorToTarget = target.position - transform.position;
        vectorToTarget.Normalize();

        float dot = Vector3.Dot( transform.forward , vectorToTarget );

        return dot > dotThreshold;//判断，如果dot>0.5,就表明player在enemy攻击范围内，返回true
   } 
}
