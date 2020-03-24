using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UILoadLevel : MonoBehaviour
{
	public TextMeshPro TextObject;
    // Start is called before the first frame update
    void Start()
    {
	    TextObject = GetComponent<TextMeshPro>();
	    TextObject.text = GameManager.Instance.GetCurrentLevel().ToString();
    }
}
