using Controllers;
using UnityEngine;
using UnityEngine.UI;

public class FPSVisualizer : MonoBehaviour {

    public Text FPSOutput;

    void Start()
    {
        EventManager.Instance.AddUpdateListener(OnUpdate);
    }
	
	private void OnUpdate()
	{
	    FPSOutput.text = (int)(1/Time.deltaTime)+"";
	}
}
