using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    private Character CharacterInstance;
    // Start is called before the first frame update
    void Start()
    {
        CharacterInstance = transform.parent.GetComponent<Character>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CharacterInstance.SetIsGrounded(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CharacterInstance.SetIsGrounded(false);
    }
}
