using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
{
	private static T _instance;

	public static T Instance
	{
		get => _instance;
	}

	private bool _isPersistent = true;

	private void Awake()
	{
		if (_instance != null && _instance != this) 
		{ 
			Destroy(gameObject);
			return;
		}
		_instance = (T)FindObjectOfType(typeof(T));
		
		if (_isPersistent)
			DontDestroyOnLoad(gameObject);
	}
}
