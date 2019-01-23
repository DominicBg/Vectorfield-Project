using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFunction : VectorFunction
{
    [SerializeField] float rotateX = 90;
    [SerializeField] float rotateY = 90;
    [SerializeField] float rotateZ = 90;

    public override Vector3 GetVector(Vector3 point)
    {
        if (point.x == 0 && point.z == 0)
            return Vector3.zero;

        Vector3 newV = point;
        if(rotateX != 0)
            newV = newV.RotateVectorX(rotateX);

        if (rotateY != 0)
            newV = newV.RotateVectorY(rotateY);

        if (rotateZ != 0)
            newV = newV.RotateVectorZ(rotateZ);

        return newV;
    }
}
