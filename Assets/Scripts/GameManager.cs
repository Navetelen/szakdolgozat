using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class GameManager : MonoBehaviourPun
{
   [Header("Players")]
   public string playerPrefabPath;
   public TestPlayer[] players;
   public Transform[] spawnPoints;
   public Player photonPlayer;
    private int playersInGame;


   //instance
   public static GameManager instance;

   void Awake()
   {
       instance = this;
   }

   void Start()
   {
       players = new TestPlayer[PhotonNetwork.PlayerList.Length];
       photonView.RPC("ImInGame", RpcTarget.AllBuffered);
   }

   void Update()
   {

   }

   [PunRPC]
   void ImInGame(){
        playersInGame++;
        if(playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
   }

   void SpawnPlayer()
   {
       GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabPath, spawnPoints[0].position,Quaternion.identity );
   }

   public void OnBackToMenuButton(){
       NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Menu");
   }



}
