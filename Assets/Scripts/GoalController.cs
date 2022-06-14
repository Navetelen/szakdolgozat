using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//cél objektum irányító szkriptje
public class GoalController : MonoBehaviour
{
    public Vector3 position;
    public bool gameIsRunning;
    public GameController gameController;

    public List<GameObject> unitsAttacking;

    public int attackers;

    void Start(){
        position = this.transform.position;
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gameController.goalController = this.GetComponent<GoalController>();
        InvokeRepeating("CheckState",1f,1f);
    }

    void Update(){
        gameIsRunning = gameController.gameIsRunning;
    }

    public void CheckState(){
        if(gameIsRunning){
            attackers = unitsAttacking.Count;
            if(attackers > 0 && attackers < 10){
                gameController.changeStock(-1);
            }else if(attackers >= 10 && attackers <=20){
                gameController.changeStock(-3);
            }else if(attackers > 20){
                gameController.changeStock(-5);
            }else{
                gameController.changeGold(5);
            }
        }
    }

     public void OnTriggerEnter(Collider other){
        //Támadók objektumainak tárolása listában
		if (other.gameObject.CompareTag("Unit"))
        {
            unitsAttacking.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.CompareTag("Unit")){
            unitsAttacking.Remove(other.gameObject);
        }
    }

    public void InitGoalStats(){
        unitsAttacking = new List<GameObject>();
        attackers = 0;
        position = this.transform.position;
        CancelInvoke();
    }
    
}
