using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGeneratorDrawer : VectorFieldGeneratorBase
{
    List<Vector3> positions = new List<Vector3>();
    List<Vector3Int> discretizedPositions = new List<Vector3Int>();

    [SerializeField] Vector2Int sizes;
    [SerializeField] bool isRecording;

    [SerializeField] float vectorPropagationRate = 0.1f;
    [SerializeField] int propagationSize = 1;
    [SerializeField] bool invertYZ;
  
    [SerializeField] float maxMagnitude = 2;
    [SerializeField] BoxCollider surfaceBoxCollider;

    Vector3 centerPosition;


    private void OnValidate()
    {
        ResizeGrid();
    }

    private void ResizeGrid()
    {
        centerPosition = surfaceBoxCollider.transform.position;

        Vector3 realScale = new Vector3(
            surfaceBoxCollider.size.x * surfaceBoxCollider.transform.localScale.x,
            surfaceBoxCollider.size.y * surfaceBoxCollider.transform.localScale.y,
            surfaceBoxCollider.size.z * surfaceBoxCollider.transform.localScale.z
            );
        scale = realScale;
        visualEffect.SetVector3("Size", realScale);
        visualEffect.SetVector3("Spawn Size", realScale);
    }

    public void DrawPositions(List<Vector3> positions)
    {
        discretizedPositions.Clear();
        this.positions = positions;
        for (int i = 0; i < positions.Count; i++)
        {
            Vector3Int discretPosition = CalculateDiscretizedPosition(positions[i]);
            discretizedPositions.Add(discretPosition);
        }
    }

    public void CalculateVectorField()
    {
        vectorfield = GenerateVectorField();
        RenderTo3DTexture(vectorfield);
    }

    public void ResetField()
    {
        vectorfield = null;
        positions.Clear();
        discretizedPositions.Clear();
        CalculateVectorField();
    }

    public bool InBound(Vector3Int discretizedPosition)
    {
        return
            discretizedPosition.x >= 0 && discretizedPosition.x < sizes.x &&
            discretizedPosition.z >= 0 && discretizedPosition.z < sizes.y;
    }

    public Vector3Int CalculateDiscretizedPosition(Vector3 position)
    {
        Vector3 sizeV3 = new Vector3(sizes.x, 0, sizes.y);
        
        //Take diff * scale / size and normalize it
        Vector3 scaledDiff = (position - centerPosition)
            .ElementWiseDivision(scale)
            .ElementWiseMultiplication(sizeV3);

        Vector3 normalizedPoint = scaledDiff + sizeV3 * 0.5f;

        return new Vector3Int((int)normalizedPoint.x, 0, (int)normalizedPoint.z);
    }

    public void AddVectorAtPosition(Vector3Int position, Vector3 force)
    {
        vectorfield[position.x, position.y, position.z] += force;
    }

    public void ReduceVector(float ratio)
    {
        for (int x = 0; x < sizes.x; x++)
        {
            for (int z = 0; z < sizes.y; z++)
            {
                if (vectorfield[x, 0, z] == Vector3.zero)
                    continue;
                
                //clamp max
                if (vectorfield[x, 0, z].magnitude > maxMagnitude)
                    vectorfield[x, 0, z] = vectorfield[x, 0, z].normalized * maxMagnitude;

                vectorfield[x, 0, z] -= vectorfield[x, 0, z].normalized * ratio * Time.deltaTime;

                //clamp zero
                if (vectorfield[x, 0, z].magnitude < 0.01f)
                    vectorfield[x, 0, z] = Vector3.zero;
            }
        }
        RenderTo3DTexture(vectorfield);
    }

    protected override Vector3[,,] GenerateVectorField()
    {
        if (vectorfield == null)
        {
            vectorfield = InitializeField();
            InitializeField(vectorfield);
            return vectorfield;
        }

        Vector3[] directions = CalculateDirections();
        Vector3Int[] discretePositions = discretizedPositions.ToArray();

        Vector3 middle = GetNewVector3(sizes.x * 0.5f, sizes.y * 0.5f);

        CalculateVectorfield(vectorfield, directions, discretePositions);
        AffectSurrounding(vectorfield, directions, discretePositions);

        return vectorfield;
    }

    Vector3[,,] InitializeField()
    {
        return (invertYZ) ? new Vector3[sizes.x, 1, sizes.y] : new Vector3[sizes.x, sizes.y, 1];
    }

    Vector3 GetNewVector3(float x, float y)
    {
        return (invertYZ) ? new Vector3(x, 0, y) : new Vector3(x, y, 0);
    }

    void InsertInVectorfield(Vector3[,,] vectorfield, int x, int y, Vector3 v)
    {
        if (invertYZ)
            vectorfield[x, 0, y] = v;
        else
            vectorfield[x, y, 0] = v;
    }

    void AdditionToVectorfield(Vector3[,,] vectorfield, int x, int y, Vector3 v)
    {
        if (invertYZ)
            vectorfield[x, 0, y] += v;

        else
            vectorfield[x, y, 0] += v;
    }

    private void InitializeField(Vector3[,,] vectorfield)
    {
        for (int x = 0; x < sizes.x; x++)
        {
            for (int y = 0; y < sizes.y; y++)
            {
                InsertInVectorfield(vectorfield, x, y, Vector3.zero);
            }
        }
    }

    private Vector3[] CalculateDirections()
    {
        int length = positions.Count - 1;
        Vector3[] directions = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            directions[i] = (positions[i + 1] - positions[i]).normalized;
        }
        return directions;
    }

    private void CalculateVectorfield(Vector3[,,] vectorfield, Vector3[] directions, Vector3Int[] discretePositions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            if (directions[i].y != 0)
            {
                directions[i] = directions[i].SetY(0);
            }
            Debug.Log(discretePositions[i]);
            vectorfield[discretePositions[i].x, 0, discretePositions[i].z] = directions[i];
        }
    }

    private void AffectSurrounding(Vector3[,,] vectorfield, Vector3[] directions, Vector3Int[] discretePositions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            int x = discretePositions[i].x;
            int y = discretePositions[i].z;
            Vector3 direction = directions[i];

            for (int j = -propagationSize; j <= propagationSize; j++)
            {
                for (int k = -propagationSize; k <= propagationSize; k++)
                {
                    if (j == k)
                        continue;

                    int xx = x + j;
                    int yy = y + k;
                    //In bound
                    if (xx >= 0 && xx < sizes.x && yy >= 0 && yy < sizes.y)
                    {
                        float diff = Mathf.Abs(j) + Mathf.Abs(k);
                        vectorfield[xx, 0, yy] += direction * (vectorPropagationRate / diff);
                    }
                }
            }
        }
    }

}
