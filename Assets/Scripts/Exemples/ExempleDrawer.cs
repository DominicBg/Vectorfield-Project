using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is an exemple of how to use VectorFieldGeneratorDrawer
/// It records positions and send them to VectorFieldGeneratorDrawer
/// </summary>
public class ExempleDrawer : MonoBehaviour
{
    float currentTime = 0;
    Camera mainCamera;

    [SerializeField] float rayDistance = 55;
    [SerializeField] LayerMask layerMaskFloor;
    [SerializeField] VectorFieldGeneratorDrawer drawer;
    [SerializeField] float decaySpeed;
    List<Vector3> positions = new List<Vector3>();

    void Awake()
    {
        mainCamera = Camera.main;
        drawer.ResetField();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SavePosition();
            ShowDirections();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ReleaseDraw();
        }

        if(Input.GetKeyDown(KeyCode.Space))
            positions.Clear();
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

    void ReleaseDraw()
    {
        drawer.DrawPositions(positions);
        positions.Clear();
    }

    void SavePosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, layerMaskFloor))
        {
            positions.Add(hit.point);
        }
    }
}
