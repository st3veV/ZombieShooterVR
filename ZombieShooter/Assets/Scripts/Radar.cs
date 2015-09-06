using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class Radar : MonoBehaviour
{

    public GameObject EnemyAvatarPrefab;
    public GameObject LookAtGameObject;
    public GameObject ControlRotationGameObject;

    private List<GameObject> avatars;
    private Transform helperTransform;
    private float radarRadius = 10.0f;
    private int radarLayer;
    private int invisibleLayer;

    private GameObject[] enemies;

    private int timer = 0;

	// Use this for initialization
	void Start () {
	    avatars = new List<GameObject>();
        
        GameObject Helper = new GameObject("helper");
        Helper.transform.SetParent(transform);
	    helperTransform = Helper.transform;

	    radarLayer = LayerMask.NameToLayer("Radar");
        invisibleLayer = LayerMask.NameToLayer("Invisible");

	}

	// Update is called once per frame
	void Update ()
	{
        //update camera transform
	    Vector3 eulerAngles = LookAtGameObject.transform.rotation.eulerAngles;
	    helperTransform.rotation = Quaternion.Euler(90, eulerAngles.y, 0);
	    ControlRotationGameObject.transform.rotation = helperTransform.rotation;

	    UpdateTrackedObjects();

	}

    private void UpdateEnemySet()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    private void UpdateTrackedObjects()
    {
        UpdateEnemySet();
        int i = 0;
        GameObject avatar;
        Vector3 position;
        bool needsAvatar = false;
        foreach (GameObject o in enemies)
        {
            //Find proper position for avatar
            if (Vector3.Distance(o.transform.position, transform.position) > radarRadius)
            {
                needsAvatar = true;
                helperTransform.LookAt(o.transform);
                position = transform.position + radarRadius*helperTransform.forward;
            }
            else
            {
                needsAvatar = false;
                position = o.transform.position;
            }

            //get or create avatar and move it to position
            if (needsAvatar)
            {
                if (i < avatars.Count)
                {
                    avatar = avatars[i];
                    avatar.layer = radarLayer;
                    avatar.transform.position = position;
                }
                else
                {
                    avatar = Instantiate(EnemyAvatarPrefab, position, Quaternion.identity) as GameObject;
                    avatars.Add(avatar);
                }

                i++;
            }
        }

        // remove unnecessary avatars
        if (i < avatars.Count)
        {
            for (int j = i; j < avatars.Count; j++)
            {
                avatar = avatars[j];
                avatar.layer = invisibleLayer;
            }
        }
    }
}
