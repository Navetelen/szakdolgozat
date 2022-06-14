using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;


public class TestPlayer : MonoBehaviourPun
{

    public int points;
    public int enemyPoints;
    public Text myPoints;
    private static TestPlayer instance;

    public TextMeshProUGUI p1_score;
    public TextMeshProUGUI p2_score;

    // Start is called before the first frame update
    void Start()
    {

       
        p1_score = GameObject.Find("P1_Score").GetComponent<TextMeshProUGUI>();
        p2_score = GameObject.Find("P2_Score").GetComponent<TextMeshProUGUI>();

        int id = PhotonNetwork.LocalPlayer.ActorNumber;
        //id = player.ActorNumber;
        GameManager.instance.players[id-1] = this;

        if(PhotonNetwork.IsMasterClient)
        {
            points = 300;
            gameObject.tag = "Player1";
            Debug.Log("I am master and P1. Score: " + points);
        }
        else
        {
            points = 100;
            gameObject.tag = "Player2";
            Debug.Log("I am P2. Score: " + points);
        }

        sendMyPointsToEnemy();
        UpdateTexts();
        


    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Q))
            GetPoints();
            photonView.RPC("UpdateTexts",RpcTarget.All);
        if(Input.GetKeyUp(KeyCode.W))
            GivePoints();
        if(Input.GetKeyUp(KeyCode.E))
            AllGetsPoints();
        
        
    }

    [PunRPC]
    public void GetPoints()
    {
        points += 100;
        Debug.Log("My points: " + points);
        sendMyPointsToEnemy();
    }


    public void GivePoints()
    {
        photonView.RPC("GetPoints",RpcTarget.Others);
        Debug.Log("Gave to enemy");
    }


    public void AllGetsPoints()
    {
        Debug.Log("All got points");
        photonView.RPC("GetPoints",RpcTarget.All);
    }

    [PunRPC]
    public void UpdateTexts()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            p1_score.text = "Score: " + points;
            p2_score.text = "Score: " + enemyPoints;
        }
        else
        {
            p1_score.text = "Score: " + enemyPoints;
            p2_score.text = "Score: " + points;
        }
        
    }

    public void sendMyPointsToEnemy()
    {
        photonView.RPC("GetPointsOfEnemy",RpcTarget.Others,points);
    }

    [PunRPC]
    public void GetPointsOfEnemy(int _points)
    {
        enemyPoints = _points;
    }


}
