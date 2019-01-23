using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] Transform rotateAround;
    [SerializeField] float speed = 5;

    [SerializeField] VectorFieldGeneratorDrawer drawer;

    Rigidbody rb;
    List<Vector3> recordPositionList = new List<Vector3>();

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(rotateAround.position, transform.up, Time.deltaTime * speed);
    }

    private void FixedUpdate()
    {
        ParticleEffect();
    }

    private void ParticleEffect()
    {
        //Check if on the plane
        if (drawer.InBound(transform.position))
            recordPositionList.Add(transform.position);

        if (recordPositionList.Count == 2)
        {
            //Send the list
            drawer.DrawPositions(recordPositionList);
            recordPositionList.Clear();
        }
    }
}
