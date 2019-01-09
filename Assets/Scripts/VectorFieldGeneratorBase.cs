using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public abstract class VectorFieldGeneratorBase : MonoBehaviour
{
   // [SerializeField] VectorFunction function;
    [SerializeField] protected int size = 12;
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
        vectorfield = GenerateVectorField(size);
        RenderTo3DTexture(vectorfield);
    }

    private void OnDrawGizmos()
    {
        if(vectorfield != null && drawGizmos)
            ShowVectorField(vectorfield);
    }

    //Faire un texture3D updater

    [ContextMenu("Render To 3D Texture")]
    public void RenderTo3DTexture(Vector3[,,] vectorfield)
    {
        //vectorfield = GenerateVectorField(size);
        Texture3D texture = ConvertVectorFieldToTexture3D(vectorfield, size);

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

    protected abstract Vector3[,,] GenerateVectorField(int size);

    public void ShowVectorField(Vector3[,,] vectorField)
    {
        Vector3 middle = Vector3.one * size * 0.5f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    
                    if (ShouldSkipIndex(x,y,z,size))
                        continue;

                    if (vectorField[x, y, z] == Vector3.zero)
                        continue;

                    Vector3 start = new Vector3(x, y, z) - middle;
                    Vector3 end = new Vector3(x, y, z) + vectorField[x, y, z] - middle;
                    Color color = new Color((float)x / size, (float)y / size, (float)z / size);
                    Gizmos.color = color;
                    Gizmos.DrawLine(start * scale / size, end * scale / size);
                    Gizmos.DrawSphere(end * scale / size, .1f * scale / size);
                }
            }
        }
    }

    Texture3D ConvertVectorFieldToTexture3D(Vector3[,,] vectorField, int size)
    {
        Color[] colorArray = new Color[size * size * size];
        Texture3D texture = new Texture3D(size, size, size, TextureFormat.RGBA32, true);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    //if -1 = 0,  if 0 = .5 , if 1 = 1
                    // -1 + 1 / 2 = 0,  0+1 / 2 0.5
                    Vector3 v = vectorField[x, y, z];
                    Vector3 vv = (v + Vector3.one) * 0.5f;
                    Color c = new Color(vv.x, vv.y, vv.z, 1);

                    if (debugLogs)
                    { 
                        Debug.Log(v);
                        Debug.Log(vv);
                        Debug.Log(c);
                        Debug.Log("---");
                    }
                    colorArray[GetIndex(x, y, z, size)] = c;
                }
            }
        }
        texture.SetPixels(colorArray);
        texture.Apply();
        return texture;
    }

    bool ShouldSkipIndex(int x, int y, int z, int size)
    {
        int skipIndex = GetIndex(x, y, z, size);
        return (optimizeRenderIndex != 0 && skipIndex % (1 << optimizeRenderIndex) != 0);
    }

    int GetIndex(int x, int y, int z, int size)
    {
        return x + (y * size) + (z * size * size);
    }
}