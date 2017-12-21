using Com.ObscureProduction.OnTheEdge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomPlayerManager : Photon.PunBehaviour
{
    #region Private Variables
    int playerNumber;
    string playerName;
    GameObject thisChar;
    Vector3[] characterPos = new[] { new Vector3(-2.59f, -0.77f, -0.32f), new Vector3(-0.32f, -0.77f, 2.4f), new Vector3(2.65f, -0.77f, 3f), new Vector3(5.47f, -0.77f, 2.4f)};
    int amountOfPlayerConnected;
    #endregion

    #region Public Variables
    #endregion


    #region Monobehaviour Methods
    // Use this for initialization
    void Start () {
        amountOfPlayerConnected = PhotonNetwork.room.PlayerCount;
        playerName = photonView.owner.NickName;
        playerNumber = photonView.ownerId;
        if (photonView.isMine)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>().LobbySetup(playerNumber);
            if (playerNumber != 1)
            {
                StartCoroutine("SmallDelay");
            }
            else
            {
                photonView.RPC("PlayerDropIn", PhotonTargets.AllBuffered, playerNumber, playerName);
            }
        }
    }
    IEnumerator SmallDelay()
    {
        yield return new WaitForSeconds(1f);
        photonView.RPC("PlayerDropIn", PhotonTargets.AllBuffered, playerNumber, playerName);
    }
	
	// Update is called once per frame
	void Update () {

    }
    #endregion

    #region Custom Functions
    #endregion

    #region RPCs

    [PunRPC]
    public void PlayerDropIn(int playerID, string nickName)
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<Manager>().UpdatePlayerInfo(playerID,nickName);
    }
    #endregion

}
