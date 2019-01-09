using System.Collections.Generic;
using UnityEngine;

public static class Vector234Extensions
{
	// Vector2
	public static Vector2 SetX(this Vector2 aVec, float aXValue)
	{
		aVec.x = aXValue;
		return aVec;
	}
	public static Vector2 SetY(this Vector2 aVec, float aYValue)
	{
		aVec.y = aYValue;
		return aVec;
	}

	public static Vector2 SetAbsolute(this Vector2 vec)
	{
		vec.x = Mathf.Abs(vec.x);
		vec.y = Mathf.Abs(vec.y);
		return vec;
	}
	// Vector3
	public static Vector3 SetX(this Vector3 aVec, float aXValue)
	{
		aVec.x = aXValue;
		return aVec;
	}
	public static Vector3 SetY(this Vector3 aVec, float aYValue)
	{
		aVec.y = aYValue;
		return aVec;
	}
	public static Vector3 SetZ(this Vector3 aVec, float aZValue)
	{
		aVec.z = aZValue;
		return aVec;
	}

	public static Vector3 SetAbsolute(this Vector3 vec)
	{
		vec.x = Mathf.Abs(vec.x);
		vec.y = Mathf.Abs(vec.y);
		vec.z = Mathf.Abs(vec.z);
		return vec;
	}

	// Vector4
	public static Vector4 SetX(this Vector4 aVec, float aXValue)
	{
		aVec.x = aXValue;
		return aVec;
	}
	public static Vector4 SetY(this Vector4 aVec, float aYValue)
	{
		aVec.y = aYValue;
		return aVec;
	}
	public static Vector4 SetZ(this Vector4 aVec, float aZValue)
	{
		aVec.z = aZValue;
		return aVec;
	}
	public static Vector4 SetW(this Vector4 aVec, float aWValue)
	{
		aVec.w = aWValue;
		return aVec;
	}
	//Color
	public static Color Clamp(this Color color)
	{
		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);
		color.a = Mathf.Clamp01(color.a);

		return color;
	}
	public static Color ClampMaxAlpha(this Color color)
	{
		color.r = Mathf.Clamp01(color.r);
		color.g = Mathf.Clamp01(color.g);
		color.b = Mathf.Clamp01(color.b);
		color.a = 1;
		
		return color;
	}
	public static float GetLuminance(this Color color)
	{
		return (0.2126f * color.r) + (0.7152f * color.g) + (0.722f * color.b);
	}
	public static int Minimum(this int number, int min)
	{
		if(number < min)
			number = min;
		return number;
	}

	public static float Minimum(this float number, float min)
	{
		if(number < min)
			number = min;
		return number;
	}
    public static float Randomise(this float value, float randomiseRatio)
    {
        float ratio = value * randomiseRatio;
        return value + Random.Range(-ratio, ratio);
    }

	public static Transform FindHierarchyChild(this Transform tr, string name)
	{
		return RecursionFindHierarchyChild(tr,name);
	}
	static Transform RecursionFindHierarchyChild(Transform tr, string name)
	{
		if(tr.name == name)
		{
			return tr;
		}
		Transform find;
		for (int i = 0; i < tr.childCount; i++)
		{
			find = RecursionFindHierarchyChild(tr.GetChild(i),name);
			if(find != null)
				return find;
		}
		return null;
	}

    public static Vector3 RoundDown(this Vector3 vector, int decimalPlaces)
    {
        return new Vector3(vector.x.RoundDown(decimalPlaces), vector.y.RoundDown(decimalPlaces), vector.z.RoundDown(decimalPlaces));
    }

    public static float RoundDown(this float number, int decimalPlaces)
    {
        return Mathf.Floor(number * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
    }

    public static decimal Map (this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        for (var i = 0; i < list.Count; i++)
            list.Swap(i, Random.Range(i, list.Count));
    }

    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }

    public static float SymmetricEvaluate(this AnimationCurve animationCurve, float t)
    {
        float t2 = t * 2;
        return (t2 <= 1) ? animationCurve.Evaluate(t2) : animationCurve.Evaluate(2 - t2);
    }

    //Still need work
    public static float SymmetricEvaluate(this AnimationCurve animationCurve, float t, float offset)
    {
        offset *= 2;

        if (t <= offset)
        {
            t = t * (1 / offset);

            return animationCurve.SymmetricEvaluate(t);
        }
        else
        {
            t = t * (1 / (1-offset));

            return animationCurve.SymmetricEvaluate(t);
        }
    }
}
[System.Serializable]
public struct Vector3Bool
{
    public bool x;
    public bool y;
    public bool z;

    public static implicit operator bool(Vector3Bool v3Bool)
    {
        return v3Bool.x || v3Bool.y || v3Bool.z;
    }
}

[System.Serializable]
public struct Vector3AtIndex
{
    public Vector3 min;
    public Vector3 max;
    public int index;

    private Vector3 vector;

    public void Randomise()
    {
        vector = new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    public void SetVectorValue(Vector3 vector)
    {
        this.vector = vector;
    }

    public Vector3 GetVectorValue()
    {
        return vector;
    }

    public static Vector3 GetVectorAtTimeWithIndex(Vector3AtIndex[] vector3AtIndex, int hiddenVariableIndex)
    {
        for (int i = 0; i < vector3AtIndex.Length; i++)
        {
            if (vector3AtIndex[i].index == hiddenVariableIndex)
                return vector3AtIndex[i].vector;
        }
        return Vector3.zero;
    }

    public static int[] GetAllIndex(Vector3AtIndex[] vector3AtIndex)
    {
        int[] indexes = new int[vector3AtIndex.Length];
        for (int i = 0; i < vector3AtIndex.Length; i++)
        {
            indexes[i] = vector3AtIndex[i].index;
        }
        return indexes;
    }
}