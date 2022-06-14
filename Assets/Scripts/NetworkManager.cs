using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers;

    //instance
    public static NetworkManager instance;

    void Awake(){
        if(instance != null && instance != this){
            gameObject.SetActive(false);
        }else{
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start(){
        //connect to master server
        //PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster(){
        PhotonNetwork.JoinLobby();
        Debug.Log("Joined Master Server");

    }

    
    public override void OnJoinedLobby(){
        Debug.Log("Joined Lobby: " + PhotonNetwork.CurrentLobby.Name);
        Debug.Log(PhotonNetwork.CurrentLobby);
    }
    

    public void CreateRoom ( string roomName){
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;


        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom ( string roomName){
        PhotonNetwork.JoinRoom(roomName);
    }

    
    public override void OnJoinedRoom(){
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log("Photon view: " + photonView);
    }

    
    [PunRPC]
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(sceneName);
    }
}
