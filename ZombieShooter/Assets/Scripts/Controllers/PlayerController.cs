using UnityEngine;

namespace Controllers
{
    public class PlayerController : Singleton<PlayerController>
    {
        public Inventory Inventory;
        public LifetimeComponent Lifetime;
        public Gun Gun;
        public MyoHandler MyoHandler;

        private void Awake()
        {
            Debug.Log("player controller awake");
            GameObject player = Instantiate(Resources.Load("Prefabs/Player") as GameObject);
            player.name = "Player";
            player.transform.SetParent(gameObject.transform);
            Lifetime = player.GetComponent<LifetimeComponent>();
            Inventory = player.GetComponent<Inventory>();
            Gun = player.GetComponentInChildren<Gun>();
            MyoHandler = player.GetComponentInChildren<MyoHandler>();
        }

        private void Start()
        {

        }

        private void Update()
        {

        }

        public void Reset()
        {
            UserData.Instance.ResetScore();
            Inventory.Reset();
            Lifetime.Reset();
            Gun.Reset();
        }
    }
}