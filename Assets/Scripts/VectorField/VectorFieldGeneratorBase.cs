using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public abstract class VectorFieldGeneratorBase : MonoBehaviour
{
    [Header("Editor param")]
    [SerializeField] string savingName = "Vectorfield";
    [SerializeField] bool debugLogs;
    [SerializeField] bool drawGizmos;
    [SerializeField] int optimizeRenderIndex;

    [Header("General params")]
    [SerializeField] bool rescaleVFX;
    [SerializeField] protected Vector3 scale = Vector3.one * 10;

    [Header("Visual effects")]
    [SerializeField] protected VisualEffect visualEffect;
    [SerializeField] TextureWrapMode wrapMode;

    protected Vector3[,,] vectorfield;
    protected Texture3D currentTexture;

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

    [ContextMenu("Render To 3D Texture")]
    public void RenderTo3DTexture(Vector3[,,] vectorfield)
    {
        Texture3D texture = ConvertVectorFieldToTexture3D(vectorfield);

        texture.wrapMode = wrapMode;

#if UNITY_EDITOR
        if(currentTexture == null)
        UnityEditor.AssetDatabase.CreateAsset(texture, "Assets/" + savingName + ".asset");
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

    protected void AffectTexture3DWithPositionDictionary(Texture3D texture, Dictionary<Vector3Int, Vector3> positionDictionary, Vector3[,,] vectorfield)
    {
        Color[] colorArray = texture.GetPixels();
        foreach (Vector3Int discretePosition in positionDictionary.Keys)
        {
            Vector3 vector = positionDictionary[discretePosition];
            Color c = Vector3ToColor(vector);
            int x = discretePosition.x;
            int y = discretePosition.y;
            int z = discretePosition.z;
            int i = GetIndex(x, y, z, GetSizesVectorField(vectorfield));
            colorArray[i] = c;  
        }
        texture.SetPixels(colorArray);
        texture.Apply();
    }

    protected void InitializeVectorFieldInward(Vector3[,,] vectorfield, float inwardStrength, bool normalized = true)
    {
        Vector3Int size = new Vector3Int(vectorfield.GetLength(0), vectorfield.GetLength(1), vectorfield.GetLength(2));
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                for (int k = 0; k < size.z; k++)
                {
                    Vector3 pos = new Vector3(
                        i - size.x * 0.5f,
                        j - size.y * 0.5f,
                        k - size.z * 0.5f
                        );

                    if (normalized)
                        pos = pos.normalized;

                    pos *= inwardStrength / size.magnitude; 
                    vectorfield[i, j, k] = -pos;
                }
            }
        }
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

    public struct VectorAt3DIndex
    {
        public Vector3 vector;
        public Vector3Int index;
    }
}