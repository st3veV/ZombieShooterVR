using UnityEngine;
using UnityEngine.UI;

public class ScoreVisualizer : MonoBehaviour
{

    private UserData _userData;
    public Text TextOutput;

	// Use this for initialization
	void Start ()
	{
	    _userData = UserData.Instance;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    TextOutput.text = "" + _userData.Score;
	}
}
