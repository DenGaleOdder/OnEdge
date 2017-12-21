using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ObscureProduction.OnTheEdge
{

    public class Test_Cannon : Photon.PunBehaviour
    {
        float cooldown;
        public GameObject attackBall;
        public Transform attackSpawnPos;
        // Use this for initialization
        void Start()
        {
            cooldown = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            else
            {
                var ball = (GameObject)Instantiate(attackBall, attackSpawnPos.position, attackSpawnPos.rotation);

                // Add velocity to the bullet
                ball.GetComponent<Rigidbody>().velocity = ball.transform.forward * 15;

                // Destroy the bullet after 2 seconds
                cooldown = 2f;
            }
        }
    }
}
