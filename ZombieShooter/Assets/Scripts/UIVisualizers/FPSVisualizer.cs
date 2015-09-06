using System;
using UnityEngine;
using UnityEngine.UI;

public class FPSVisualizer : MonoBehaviour {

    public Text FPSOutput;

	
	void Update ()
	{
	    FPSOutput.text = (int)(1/Time.deltaTime)+"";
	}
}
