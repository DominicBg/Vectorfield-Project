using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinVectorField : VectorFunction
{
    [SerializeField] float scale;
    public override Vector3 GetVector(Vector3 point)
    {
        float perlinValue = Perlin3D(point * scale);
        float fullCirclevalue = perlinValue * 360;
        return GameMath.RotateVectorX(fullCirclevalue, point);
    }

    float Perlin3D(Vector3 point)
    {
        return Perlin3D(point.x, point.y, point.z);
    }

    float Perlin3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);

        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, z);
        float zx = Mathf.PerlinNoise(z, x);

        float xyz = xy + yz + xz + yx + zy + zx;
        return xyz / 6;
    }
}
