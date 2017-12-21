using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace Com.ObscureProduction.OnTheEdge
{
    public class Manager : Photon.PunBehaviour
    {
        GameObject detailsPanel,dIndicator, charList,skinList,colourOutline;
        GameObject[] playerInfo = new GameObject[4];
        Vector3[] previewPos = new Vector3[4];
        GameObject[] characterChoices = new GameObject[4];
        string[] prefabNames = new string[] {"ClassicPreview", "BirdPreview", "CyborgPreview", "RobotPreview", "ZombiePreview"};
        string[] materialNames = new string[] {"Classic", "Bird", "Cyborg", "Robot", "Zombie"};
        string[] colourNames = new string[] {"Red", "Green", "Blue", "Yellow", "Purple", "Black"};
        #region Public Variables
        public int playerNumber;
        public GameObject playerPrefab;
        public GameObject playerPrefabLobby;
        public string chosenCharacter = "Random";
        public Vector3[] spawnLocations = new[] { new Vector3(5f, 5f, 5f), new Vector3(-5f, -5f, -5f), new Vector3(5f, 5f, -5f), new Vector3(-5f, -5f, 5f) };
        public bool[] readyState = new bool[4] { false,false,false,false};
        #endregion
        #region Private Variables
        int playersReady = 0;
#endregion

        #region Mono Methods
        private void Start()
        {
            detailsPanel = GameObject.FindGameObjectWithTag("PanelDetails");
            dIndicator = detailsPanel.transform.GetChild(5).gameObject;
            charList = detailsPanel.transform.GetChild(0).gameObject;
            skinList = detailsPanel.transform.GetChild(1).gameObject;
            skinList.SetActive(false);
            detailsPanel.SetActive(false);
            if (SceneManager.GetActiveScene().name != "Lobby")
            {
                if (playerPrefab == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
                }
                else
                {
                    if (PlayerControl.LocalPlayerInstance == null)
                    {
                        Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(chosenCharacter, spawnLocations[playerNumber-1], Quaternion.identity,0);
                    }
                    else
                    {
                        Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
                    }
                }
            }
            else
            {
                if (playerPrefab == null)
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
                }
                else
                {
                    if (PlayerControl.LocalPlayerInstance == null)
                    {
                        Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
                        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                        PhotonNetwork.Instantiate(this.playerPrefabLobby.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                    }
                    else
                    {
                        Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
                    }
                }
            }
        }
        #endregion

        #region Photon Messages


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }


        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                //LoadArena();
            }
        }

        #endregion


        #region Public Methods

        public void ReadyControl(int player, bool yesOrNo)
        {
            readyState[player-1] = yesOrNo;
            playersReady = 0;

            for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
            {
                if (readyState[i] == true)
                {
                    playersReady++;
                }
                else
                {
                    playersReady--;
                }
            }
            if (playersReady == PhotonNetwork.room.PlayerCount)
            {
                LoadArena();
            }
        }

            public void LeaveRoom()
            {
                PhotonNetwork.LeaveRoom();
            }


            #endregion

            #region Private Methods


            void LoadArena()
            {
                if (!PhotonNetwork.isMasterClient)
                {
                    Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                }
                Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
                PhotonNetwork.LoadLevel(levelName: "Room for " + PhotonNetwork.room.PlayerCount);
            }

            void PlayerJoinedLobbyScene()
            {

                if (!PhotonNetwork.isMasterClient)
                {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
                }
            
            }
            #endregion

        public void LobbySetup(int playerNumber)
        {
            switch (playerNumber)
            {
                case 1:
                    ImPlayer1();
                    break;
                case 2:
                    ImPlayer2();
                    break;
                case 3:
                    ImPlayer3();
                    break;
                case 4:
                    ImPlayer4();
                    break;
            }
            for(int i = 0; i<4;i++)
            {
                playerInfo[i].SetActive(false);
            }
        }
        public void UpdatePlayerInfo(int playerNumber, string nickName)
        {
            Debug.Log("player"+playerNumber +" is trying to find "+playerInfo[playerNumber-1].tag);
            playerInfo[playerNumber - 1].SetActive(true);
            characterChoices[playerNumber - 1] = (GameObject)Instantiate(Resources.Load("BlankCharacter"), previewPos[playerNumber - 1], Quaternion.Euler(0, 270, 0));
            playerInfo[playerNumber - 1].GetComponentInChildren<Text>().text = nickName;
        }

        void ImPlayer1()
        {
            playerNumber = 1;
            playerInfo[0] = GameObject.FindGameObjectWithTag("PanelP1");
            playerInfo[1] = GameObject.FindGameObjectWithTag("PanelP2");
            playerInfo[2] = GameObject.FindGameObjectWithTag("PanelP3");
            playerInfo[3] = GameObject.FindGameObjectWithTag("PanelP4");
            previewPos[0] = new Vector3(-2.59f, -0.77f, -0.32f);
            previewPos[1] = new Vector3(-0.32f, -0.77f, 2.4f);
            previewPos[2] = new Vector3(2.65f, -0.77f, 3f);
            previewPos[3] = new Vector3(5.47f, -0.77f, 2.4f);
        }
        void ImPlayer2()
        {
            playerNumber = 2;
            Debug.Log("ThisIsPlayer2 was executed by player" + playerNumber);
            playerInfo[0] = GameObject.FindGameObjectWithTag("PanelP2");
            playerInfo[1] = GameObject.FindGameObjectWithTag("PanelP1");
            playerInfo[2] = GameObject.FindGameObjectWithTag("PanelP3");
            playerInfo[3] = GameObject.FindGameObjectWithTag("PanelP4");
            previewPos[0] = new Vector3(-0.32f, -0.77f, 2.4f);
            previewPos[1] = new Vector3(-2.59f, -0.77f, -0.32f);
            previewPos[2] = new Vector3(2.65f, -0.77f, 3f);
            previewPos[3] = new Vector3(5.47f, -0.77f, 2.4f);
        }
        void ImPlayer3()
        {
            playerNumber = 3;
            Debug.Log("ThisIsPlayer3 was executed by player" + playerNumber);
            playerInfo[0] = GameObject.FindGameObjectWithTag("PanelP3");
            playerInfo[1] = GameObject.FindGameObjectWithTag("PanelP2");
            playerInfo[2] = GameObject.FindGameObjectWithTag("PanelP1");
            playerInfo[3] = GameObject.FindGameObjectWithTag("PanelP4");
            previewPos[0] = new Vector3(2.65f, -0.77f, 3f);
            previewPos[1] = new Vector3(-0.32f, -0.77f, 2.4f);
            previewPos[2] = new Vector3(-2.59f, -0.77f, -0.32f);
            previewPos[3] = new Vector3(5.47f, -0.77f, 2.4f);
        }
        void ImPlayer4()
        {
            playerNumber = 4;
            Debug.Log("ThisIsPlayer4 was executed by player" + playerNumber);
            playerInfo[0] = GameObject.FindGameObjectWithTag("PanelP4");
            playerInfo[1] = GameObject.FindGameObjectWithTag("PanelP2");
            playerInfo[2] = GameObject.FindGameObjectWithTag("PanelP3");
            playerInfo[3] = GameObject.FindGameObjectWithTag("PanelP1");
            previewPos[0] = new Vector3(5.47f, -0.77f, 2.4f);
            previewPos[1] = new Vector3(-0.32f, -0.77f, 2.4f);
            previewPos[2] = new Vector3(2.65f, -0.77f, 3f);
            previewPos[3] = new Vector3(-2.59f, -0.77f, -0.32f);
        }
        public void Details()
        {
            if(!detailsPanel.activeSelf)
            {
                detailsPanel.SetActive(true);
            }
            else
            {
                detailsPanel.SetActive(false);
            }
        }
        public void ShowSkins()
        {
            if(!skinList.activeSelf)
            {
                dIndicator.GetComponent<RectTransform>().position = new Vector2(0,-90);
                charList.SetActive(false);
                skinList.SetActive(true);
            }
        }
        public void ShowChars()
        {
            if(!charList.activeSelf)
            {
                dIndicator.GetComponent<RectTransform>().position = new Vector2(-350, -90);
                skinList.SetActive(false);
                charList.SetActive(true);
            }
        }
        public void ChooseCharacter(int charNumber)
        {
            photonView.RPC("ChangeCharacter", PhotonTargets.AllBuffered, charNumber, playerNumber);
        }

        public void ChooseSkin(int colourNum)
        {
            if(colourOutline != null)
            {
                colourOutline.GetComponent<Outline>().effectColor = new Color(255, 255, 255, 255);
            }
            colourOutline = EventSystem.current.currentSelectedGameObject;
            colourOutline.GetComponent<Outline>().effectColor = new Color(255, 193, 0, 255);
            photonView.RPC("ChangeSkin", PhotonTargets.AllBuffered, playerNumber, colourNum);
        }
        [PunRPC]
        public void ChangeCharacter(int prefabNumber, int playerNumber)
        {
            Destroy(characterChoices[playerNumber - 1]);
            characterChoices[playerNumber - 1] = (GameObject)Instantiate(Resources.Load(prefabNames[prefabNumber]), previewPos[playerNumber - 1], Quaternion.Euler(0, 270, 0));
            characterChoices[playerNumber - 1].GetComponent<PhotonView>().TransferOwnership(playerNumber);

        }
        [PunRPC]
        public void ChangeSkin(int playerNumber, int cNumber)
        {
            string materialName = characterChoices[playerNumber - 1].name.Remove(characterChoices[playerNumber - 1].name.Length - 14);
            string texToApply = "Textures/" + materialName + "/" + colourNames[cNumber];
            Debug.Log(texToApply);
            Renderer objToApplyTexTo = characterChoices[playerNumber - 1].GetComponentInChildren<Renderer>();
            objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply, typeof(Texture2D)) as Texture2D);
        }
    }
    }

