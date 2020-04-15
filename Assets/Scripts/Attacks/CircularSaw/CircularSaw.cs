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
        Invoke("DestroySaw", 5);
    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(transform.up * RotationSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }

    void DestroySaw()
    {
        Destroy(gameObject);
    }
}
