using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerController : MonoBehaviourPun
{
    [Header("Resources")]
    public int stock;
    public int scrolls;
    public int gold;
    public int player1FinalScore;
    public int player2FinalScore;

    [Header("Unit Info")]
    public int currentWave;
    public int maxWaveCount;
    public int unitsAlive;

    [Header("Enemy Resources")]
    public int enemyStock;
    public int enemyScrolls;
    public int enemyGold;

    [Header("Unit Info")]
    public int enemyCurrentWave;
    public int enemyMaxWaveCount;
    public int enemyUnitsAlive;


    [Header("Needed Objects")]
    public GameController gameController;
    public UnitManager unitManager;

    public Player player;
    public static PlayerController me;
    public bool ended;
    public bool isUnderAttack;

    [Header("UI")]
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerNumber;

    void Start()
    {
        player = PhotonNetwork.LocalPlayer;
        FindGameObjects();
        SyncStatsWithGame();

        ended = false;

        if(PhotonNetwork.IsMasterClient){
            gameController.UpdatePlayer1Stats(this);
            gameController.isMaster = true;
        }
        else
        {
            gameController.UpdatePlayer2Stats(this);
            gameController.isMaster = false;
        }
        playerName.text = player.NickName;

        if(PhotonNetwork.IsMasterClient)
        {
            playerNumber.text = "1. játékos";
        }
        else{
            playerNumber.text = "2. játékos";
        }
        
        
    }

    void Update()
    {
        SyncStatsWithGame();
        SendStatsToEnemy();

        if(PhotonNetwork.IsMasterClient){
            gameController.UpdatePlayer1Stats(this);
        }
        else
        {
            gameController.UpdatePlayer2Stats(this);
        }

        //ellenfél támadása
        if(Input.GetKeyDown(KeyCode.Tab)){
            if(unitManager.unitsInGame.Count > 0){
                 gameController.CastSpell();
                photonView.RPC("UnderAttack",RpcTarget.Others);
            }
            else{
                gameController.SetErrorText("Nincsenek egységek");
            }
           
        }

        if(PhotonNetwork.IsMasterClient){
            player1FinalScore = stock + scrolls + gold;
            player2FinalScore = enemyStock + enemyScrolls + enemyGold;
        }else{
            player2FinalScore = stock + scrolls + gold;
            player1FinalScore = enemyStock + enemyScrolls + enemyGold;
        }

        if(gameController.stock == 0){
           
            SendPlayerScoresToGameController();
            gameController.ended = true;
        }

        if(gameController.ended){
            SendPlayerScoresToGameController();
            if(!ended){
                photonView.RPC("EndGameForAll",RpcTarget.All);
                ended = true;
            }
        }
    }

    public void SendPlayerScoresToGameController(){
        gameController.player1FinalScore = player1FinalScore;
        gameController.player2FinalScore = player2FinalScore;
    }

    [PunRPC]
    public void EndGameForAll(){
        gameController.EndGame();
    }

    [PunRPC]
    public void UnderAttack(){
             gameController.UnderAttack();
    }

    public void SyncStatsWithGame(){
        stock = gameController.stock;
        scrolls = gameController.scrolls;
        gold = gameController.gold;

        currentWave = unitManager.waveCount;
        maxWaveCount = unitManager.maxWaveCount;

        unitsAlive = unitManager.unitsInGame.Count;
    }

    public void FindGameObjects()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();
        
        playerName = GameObject.Find("PlayerNameTMP").GetComponent<TextMeshProUGUI>();
        playerNumber = GameObject.Find("PlayerNumberTMP").GetComponent<TextMeshProUGUI>();
    }

    public void SendStatsToEnemy(){
        photonView.RPC("GetStatsOfEnemy",RpcTarget.Others,stock,scrolls,gold,currentWave,maxWaveCount,unitsAlive);
    }

    [PunRPC]
    public void GetStatsOfEnemy(int _stock,int _scrolls, int _gold, int _currentWave, int _maxWaveCount, int _unitsAlive){
        enemyStock = _stock;
        enemyScrolls = _scrolls;
        enemyGold = _gold;
        enemyCurrentWave = _currentWave;
        enemyMaxWaveCount = _maxWaveCount;
        enemyUnitsAlive = _unitsAlive;
    }
}
