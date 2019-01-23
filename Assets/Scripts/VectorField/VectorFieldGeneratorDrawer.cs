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

    [SerializeField] float decayFactor = 1;

    [Header("Insert the surface here")]
    [SerializeField] BoxCollider surfaceBoxCollider;

    Vector3 centerPosition;

    Dictionary<Vector3Int, Vector3> positionDictionary = new Dictionary<Vector3Int, Vector3>();
    bool hasChanged = false;


    /// <summary>
    /// Logique de l'algo
    /// Si on veut draw des trucs, on rajoute tout dans un Dictionary
    /// Avec comme key la position normalizé et discretisé (x,y,z) pour accédé au vectorfield[,,]
    /// Et la value est le vecteur à la position (x,y,z)
    /// 
    /// Dans l'update on réduit le vecteur à chaque case dans le dictionary, pour évité de calculé sur tous les cases de la grilles
    /// Si une case a un vecteur avec une magnitude proche de 0, on le remove du dictionary
    /// </summary>


    private void Update()
    {
        if (hasChanged)
        {
            CalculateVectorField();
            hasChanged = false;
        }
        if (positionDictionary.Count != 0)
        { 
            ReduceVectorOptimized(decayFactor);
        }
    }

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
        Dictionary<Vector3Int, Vector3> newPosDictionary = GetPositionsDictionary(positions);

        if (propagationSize > 0)
            AffectSurrounding(newPosDictionary, propagationSize, vectorPropagationRate);

        foreach (Vector3Int discretPos in newPosDictionary.Keys)
        {
            AddInDictionary(positionDictionary, discretPos, newPosDictionary[discretPos]);
        }
    }

    public void CalculateVectorField()
    {
        vectorfield = GenerateVectorField();

        if (currentTexture == null)
            RenderTo3DTexture(vectorfield);
        else
            AffectTexture3DWithPositionDictionary(currentTexture, positionDictionary, vectorfield);
    }

    public void ResetField()
    {
        vectorfield = null;
        positionDictionary.Clear();
        CalculateVectorField();
    }

    public bool InBound(Vector3 position)
    {
        Vector3Int discretPosition = CalculateDiscretizedPosition(position);
        return InBound(discretPosition);
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

    public void AddVectorAtPosition(Vector3Int discretePosition, Vector3 vector)
    {
        AddInDictionary(positionDictionary, discretePosition, vector);
        vectorfield[discretePosition.x, discretePosition.y, discretePosition.z] += vector;
    }

    void ReduceVectorOptimized(float ratio)
    {
        List<Vector3Int> removeList = new List<Vector3Int>();
        List<Vector3Int> keys = new List<Vector3Int>();
        foreach (Vector3Int discretePos in positionDictionary.Keys)
        {
            int x = discretePos.x;
            int z = discretePos.z;

            //Clamp max
            if (vectorfield[x, 0, z].magnitude > maxMagnitude)
            {
                vectorfield[x, 0, z] = vectorfield[x, 0, z].normalized * maxMagnitude;
            }

            Vector3 vectorToRemove = (vectorfield[x, 0, z].normalized * maxMagnitude) * ratio * Time.deltaTime;
            Vector3 vectoreBeforeRemove = vectorfield[x, 0, z];

            //Save key to update dictionary outside of foreach
            keys.Add(discretePos);

            //Crossed 0
            if (vectorToRemove.magnitude >= vectoreBeforeRemove.magnitude)
            {
                vectorfield[x, 0, z] = Vector3.zero;
                removeList.Add(discretePos);
            }
            else
            {
                vectorfield[x, 0, z] -= vectorToRemove;
            }
        }
        //Update dictionary outside of foreach
        foreach(Vector3Int discretePos in keys)
        {
            positionDictionary[discretePos] = vectorfield[discretePos.x, 0, discretePos.z];
        }

        foreach (Vector3Int vectorToRemove in removeList)
        {
            positionDictionary.Remove(vectorToRemove);
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

        Vector3 middle = new Vector3(gridResolution.x * 0.5f, 0, gridResolution.y * 0.5f);

        //Broken avec le dictionary
        //AffectSurrounding(vectorfield, positionDictionary);
        CalculateVectorfield(vectorfield, positionDictionary);

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

    private Dictionary<Vector3Int, Vector3> GetPositionsDictionary(List<Vector3> positions)
    {
        Dictionary<Vector3Int, Vector3> dictionary = new Dictionary<Vector3Int, Vector3>();
        int length = positions.Count - 1;
        for (int i = 0; i < length; i++)
        {
            Vector3 vector = (positions[i + 1] - positions[i]).normalized;
            Vector3Int discretePosition = CalculateDiscretizedPosition(positions[i]);
            AddInDictionary(dictionary, discretePosition, vector);
        }
        return dictionary;
    }
   
    private void AffectSurrounding(Dictionary<Vector3Int, Vector3> newPositionDictionary, int propagationSize, float vectorPropagationRate)
    {
        //Pour chaque direction
        List<Vector3Int> newPositions = new List<Vector3Int>();
        List<Vector3> newVectors = new List<Vector3>();
        foreach (Vector3Int discretePos in newPositionDictionary.Keys)
        { 
            int x = discretePos.x;
            int z = discretePos.z;
            Vector3 direction = newPositionDictionary[discretePos];

            //calculer les surroundings 
            for (int j = -propagationSize; j <= propagationSize; j++)
            {
                for (int k = -propagationSize; k <= propagationSize; k++)
                {
                    if (j == k)
                        continue;

                    int xx = x + j;
                    int zz = z + k;
                    //In bound
                    if (xx >= 0 && xx < gridResolution.x && zz >= 0 && zz < gridResolution.y)
                    {
                        float diff = Mathf.Abs(j) + Mathf.Abs(k);
                        Vector3 vector = direction * (vectorPropagationRate / diff);

                        //Add dans la liste pour pas modifié l'état du dictionary dans le foreach
                        newPositions.Add(new Vector3Int(xx,0,zz));
                        newVectors.Add(vector);
                    }
                }
            }
        }

        for (int i = 0; i < newPositions.Count; i++)
        {
            //Add dans le dictionaire pour affecter le vectorfield plus tard
            AddInDictionary(newPositionDictionary, newPositions[i], newVectors[i]);
        }
    }

    private void CalculateVectorfield(Vector3[,,] vectorfield, Dictionary<Vector3Int, Vector3> dictionaryPositions)
    {
        foreach (Vector3Int discretePos in dictionaryPositions.Keys)
        {
            Vector3 direction = dictionaryPositions[discretePos];
            if (direction.y != 0)
            {
                direction = direction.SetY(0);
            }
            int x = discretePos.x;
            int z = discretePos.z;

            //Clamp max
            if (direction.magnitude > maxMagnitude)
            {
                direction = direction.normalized * maxMagnitude;
            }
            vectorfield[x, 0, z] = direction;
        }
    }

    void AddInDictionary(Dictionary<Vector3Int, Vector3> dict, Vector3Int discretePosition, Vector3 vector)
    {
        hasChanged = true;
        if (dict.ContainsKey(discretePosition))
        {
            //The final value will be normalized by the decay function
            dict[discretePosition] += vector;
        }
        else
        {
            dict.Add(discretePosition, vector);
        }
    }
}
