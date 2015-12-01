using UnityEngine;
using UnityEngine.UI;

public class ScoreVisualizer : MonoBehaviour
{

    private UserData _userData;
    public Text TextOutput;
    
	void Start ()
	{
	    _userData = UserData.Instance;
	}
	
	void Update ()
	{
	    TextOutput.text = _userData.Score.ToString();
	}
}
