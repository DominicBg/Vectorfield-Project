using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGeneratorFunction : VectorFieldGeneratorBase
{
    [SerializeField] VectorFunction function;
    [SerializeField] Vector3Int size;
    private void OnValidate()
    {
        UpdateVectorField();
    }

    protected override Vector3[,,] GenerateVectorField()
    {
        Vector3[,,] vectorField = new Vector3[size.x, size.y, size.z];
        Vector3 middle = (Vector3)size / 2;
        //function.size = size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int z = 0; z < size.z; z++)
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
