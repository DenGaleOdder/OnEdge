using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.ObscureProduction.OnTheEdge
{

    public class PlayerControl : Photon.PunBehaviour, IPunObservable
    {
        #region Private Variables
        GameObject playerInfoHolder;
        Rigidbody playerRB;
        Animator playerAnimations;
        bool isMoving;
        bool isFiring;
        bool coroutineRunning;
        float speed = 5;
        float cooldown;
        bool beingHit;
        int aniStateControl;
        #endregion

        #region Public Variables
        public GameObject attackBall;
        public Transform attackSpawnPos;
        public static GameObject LocalPlayerInstance;
        #endregion

        #region Monobehavior Methods

        void Awake()
        {
            // #Important
            // used in Manager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerControl.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(this.gameObject);
        }

        // Use this for initialization
        void Start()
        {
            gameObject.name = "P"+photonView.ownerId;
            playerInfoHolder = GameObject.Find("Player" + photonView.ownerId);
            string texToApply = playerInfoHolder.GetComponent<LobbyPlayer>().texToApply;
            playerRB = gameObject.GetComponent<Rigidbody>();
            playerAnimations = gameObject.GetComponent<Animator>();
            cooldown = 0;


            Renderer objToApplyTexTo = gameObject.GetComponentInChildren<Renderer>();
            objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply, typeof(Texture2D)) as Texture2D);
        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.isMine)
            {
                    DefaultAttack();
            }
            // might have to introduce a new bool "onCooldown" and place the timer inside photonView.isMine & add the new bool in the last condition.
            
        }

        private void FixedUpdate()
        {
            if (photonView.isMine == false && PhotonNetwork.connected == true)
            {
                return;
            }
            else
            {
                Moving();
                Rotation();
            }
        }
#endregion

        #region Movement & Rotation
        void Moving()
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            if (beingHit)
            {
                aniStateControl = 3;
                playerAnimations.SetInteger("StateControl", aniStateControl);
                playerRB.AddForce(moveX*10,0,moveZ*10);
            }
            else
            {
                if (moveX == 0 && moveZ == 0)
                {
                    aniStateControl = 0;
                }
                else
                {
                    aniStateControl = 1;
                }
                playerAnimations.SetInteger("StateControl", aniStateControl);
                playerRB.velocity = new Vector3(moveX*speed, playerRB.velocity.y, moveZ*speed);


            }
            /*else
            {
                //playerRB.velocity = Vector3.zero;
                playerAnimations.SetBool("isMoving", false);
            }*/
        }
        void Rotation()
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitdist = 0.0f;

            // If the ray is parallel to the plane, Raycast will return false.
            if (playerPlane.Raycast(ray, out hitdist))
            {
                // Get the point along the ray that hits the calculated distance.
                Vector3 targetPoint = ray.GetPoint(hitdist);
                // Determine the target rotation.  This is the rotation if the transform looks at the target point.
                Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
                targetRotation *= Quaternion.Euler(0, 90, 0);
                // Smoothly rotate towards the target point.
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }
        #endregion

        #region Attacks

        void DefaultAttack()
        {
            if(cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
               photonView.RPC("BulletSpawn", PhotonTargets.All);
                cooldown = 2f;
            }
        }

        void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(aniStateControl);
            }
            else
            {
                // Network player, receive data
                this.aniStateControl = (int)stream.ReceiveNext();
            }
        }


        #endregion

        #region RPC's

        [PunRPC]
        public void BulletSpawn()
        {
            var ball = (GameObject)Instantiate(attackBall, attackSpawnPos.position, attackSpawnPos.rotation);

            // Add velocity to the bullet
            ball.GetComponent<Rigidbody>().velocity = ball.transform.forward * 8;

            // Destroy the bullet after 2 seconds
            Destroy(ball, 2.0f);
        }

        [PunRPC]
        public void GotHit(Vector3 directionToPush)
        {
            StartCoroutine("BeingPushed",directionToPush);
        }

        [PunRPC]
        public void SetPlayerColour(string texToApply)
        {
            Renderer objToApplyTexTo = gameObject.GetComponentInChildren<Renderer>();
            objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply, typeof(Texture2D)) as Texture2D);
        }
        #endregion

        #region Coroutines
        IEnumerator BeingPushed(Vector3 directionToPush)
        {
            beingHit = true;
            playerRB.velocity = Vector3.zero;
            playerRB.AddForce(directionToPush);
            yield return new WaitForSeconds(1f);
            beingHit = false;
        }
        #endregion
    }
}

