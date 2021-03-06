﻿using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class Initializer : MonoBehaviour
    {

        public List<GameObject> DontDestroyGameObjects;

        void Start()
        {
            Controller c = Controller.Instance;
            Debug.Log("Controller stated: " + c.name);
            SetDontDestroys();

        }
        
        private void SetDontDestroys()
        {
            foreach (GameObject dontDestroyGameObject in DontDestroyGameObjects)
            {
                DontDestroyOnLoad(dontDestroyGameObject);
            }
        }
    }
}