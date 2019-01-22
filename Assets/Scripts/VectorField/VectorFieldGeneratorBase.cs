using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public abstract class VectorFieldGeneratorBase : MonoBehaviour
{
    // [SerializeField] VectorFunction function;
    //[SerializeField] protected int size = 12;
    [SerializeField] string name;
    [SerializeField] bool debugLogs;
    [SerializeField] bool drawGizmos;
    [SerializeField] bool rescaleVFX;
    [SerializeField] int optimizeRenderIndex;
    [SerializeField] protected Vector3 scale;
    [SerializeField] protected VisualEffect visualEffect;
    [SerializeField] TextureWrapMode wrapMode;
    protected Vector3[,,] vectorfield;
    Texture3D currentTexture;

    public void UpdateVectorField()
    {
        vectorfield = GenerateVectorField();
        RenderTo3DTexture(vectorfield);
    }

    private void OnDrawGizmos()
    {
        if (vectorfield != null && drawGizmos)
            ShowVectorField(vectorfield);
    }

    //Faire un texture3D updater

    [ContextMenu("Render To 3D Texture")]
    public void RenderTo3DTexture(Vector3[,,] vectorfield)
    {
        Texture3D texture = ConvertVectorFieldToTexture3D(vectorfield);

        texture.wrapMode = wrapMode;

#if UNITY_EDITOR
        if(currentTexture == null)
        UnityEditor.AssetDatabase.CreateAsset(texture, "Assets/" + name + ".asset");
#endif
        currentTexture = texture;


        if (rescaleVFX) {
            visualEffect.SetVector3("Size", scale);
            visualEffect.SetVector3("Spawn Size",  scale);
        }
        visualEffect.SetTexture("VectorField", texture);
    }

    protected abstract Vector3[,,] GenerateVectorField();

    public void ShowVectorField(Vector3[,,] vectorfield)
    {
        Vector3Int sizes = GetSizesVectorField(vectorfield);
        Vector3 middle = (Vector3)sizes * 0.5f;

        for (int x = 0; x < sizes.x; x++)
        {
            for (int y = 0; y < sizes.y; y++)
            {
                for (int z = 0; z < sizes.z; z++)
                {
                    if (ShouldSkipIndex(x, y, z, sizes))
                        continue;
  
                    Vector3 start = new Vector3(x, y, z) - middle;
                    Vector3 end = new Vector3(x, y, z) + vectorfield[x, y, z] - middle;
                    Color color = new Color((float)x / sizes.x, (float)y / sizes.y, (float)z / sizes.z);
                    Gizmos.color = color;

                    //Calculate position in world coord
                    Vector3 step = scale.ElementWiseDivision(sizes);
                    Vector3 realStart = transform.position + start.ElementWiseMultiplication(step) + step * .5f;
                    Vector3 realEnd = transform.position + end.ElementWiseMultiplication(step) + step * .5f;

                    //Show gizmos
                    Gizmos.DrawLine(
                       realStart,
                        realEnd);

                    Gizmos.DrawSphere(
                        realEnd,
                        (.1f * scale.magnitude) / ((Vector3)sizes).magnitude);
                }
            }
        }
    }

    Texture3D ConvertVectorFieldToTexture3D(Vector3[,,] vectorField)
    {
        Vector3Int sizes = GetSizesVectorField(vectorField);
        Color[] colorArray = new Color[sizes.x * sizes.y * sizes.z];
        Texture3D texture = new Texture3D(sizes.x, sizes.y, sizes.z, TextureFormat.RGBA32, true);
  
        for (int z = 0; z < sizes.z; z++)
        {
            for (int y = 0; y < sizes.y; y++)
            {
                for (int x = 0; x < sizes.x; x++)
                {
                    Color c = Vector3ToColor(vectorField[x, y, z]);
                    int i = GetIndex(x, y, z, sizes);
                    if(debugLogs)
                        Debug.Log(x + " , " + y + " , " + z + ", i " + i);
                    colorArray[i] = c;
                }
            }
        }

        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }

    Vector3Int GetSizesVectorField(Vector3[,,] vectorfield)
    {
        int sizeX = vectorfield.GetLength(0);
        int sizeY = vectorfield.GetLength(1);
        int sizeZ = vectorfield.GetLength(2);
        return new Vector3Int(sizeX, sizeY, sizeZ);
    }

    Color Vector3ToColor(Vector3 v)
    {
        //if -1 = 0,  if 0 = .5 , if 1 = 1
        // -1 + 1 / 2 = 0,  0+1 / 2 0.5
        Vector3 vv = (v + Vector3.one) * 0.5f;
        return new Color(vv.x, vv.y, vv.z, 1);
    }

    bool ShouldSkipIndex(int x, int y, int z, Vector3Int sizes)
    {
        int skipIndex = GetIndex(x, y, z, sizes);
        return (optimizeRenderIndex != 0 && skipIndex % (1 << optimizeRenderIndex) != 0);
    }

    int GetIndex(int x, int y, int z, int size)
    {
        return x + (y * size) + (z * size * size);
    }

    int GetIndex(int x, int y, int z, Vector3Int sizes)
    {
        return x + (y * sizes.x) + (z * sizes.x * sizes.y);
    }
}