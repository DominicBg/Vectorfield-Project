using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGeneratorDrawer : VectorFieldGeneratorBase
{
    [SerializeField] Vector2Int gridResolution = new Vector2Int(10,10);

    [Header("Options")]
    //number of grid around the selected that will be affected
    [SerializeField] int propagationSize = 0;
    //Each surrounding grid will be affected by how much (ratio 0,1)
    [SerializeField] float vectorPropagationRate = 0.1f;
    //Max magnitude of each vector
    [SerializeField] float maxMagnitude = 2;

    [Header("Insert the surface here")]
    [SerializeField] BoxCollider surfaceBoxCollider;

    Vector3 centerPosition;
    List<Vector3> positions = new List<Vector3>();
    List<Vector3Int> discretizedPositions = new List<Vector3Int>();

    private void OnValidate()
    {
        ResizeGrid();
    }

    private void ResizeGrid()
    {
        centerPosition = surfaceBoxCollider.transform.position;

        Vector3 realScale = surfaceBoxCollider.size.ElementWiseMultiplication(surfaceBoxCollider.transform.localScale);

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
            discretizedPosition.x >= 0 && discretizedPosition.x < gridResolution.x &&
            discretizedPosition.z >= 0 && discretizedPosition.z < gridResolution.y;
    }

    public Vector3Int CalculateDiscretizedPosition(Vector3 position)
    {
        Vector3 sizeV3 = new Vector3(gridResolution.x, 0, gridResolution.y);
        
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
        for (int x = 0; x < gridResolution.x; x++)
        {
            for (int z = 0; z < gridResolution.y; z++)
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
            vectorfield = new Vector3[gridResolution.x, 1, gridResolution.y];
            InitializeField(vectorfield);
            return vectorfield;
        }

        Vector3[] directions = CalculateDirections();
        Vector3Int[] discretePositions = discretizedPositions.ToArray();

        Vector3 middle = new Vector3(gridResolution.x * 0.5f, 0, gridResolution.y * 0.5f);

        CalculateVectorfield(vectorfield, directions, discretePositions);
        AffectSurrounding(vectorfield, directions, discretePositions);

        return vectorfield;
    }

    private void InitializeField(Vector3[,,] vectorfield)
    {
        for (int x = 0; x < gridResolution.x; x++)
        {
            for (int y = 0; y < gridResolution.y; y++)
            {
                vectorfield[x, 0, y] = Vector3.zero;
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

            int x = discretePositions[i].x;
            int z = discretePositions[i].z;
            vectorfield[x, 0, z] = directions[i];
        }
    }

    private void AffectSurrounding(Vector3[,,] vectorfield, Vector3[] directions, Vector3Int[] discretePositions)
    {
        //Pour chaque direction
        for (int i = 0; i < directions.Length; i++)
        {
            int x = discretePositions[i].x;
            int y = discretePositions[i].z;
            Vector3 direction = directions[i];

            //calculer les surroundings 
            for (int j = -propagationSize; j <= propagationSize; j++)
            {
                for (int k = -propagationSize; k <= propagationSize; k++)
                {
                    if (j == k)
                        continue;

                    int xx = x + j;
                    int yy = y + k;
                    //In bound
                    if (xx >= 0 && xx < gridResolution.x && yy >= 0 && yy < gridResolution.y)
                    {
                        float diff = Mathf.Abs(j) + Mathf.Abs(k);
                        vectorfield[xx, 0, yy] += direction * (vectorPropagationRate / diff);
                    }
                }
            }
        }
    }

}
