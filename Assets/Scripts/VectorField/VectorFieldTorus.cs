using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldTorus : VectorFieldGeneratorBase
{
    [SerializeField] int innerResolution;
    [SerializeField] int outerResolution;

    [SerializeField] float innerRadius;
    [SerializeField] float outerRadius;

    [SerializeField] Vector3 axis;
    [SerializeField] Vector3 stuff;
    [SerializeField] int size;
    [SerializeField] float innerStrength = .3f;
    [SerializeField] int numberThickness = 3;
    [SerializeField] float padding = 10;
    // Start is called before the first frame update
    void Start()
    {



    }

    [ContextMenu("Generate field")]
    private void GenerateTorus()
    {
        vectorfield = new Vector3[size, size, size];
        InitializeVectorFieldInward(vectorfield, innerStrength, false);

        for (int i = 0; i < innerResolution; i++)
        {
            float innerAngle = 360f / innerResolution * i;


            for (int k = 0; k < numberThickness; k++)
            {
                float t = (float)(k+1) / numberThickness;
                float currentOuterRadius = Mathf.Lerp(0, outerRadius, t);
                float currentInnerRadius = Mathf.Lerp(0, innerRadius, t);

                Vector3 innerPts = transform.position + GameMath.RotateAroundAxisDeg(currentInnerRadius * Vector3.right, axis.normalized, innerAngle);
                Vector3 diff = (innerPts - transform.position).normalized;


                //Vector3 previousPoint = Vector3.zero;

                for (int j = 0; j < outerResolution; j++)
                {
                    float outterAngle = 360f / outerResolution * j;

                    Vector3 tangeantAxis = Vector3.Cross(axis.normalized, diff).normalized;
                    Vector3 pointdir = GameMath.RotateAroundAxisDeg(diff * currentOuterRadius, tangeantAxis, outterAngle);
                    Vector3 point = innerPts + pointdir;

                    if (j != 0)
                    {
                        //Vector3 direction = (point - previousPoint).normalized;

                        Vector3Int index = GetDiscretPosition(point, size);
                        vectorfield[index.x, index.y, index.z] = Vector3.Cross(tangeantAxis, pointdir).normalized;
                    }

                    //previousPoint = point;
                }
            }
 
        }
        RenderTo3DTexture(vectorfield);
    }

    Vector3Int GetDiscretPosition(Vector3 position, int size)
    {
        float r = innerRadius + outerRadius + padding;
        //Le torus est centré 0,0,0, on le offset pour qui soit entre 0 et 2r
        position += Vector3.one * r;
        //On divise par le max qui est 2r, pour avoir des values entre 0 et 1
        position /= 2 * r;
        //On multiplie par la gridsize pour avoir entre 0 et size
        position *= size;
        return new Vector3Int((int)(position.x), (int)(position.y), (int)(position.z)); ;
    }

    protected override Vector3[,,] GenerateVectorField()
    {
        throw new System.NotImplementedException();
    }
}
