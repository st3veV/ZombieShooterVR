using UnityEngine;

public class UserController : MonoBehaviour
{

    public Inventory UserInventory;
    public LifetimeComponent UserLifetime;
    public Gun UserGun;

	void Start () {
	    
	}
	
	void Update () {
	
	}

    public void Reset()
    {
        UserData.Instance.ResetScore();
        UserInventory.Reset();
        UserLifetime.Reset();
        UserGun.Reset();
    }
}
