﻿using UnityEngine;

public class GameSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
	    Application.targetFrameRate = 60;
	}
	
}
