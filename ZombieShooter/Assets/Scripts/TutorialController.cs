using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour {

    public GameObject TutorialTarget;
    public MyoHandler MyoHandler;

    private LifetimeComponent _targetLifetime;

    public List<GameObject> TutorialInstructions;

	// Use this for initialization
	void Start () {
        TutorialTarget.SetActive(false);
	    _targetLifetime = TutorialTarget.GetComponent<LifetimeComponent>();
        _targetLifetime.OnDie += _targetLifetime_OnDie;
        HideTutorialInstructions();
	    TutorialStep1();
	}

    private void HideTutorialInstructions()
    {
        foreach (GameObject o in TutorialInstructions)
        {
            o.SetActive(false);
        }
    }

    private void TutorialStep1()
    {
        MyoHandler.OnMyoReset += MyoHandler_OnMyoReset;
        TutorialInstructions[0].SetActive(true);
    }

    void MyoHandler_OnMyoReset()
    {
        MyoHandler.OnMyoReset -= MyoHandler_OnMyoReset;
        HideTutorialInstructions();
        TutorialInstructions[1].SetActive(true);
        Debug.Log("Myo has be reset");
        TutorialTarget.SetActive(true);
    }

    void _targetLifetime_OnDie(LifetimeComponent obj)
    {
        Debug.Log("Tutorial end");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
