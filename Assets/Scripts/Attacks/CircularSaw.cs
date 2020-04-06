using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularSaw : AttackObject
{
    public float RotationSpeed = 5.0f;
    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * RotationSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }
}
