using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFieldGeneratorDrawer : VectorFieldGeneratorBase
{
    List<Vector3> positions = new List<Vector3>();
    List<Vector3> screenPositions = new List<Vector3>();

    [SerializeField] bool isRecording;

    [SerializeField] float delay = 0.1f;
    [SerializeField] float depth = 5;
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
            vectorfield = GenerateVectorField(size);
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

    protected override Vector3[,,] GenerateVectorField(int size)
    {
        Vector3[,,] vectorfield = new Vector3[size, size, size];

        Vector3 middle = Vector3.one * size * 0.5f;
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    vectorfield[x, y, z] = Vector3.down * 0.02f;// (middle - new Vector3(x, y, z)).SetZ(0) / size;
                }
            }
        }

        for (int i = 0; i < screenPositions.Count-1; i++)
        {
            Vector3 direction = (screenPositions[i + 1] - screenPositions[i]).normalized;

            //Discretize value
            float screenRatio = (float)Screen.height / Screen.width;
            int x = (int)(screenPositions[i].x / Screen.width * size);
            int y = (int)(screenPositions[i].y / Screen.height * size * screenRatio);

            vectorfield[x, y, 0] = direction;

        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                    vectorfield[x, y, z] = vectorfield[x, y, 0];
            }
        }
            /*
            //affect surrounding
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (j == k)
                        continue;

                    int xx = x + j;
                    int yy = y + k;
                    //In bound
                    if (xx >= 0 && xx < size && yy >= 0 && yy < size)
                    {
                        vectorfield[xx, yy, 0] += direction * 0.1f;
                    }
                }
            }*/
        return vectorfield;
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
