using Radar;
using Shooting;
using UnityEngine;

namespace Controllers
{
    public class PlayerController : Singleton<PlayerController>
    {
        public Inventory Inventory;
        public LifetimeComponent Lifetime;
        public Gun Gun;
        public MyoHandler MyoHandler;
        public Transform PlayerTransform;

        private void Awake()
        {
            Debug.Log("player controller awake");
            var player = Instantiate(Resources.Load("Prefabs/Player") as GameObject);
            player.name = "Player";
            player.transform.SetParent(gameObject.transform);
            PlayerTransform = player.transform;
            Lifetime = player.GetComponent<LifetimeComponent>();
            Inventory = player.GetComponent<Inventory>();
            Gun = player.GetComponentInChildren<Gun>();
            MyoHandler = player.GetComponentInChildren<MyoHandler>();

            var radarController = RadarController.Instance;
            var head = player.transform.FindChild("CardboardMain").FindChild("Head").gameObject;
            radarController.SetCenterObject(head);

            var shootingController = ShootingController.Create();
            shootingController.SetGun(Gun);
            shootingController.transform.SetParent(transform);
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