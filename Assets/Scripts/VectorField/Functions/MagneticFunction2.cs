using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticFunction2 : VectorFunction
{
    [SerializeField] float scale;
    [SerializeField] float circleDistance;
    [SerializeField] float ySquish = 1;
    [SerializeField] float strengthOverDistance;
    public override Vector3 GetVector(Vector3 point)
    {
        point.y = point.y * ySquish;
        float halfSize = scale * size / 2;

        Vector3 middle = new Vector3(point.x, 0, point.z).normalized * halfSize * circleDistance;
        Vector3 diff = middle - point;
        Vector3 gradient = Vector3.Cross(diff.normalized, new Vector3(-point.z, 0, point.x)) * diff.magnitude / strengthOverDistance / scale;
        return gradient;
    }

}
