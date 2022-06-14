using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObjectDev : MonoBehaviour
{
    public GameController gameController;
    public GridController gridController;
    public bool isObstacle;

    // Start is called before the first frame update
    void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
    }

    void OnMouseOver(){
        if(gameController.devMode){
            if(Input.GetMouseButtonDown(1)){
                this.gameObject.SetActive(false);
                if(isObstacle && gameController.gameIsRunning)
                {
                    gridController = GameObject.Find("GridController").GetComponent<GridController>();
                    Vector3 goalposition = gridController.FindGoal();
                    gridController.setGoal(goalposition);
                }
                
                
                
                
            }
        }
        
    }
}
