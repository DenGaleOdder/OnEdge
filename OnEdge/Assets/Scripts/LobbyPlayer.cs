using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.ObscureProduction.OnTheEdge
{

    public class LobbyPlayer : Photon.PunBehaviour
    {
        #region Private variables
        string playerName;
        int playerNumber;
        int amountOfPlayerConnected;
        GameObject infoPanel;
        GameObject gameManager;
        GameObject[] readyButtons = new GameObject[4];
        bool ready;
    
        GameObject currentChar;
        string[] colour = new string[6] {"Red","Green","Blue","Yellow","Purple","Black"};
        string nameOfPrefab; // used to instantiate correct character on when game is started.

        int charListControl; //controlling which of prefabs from the "characterModels" array we want. 
        int colourListControl; //controlling which of the 6 colours from the "colour" array we want.
        int materialListControl; // Stupid way of getting the name of the texture folder e.g. Bird or Classic. (Change this to using a string array containing the names).
        int previewImageNumber; // controlling which of the images from the "characterImages" array we want.
        #endregion

        #region Public variables
        public static GameObject LocalPlayerInstance;
        public Sprite[] characterImages;
        public GameObject[] characterModels;
        public Material[] characterColours; // changes this to a strgin array and declare all the elements to be the names of character types corrosponding to the texture folder names.
        public GameObject controlPanel;
        public GameObject charSelectPanel;
        public GameObject charDetailsPanel;
        // used when arena is loaded.
        public Vector3[] spawnLocations = new[] { new Vector3(5f, 5f, 5f), new Vector3(-5f, 5f, -5f), new Vector3(5f, 5f, -5f), new Vector3(-5f, 5f, 5f) };
        public string texToApply = "N/A";
        #endregion

        #region Monobehaviour Methods
        // Use this for initialization

        void Awake()
        {
            // #Important
            // used in Manager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerControl.LocalPlayerInstance = this.gameObject;
                controlPanel = GameObject.Find("Control Panel");
                charSelectPanel = GameObject.Find("CharSelect Panel");
                charDetailsPanel = GameObject.Find("CharDetails Panel");
                gameManager = GameObject.FindGameObjectWithTag("GameManager");
            }
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            amountOfPlayerConnected = PhotonNetwork.room.PlayerCount;
            gameObject.name = "Player" + photonView.ownerId;
            //get the name of the user and store it.
            playerName = photonView.owner.NickName;
            playerNumber = photonView.ownerId;
            if (photonView.isMine)
            {
                gameManager.GetComponent<Manager>().playerNumber = photonView.ownerId;
                //Ensure that only the panels for the lobbyView is enabled.
                //Rename this object, so it corrospond to what player you are e.g. player1 or player2.
                for (int i = 0; i < 4; i++)
                {
                    readyButtons[i] = GameObject.Find("Ready Button" + (i));
                    if (i + 1 > PhotonNetwork.room.PlayerCount)
                    {
                        readyButtons[i].SetActive(false);
                    }
                }
                AddFunctionToButtons();
                controlPanel.SetActive(true);
                charSelectPanel.SetActive(false);
                charDetailsPanel.SetActive(false);
                //change image and name on to slot, this user possess. And also buffer it, so the next players that join has the new image and name.
                photonView.RPC("PlayerDropIn", PhotonTargets.AllBuffered, playerNumber, playerName);
            }
        }

        void Update()
        {
            if (amountOfPlayerConnected != PhotonNetwork.room.PlayerCount && photonView.isMine)
            {
                for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
                {
                    readyButtons[i].SetActive(true);
                }
                amountOfPlayerConnected = PhotonNetwork.room.PlayerCount;
            }
        }

        private void OnLevelWasLoaded(int level)
        {
            if(level != 0 && level != 1)
            {
                if(photonView.isMine)
                {
                    PhotonNetwork.Instantiate(nameOfPrefab, spawnLocations[playerNumber - 1], Quaternion.identity, 0);
                }
            }
        }
        #endregion

        #region Character Selection Methods
        public void CharSelectScreen()
        {
            if(currentChar != null)
            {
                Destroy(currentChar);
            }
            controlPanel.SetActive(false);
            charSelectPanel.SetActive(true);
            charDetailsPanel.SetActive(true);
            currentChar = (GameObject)Instantiate(Resources.Load(characterModels[0].name),new Vector3(0,-0.7f,0), Quaternion.Euler(0,270,0));
            charListControl = 0;
            materialListControl = 0; // changes this to the array thing.
            colourListControl = 1;
        }
        //function called when the arraows are pressed in the character selection screen, whioch takes care of destroying the current char and instantiating a new one.
        //Also updates all the control variables.
        public void NextChar()
        {
            if (charListControl <= 3)
            {
                charListControl += 1;
                materialListControl += 1;
                colourListControl = 1;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name), new Vector3(0, -0.7f, 0), Quaternion.Euler(0, 270, 0));
            }
            else
            {
                charListControl = 0;
                materialListControl = 0;
                colourListControl = 1;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name), new Vector3(0, -0.7f, 0), Quaternion.Euler(0, 270, 0));
            }
        }

        public void PreviousChar()
        {
            if (charListControl >= 1)
            {
                charListControl -= 1;
                materialListControl -= 1;
                colourListControl = 1;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name), new Vector3(0, -0.7f, 0), Quaternion.Euler(0, 270, 0));
            }
            else
            {
                charListControl = 4;
                materialListControl = 4;
                colourListControl = 1;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name), new Vector3(0, -0.7f, 0), Quaternion.Euler(0, 270, 0));
            }
        }
        // Functions for changing texture i.e. colour of the current character being previewed.
        public void NextColour()
        {
            if (colourListControl <= 4)
            {
                colourListControl += 1;
                texToApply = "Textures/" + characterColours[materialListControl].name + "/" + colour[colourListControl];
                Renderer objToApplyTexTo = currentChar.GetComponentInChildren<Renderer>();
               objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply,typeof(Texture2D)) as Texture2D);
            }
            else
            {
                colourListControl = 0;
                texToApply = "Textures/" + characterColours[materialListControl].name + "/" + colour[colourListControl];
                Renderer objToApplyTexTo = currentChar.GetComponentInChildren<Renderer>();
                objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply, typeof(Texture2D)) as Texture2D);
            }
        }
        public void PreviousColour()
        {
            if (colourListControl >= 1)
            {
                colourListControl -= 1;
                texToApply = "Textures/" + characterColours[materialListControl].name + "/" + colour[colourListControl];
                Renderer objToApplyTexTo = currentChar.GetComponentInChildren<Renderer>();
                objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply, typeof(Texture2D)) as Texture2D);
            }
            else
            {
                colourListControl = 5;
                texToApply = "Textures/" + characterColours[materialListControl].name + "/" + colour[colourListControl];
                Renderer objToApplyTexTo = currentChar.GetComponentInChildren<Renderer>();
                objToApplyTexTo.material.SetTexture("_MainTex", Resources.Load(texToApply, typeof(Texture2D)) as Texture2D);
            }
        }
        // function called when player chooses a character.
        public void PickCharacter()
        {
            switch (charListControl)
            {
                case 0:
                    previewImageNumber = 1+colourListControl;
                    break;
                case 1:
                    previewImageNumber = 7 + colourListControl;
                    break;
                case 2:
                    previewImageNumber = 13 + colourListControl;
                    break;
                case 3:
                    previewImageNumber = 19 + colourListControl;
                    break;
                case 4:
                    previewImageNumber = 25 + colourListControl;
                    break;
            }
            controlPanel.SetActive(true);
            charSelectPanel.SetActive(false);
            charDetailsPanel.SetActive(false);
            nameOfPrefab = currentChar.name;
            nameOfPrefab = nameOfPrefab.Remove(nameOfPrefab.Length - 14);
            photonView.RPC("ChangeCharacterImage", PhotonTargets.AllBuffered,playerNumber, previewImageNumber);
            photonView.RPC("SetPlayerColour", PhotonTargets.AllBuffered, materialListControl, colourListControl);
        }
        #endregion

        void AddFunctionToButtons()
        {
            
            GameObject.Find("Character Select").GetComponent<Button>().onClick.AddListener(()=>CharSelectScreen());
            GameObject.Find("Select Button").GetComponent<Button>().onClick.AddListener(() => PickCharacter());
            GameObject.Find("Next Char").GetComponent<Button>().onClick.AddListener(() => NextChar());
            GameObject.Find("Previous Char").GetComponent<Button>().onClick.AddListener(() => PreviousChar());
            GameObject.Find("Next Colour").GetComponent<Button>().onClick.AddListener(() => NextColour());
            GameObject.Find("Previous Colour").GetComponent<Button>().onClick.AddListener(() => PreviousColour());
            readyButtons[playerNumber-1].GetComponent<Button>().onClick.AddListener(() => ReadyUp());
            
        }

        public void ReadyUp()
        {
            if (!ready)
            {
                byte colorR = 0;
                byte colorG = 255;
                ready = true;
                photonView.RPC("Ready", PhotonTargets.All, playerNumber, true, colorR, colorG, "Ready");
            }
            else
            {
                byte colorR = 192;
                byte colorG = 0;
                ready = false;
                photonView.RPC("Ready", PhotonTargets.All, playerNumber, false, colorR, colorG,"Not Ready");
            }
        }

        #region RPC's

        [PunRPC]
        public void PlayerDropIn(int playerNumber, string playerName)
        {
            GameObject.Find("Info_Player" + playerNumber).transform.GetChild(0).GetComponent<Text>().text = playerName;
            GameObject.Find("Info_Player" + playerNumber).transform.GetChild(1).GetComponent<Image>().sprite = characterImages[0];
        }

        [PunRPC]
        public void ChangeCharacterImage(int playerNumber, int imageNumber)
        {
            GameObject.Find("Info_Player" + playerNumber).transform.GetChild(1).GetComponent<Image>().sprite = characterImages[imageNumber];
        }

        [PunRPC]
        public void Ready(int player, bool readyState, byte colorR, byte colorG, string text)
        {
            GameObject readyButton = GameObject.Find("Ready Button" + (player - 1));
            readyButton.GetComponent<Image>().color = new Color32(colorR, colorG, 0, 130);
            readyButton.GetComponentInChildren<Text>().text = text;
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>().ReadyControl(player, readyState);
        }
        [PunRPC]
        public void SetPlayerColour(int mNumber, int cNumber)
        {
            texToApply = "Textures/" + characterColours[mNumber].name + "/" + colour[cNumber];
        }
        #endregion
    }
}
