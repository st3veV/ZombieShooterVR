using UnityEngine;

public class UserDataComponent : MonoBehaviour
{

    private UserData _userData;

	void Start ()
	{
	    _userData = UserData.Instance;
	}
	
	void Update () {
	
	}

    public int Score
    {
        get { return _userData.Score; }
    }

    public void IncreaseScore(int amount)
    {
        _userData.IncreaseScore(amount);
    }
}

public class UserData
{
    private static UserData _instance;
    private int _score = 0;

    public static UserData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new UserData();
            return _instance;
        }
    }

    public int Score
    {
        get { return _score; }
        private set { _score = value; }
    }

    public void IncreaseScore(int amount)
    {
        Score += amount;
    }

    public void ResetScore()
    {
        Score = 0;
    }
}
