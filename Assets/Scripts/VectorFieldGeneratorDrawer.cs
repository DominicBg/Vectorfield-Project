using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGeneratorDrawer : VectorFieldGeneratorBase
{
    List<Vector3> positions = new List<Vector3>();
    List<Vector3> screenPositions = new List<Vector3>();

    [SerializeField] Vector2Int sizes;
    [SerializeField] bool isRecording;

    [SerializeField] float delay = 0.1f;
    [SerializeField] float fallSpeed = 0.01f;
    [SerializeField] float vectorPropagationRate = 0.1f;
    [SerializeField] int propagationSize = 1;
    float depth = 5;
    float currentTime = 0;
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (isRecording)
        {
            if (Input.GetMouseButton(0) && IsReadyToTakeFrame())
            {
                SavePosition();
            }
            ShowDirections();
        }

        if (Input.GetMouseButton(1))
        {
            vectorfield = GenerateVectorField();
            RenderTo3DTexture(vectorfield);

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            positions.Clear();
            screenPositions.Clear();
        }
    }

    void SavePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 position = ray.origin + ray.direction * depth;

        screenPositions.Add(Input.mousePosition);
        positions.Add(position);
    }

    void ShowDirections()
    {
        for (int i = 1; i < positions.Count; i++)
        {
            Debug.DrawLine(
                positions[i - 1],
                positions[i]
                );
        }
    }

    protected override Vector3[,,] GenerateVectorField()
    {
        Vector3[,,] vectorfield = new Vector3[sizes.x, sizes.y, 1];

        Vector3[] directions = CalculateDirections();
        Vector3Int[] discretePositions = CalculateDiscretizedPositions();

        Vector3 middle = new Vector3(sizes.x * 0.5f, sizes.y * 0.5f, 0);

        InitializeField(vectorfield);

        CalculateVectorfield(vectorfield, directions, discretePositions);

        AffectSurrounding(vectorfield, directions, discretePositions);

        return vectorfield;
    }

    private void InitializeField(Vector3[,,] vectorfield)
    {
        for (int x = 0; x < sizes.x; x++)
        {
            for (int y = 0; y < sizes.y; y++)
            {
                vectorfield[x, y, 0] = Vector3.down * fallSpeed;// (middle - new Vector3(x, y, z)).SetZ(0) / size;
            }
        }
    }

    private Vector3[] CalculateDirections()
    {
        int length = screenPositions.Count - 1;
        Vector3[] directions = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            directions[i] = (screenPositions[i + 1] - screenPositions[i]).normalized;
        }
        return directions;
    }

    private Vector3Int[] CalculateDiscretizedPositions()
    {
        int length = screenPositions.Count - 1;
        Vector3Int[] discretePositions = new Vector3Int[length];
        for (int i = 0; i < length; i++)
        {
            float screenRatio = (float)Screen.height / Screen.width;
            int x = (int)(screenPositions[i].x / Screen.width * sizes.x);
            int y = (int)(screenPositions[i].y / Screen.height * sizes.y * screenRatio);
            discretePositions[i] = new Vector3Int(x, y, 0);
        }
        return discretePositions;
    }

    private static void CalculateVectorfield(Vector3[,,] vectorfield, Vector3[] directions, Vector3Int[] discretePositions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            int x = discretePositions[i].x;
            int y = discretePositions[i].y;

            vectorfield[x, y, 0] = directions[i];
        }
    }

    private void AffectSurrounding(Vector3[,,] vectorfield, Vector3[] directions, Vector3Int[] discretePositions)
    {
        for (int i = 0; i < directions.Length; i++)
        {
            int x = discretePositions[i].x;
            int y = discretePositions[i].y;
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
                        vectorfield[xx, yy, 0] += direction * vectorPropagationRate;
                    }
                }
            }
        }
    }

    bool IsReadyToTakeFrame()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            currentTime = delay;
            return true;
        }
        return false;
    }
}
