using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 ElementWiseDivision(this Vector3 v1, Vector3Int v2)
    {
        return ElementWiseDivision(v1, (Vector3)v2);
    }

    public static Vector3 ElementWiseDivision(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
    }

    public static Vector3 ElementWiseMultiplication(this Vector3 v1, Vector3Int v2)
    {
        return ElementWiseMultiplication(v1, (Vector3)v2);
    }

    public static Vector3 ElementWiseMultiplication(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    }

    public static Vector3 SetY(this Vector3 aVec, float aYValue)
    {
        aVec.y = aYValue;
        return aVec;
    }

    public static Vector2 RotateVector(float angle, Vector2 point)
    {
        float a = angle * Mathf.PI / 180;
        float cosA = Mathf.Cos(a);
        float sinA = Mathf.Sin(a);
        Vector2 newPoint =
            new Vector2(
                (point.x * cosA - point.y * sinA),
                (point.x * sinA + point.y * cosA)
                );
        return newPoint;
    }

    public static Vector3 RotateVectorX(this Vector3 vector, float angle)
    {
        Vector2 vec = RotateVector(angle, new Vector2(vector.z, vector.y));
        return new Vector3(vector.x, vec.y, vec.x);
    }

    public static Vector3 RotateVectorZ(this Vector3 vector, float angle)
    {
        Vector2 vec = RotateVector(angle, new Vector2(vector.x, vector.y));
        return new Vector3(vec.y, vec.x, vector.z);
    }

    public static Vector3 RotateVectorY(this Vector3 vector, float angle)
    {
        Vector2 vec = RotateVector(angle, new Vector2(vector.x, vector.z));
        return new Vector3(vec.x, vector.y, vec.y);
    }
}
