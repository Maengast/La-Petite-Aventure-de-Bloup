using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectArea : MonoBehaviour
{
    private Boss _boss;
    private void Start()
    {
        _boss = gameObject.GetComponentInParent<Boss>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
           
            _boss.HasDetectedVictim = true;
            Destroy(gameObject);
        }
    }
}
