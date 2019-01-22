using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGeneratorFunction : VectorFieldGeneratorBase
{
    [SerializeField] VectorFunction function;
    [SerializeField] int size;
    private void OnValidate()
    {
        UpdateVectorField();
    }

    protected override Vector3[,,] GenerateVectorField()
    {
        Vector3[,,] vectorField = new Vector3[size, size, size];
        Vector3 middle = size * Vector3.one * 0.5f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    Vector3 vector = (function.GetVector(new Vector3(x, y, z) - middle));
                    if (vector.magnitude > 1)
                        vector.Normalize();

                    vectorField[x, y, z] = vector;
                }
            }
        }
        return vectorField;
    }


}
