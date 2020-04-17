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
        Vector3 direction = Launcher.transform.right;
        transform.Translate(transform.right * Time.deltaTime);                                       
        transform.Rotate(0f, 0f, direction.x *RotationSpeed *360* Time.deltaTime);
    }

    void DestroySaw()
    {
        Destroy(gameObject);
    }
}
