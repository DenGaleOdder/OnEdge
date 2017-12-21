using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.ObscureProduction.OnTheEdge
{
    public class Lobby_Content : Photon.PunBehaviour
    {
        #region Public Variables
        public Sprite[] characterImg;
        public GameObject[] characterModels;
        public Material[] characterColours;
        public GameObject controlPanel;
        public GameObject charSelectPanel;
        public GameObject charDetailsPanel;
        #endregion

        #region Private Variables
        GameObject myObject;
        GameObject currentChar;
        GameObject currentColour;
        GameObject infoPanel;
        int charListControl;
        #endregion

        private void Awake()
        {
            controlPanel = GameObject.Find("Control Panel");
            charSelectPanel = GameObject.Find("CharSelect Panel");
            charDetailsPanel = GameObject.Find("CharDetails Panel");
        }
        // Use this for initialization
        void Start()
        {
            myObject = GameObject.Find("Player" );
            controlPanel.SetActive(true);
            charSelectPanel.SetActive(false);
            charDetailsPanel.SetActive(false);
            currentChar = (GameObject)Instantiate(Resources.Load(characterModels[0].name));
            charListControl = 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CharSelectScreen()
        {
            controlPanel.SetActive(false);
            charSelectPanel.SetActive(true);
            charDetailsPanel.SetActive(true);
            //charPreview.transform.rotation *= Quaternion.Euler(0, 180, 0);
        }

        public void NextChar()
        {
            if (charListControl <= 3)
            {
                charListControl += 1;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name));
            }
            else
            {
                charListControl = 0;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name));
            }
        }

        public void PreviousChar()
        {
            if(charListControl >= 1)
            {
                charListControl -= 1;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name));
            }
            else
            {
                charListControl = 4;
                Destroy(currentChar);
                currentChar = (GameObject)Instantiate(Resources.Load(characterModels[charListControl].name));
            }
        }

        public void PickCharacter()
        {
            controlPanel.SetActive(true);
            charSelectPanel.SetActive(false);
            charDetailsPanel.SetActive(false);
            
        }
    }
}
