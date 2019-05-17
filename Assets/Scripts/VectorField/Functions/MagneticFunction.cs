using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticFunction : VectorFunction
{
    [SerializeField] float q = 1;
    [SerializeField] float scale = 1;
    [SerializeField] Vector3 offset;
    public override Vector3 GetVector(Vector3 point)
    {
        point += offset;
        point *= scale;

        float xx = point.x * point.x;
        float yy = point.y * point.y;
        float zz = point.z * point.z;
        float divisor = Mathf.Pow(xx + yy + zz, 5 / 2);

        float x = q * (-2*xx + yy + zz) / divisor;
        float y = q * (xx - 2*yy + zz) / divisor;
        float z = q * (xx + yy - 2*zz) / divisor;
        return new Vector3(x, y, z);
    }
}
