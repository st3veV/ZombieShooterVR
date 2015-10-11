using UnityEngine;
using UnityEngine.UI;

public class ScoreVisualizer : MonoBehaviour
{

    public UserDataComponent UserData;
    public Text TextOutput;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update ()
	{
	    TextOutput.text = "" + UserData.Score;
	}
}
