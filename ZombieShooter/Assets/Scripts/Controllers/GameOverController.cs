using System;
using UnityEngine;
using System.Collections;

public class GameOverController : MonoBehaviour
{

    public GameObject PlayAgainTarget;
    public UserData UserData;
    public TextMesh ScoreOutput;

    private LifetimeComponent _playAgainLifetime;

    public event Action OnPlayAgain;

    private void Start()
    {
        AssignReferences();
    }

    private void AssignReferences()
    {
        UserData = UserData.Instance;

        Initialize();
    }

    public void Initialize()
    {
        _playAgainLifetime = PlayAgainTarget.GetComponent<LifetimeComponent>();
        _playAgainLifetime.OnDie += _playAgainLifetime_OnDie;

        ScoreOutput.text = string.Format("Score: {0}", UserData.Score);
    }

    private void _playAgainLifetime_OnDie(LifetimeComponent obj)
    {
        _playAgainLifetime.OnDie -= _playAgainLifetime_OnDie;
        OnOnPlayAgain();
    }

    protected virtual void OnOnPlayAgain()
    {
        var handler = OnPlayAgain;
        if (handler != null) handler();
    }
}
