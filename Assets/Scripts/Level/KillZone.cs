using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Character>())
		{
			other.GetComponent<Character>().Die();
		}
	}
}
