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
    [SerializeField] float scale;
    [SerializeField] bool rescaleVFX;
    [SerializeField] int optimizeRenderIndex;
    [SerializeField] VisualEffect visualEffect;
    [SerializeField] TextureWrapMode wrapMode;
    protected Vector3[,,] vectorfield;


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
        //vectorfield = GenerateVectorField(size);
        Texture3D texture = ConvertVectorFieldToTexture3D(vectorfield);

        texture.wrapMode = wrapMode;

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(texture, "Assets/" + name + ".asset");
#endif

        if (rescaleVFX) {
            visualEffect.SetVector3("Size", Vector3.one * scale);
            visualEffect.SetVector3("Spawn Size", Vector3.one * scale);
        }
        visualEffect.SetTexture("VectorField", texture);
    }

    protected abstract Vector3[,,] GenerateVectorField();

    public void ShowVectorField(Vector3[,,] vectorfield)
    {
        Vector3Int sizes = GetSizesVectorField(vectorfield);
        Vector3 middle = (Vector3)sizes * 0.5f;//Vector3.one * size * 0.5f;
        for (int x = 0; x < sizes.x; x++)
        {
            for (int y = 0; y < sizes.y; y++)
            {
                for (int z = 0; z < sizes.z; z++)
                {
                    if (ShouldSkipIndex(x, y, z, sizes))
                        continue;

                    if (vectorfield[x, y, z] == Vector3.zero)
                        continue;


                    Vector3 start = new Vector3(x, y, z) - middle;
                    Vector3 end = new Vector3(x, y, z) + vectorfield[x, y, z] - middle;
                    Color color = new Color((float)x / sizes.x, (float)y / sizes.y, (float)z / sizes.z);
                    Gizmos.color = color;
                    Gizmos.DrawLine(
                        (start * scale).ElementWiseDivision(sizes),
                        (end * scale).ElementWiseDivision(sizes));

                    Gizmos.DrawSphere(
                        (end * scale).ElementWiseDivision(sizes),
                        (.1f * scale) / ((Vector3)sizes).magnitude);
                }
            }
        }
    }

    Texture3D ConvertVectorFieldToTexture3D(Vector3[,,] vectorField) //, int size)
    {
        //test remove size;
        Vector3Int sizes = GetSizesVectorField(vectorField);
        //Color[] colorArray = new Color[size * size * size];
        //Texture3D texture = new Texture3D(size, size, size, TextureFormat.RGBA32, true);

        //for (int x = 0; x < size; x++)
        //{
        //    for (int y = 0; y < size; y++)
        //    {
        //        for (int z = 0; z < size; z++)
        //        {
        //            Color c = Vector3ToColor(vectorField[x, y, z]);
        //            colorArray[GetIndex(x, y, z, size)] = c;
        //        }
        //    }
        //}
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