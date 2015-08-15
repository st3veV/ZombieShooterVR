using UnityEngine;
using System.Collections;

public class EditorFireTrigger : MonoBehaviour {

    public GameObject gun;
    private Gun gunInternal;

    void Start()
    {
        gunInternal = gun.GetComponent<Gun>();
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown("x"))
        {
            gunInternal.Fire();
        }
    }
}
