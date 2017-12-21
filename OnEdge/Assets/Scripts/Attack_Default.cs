using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ObscureProduction.OnTheEdge
{

    public class Attack_Default : Photon.PunBehaviour
    {
        Vector3 directionToPush;
        // Use this for initialization
        void Start()
        {
            directionToPush = gameObject.GetComponent<Rigidbody>().velocity * 45;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<PhotonView>().RPC("GotHit", PhotonTargets.All, directionToPush);

                if (gameObject != null)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
