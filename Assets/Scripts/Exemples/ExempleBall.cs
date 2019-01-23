using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExempleBall : MonoBehaviour
{
    [SerializeField] float acceleration = 5;
    [SerializeField] float speedForParticle = 5;
    [SerializeField] int recordFrame = 10;
    [SerializeField] VectorFieldGeneratorDrawer drawer;

    Rigidbody rb;
    List<Vector3> recordPositionList = new List<Vector3>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Move();
        ParticleEffect();
    }

    private void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
        if (movement.magnitude > 1)
            movement.Normalize();

        rb.AddForce(movement * acceleration);
    }

    private void ParticleEffect()
    {
        if (rb.velocity.magnitude >= speedForParticle)
        {
            //Check if on the plane
            if(drawer.InBound(transform.position))
                recordPositionList.Add(transform.position);

            if(recordPositionList.Count >= recordFrame)
            {
                //Send the list
                drawer.DrawPositions(recordPositionList);
                recordPositionList.Clear();
            }
        }
        else
        {
            recordPositionList.Clear();
        }
    }
}