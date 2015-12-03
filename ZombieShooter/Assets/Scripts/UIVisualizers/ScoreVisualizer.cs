using Controllers;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVisualizer : MonoBehaviour
{

    private UserData _userData;
    public Text TextOutput;
    
	void Start ()
	{
	    _userData = UserData.Instance;
        EventManager.Instance.AddUpdateListener(OnUpdate);
	}
	
	private void OnUpdate()
	{
	    TextOutput.text = _userData.Score.ToString();
	}
}
