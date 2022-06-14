using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

//a fizikailag a világba teremtett mezők irányító szkriptje
public class cellManager : MonoBehaviour
{
    
    Renderer cellRenderer;
    //GameObject Cell;

    public Color highlighted;
    public Color baseColor;
    public Color deactivated;

    public bool canBuild;

    public Vector2Int gridIndex;
    public byte cost;
	public ushort bestCost;

    public GameController gameController;
    public GridDebug gridDebug;

    public TextMeshPro costText;

    Vector3 cellPositionElevated;
    Vector3 cellPositionGround;

    FlowField flowField;
    
    

    private void Awake(){
        
        //renderelő lekérése a szín változtatáshoz
        cellRenderer = this.GetComponent<Renderer>();
        
        highlighted = new Color(1,1,1,1);
        baseColor = new Color(1,1,1,0.5f);
        deactivated = new Color(1,1,1,0.0f);

        flowField = new FlowField(0.5f,new Vector2Int(40,50));
        

         cellRenderer.material.SetColor("_Color",baseColor);

        //player objektum lekérése
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        //építés eleinte lehetséges
        canBuild = gameController.canBuild;

        costText.text = "";
    }

    void Start(){
        cellPositionGround = new Vector3(transform.position.x,-0.5f,transform.position.z);
        cellPositionElevated = new Vector3(transform.position.x,0,transform.position.z);
        gridDebug = GameObject.Find("GridDebug").GetComponent<GridDebug>();
        //setCostFieldText();
    }



    //egér föléhelyzésekor
    void OnMouseOver(){
        //ha építő módban vagyunk, és aktív
        if(canBuild){
            //világosabbá tesszük azt amelyiken épp az egér van -> player feedback
             cellRenderer.material.SetColor("_Color",highlighted);

            //Q-val tornyot teremtünk ennek a cellának a pozíciójára, a transform.position az itt this.transform.position, vagyis ennek az objektumnak az elhelyezkedése a játéktérben
            if (Input.GetKeyUp(KeyCode.Q)){
                gameController.buildTower(cellPositionElevated);
                

            }
            //W-vel falat teremtünk
            if (Input.GetKeyUp(KeyCode.W)){
                gameController.buildWall(cellPositionElevated);
            }

            //E-vel nehéz terepet teremtünk, ezt alacsonyabbra helyezzük, hogy ne akajanak el benne az egységek
            if (Input.GetKeyUp(KeyCode.E)){
                gameController.buildTerrain(cellPositionGround);
            }


            //E-vel nehéz terepet teremtünk, ezt alacsonyabbra helyezzük, hogy ne akajanak el benne az egységek
            if (Input.GetKeyUp(KeyCode.G)){
                if(gameController.devMode){
                    gameController.ResetGoal(cellPositionElevated);
                }
                else
                {
                    //csak dev modban lehet
                }
            }
        }else{
            cellRenderer.material.SetColor("_Color",deactivated);
        }

        //Az ideiglenes cél beállítása az egérgörgő lenyomásával
        if (Input.GetMouseButtonDown(2))
        {
            //elküldi a saját gridIndexét és worldpositionjét
            gameController.DummyGoal(cellPositionGround,gridIndex);
		
		
        
        }
    }

    void OnMouseExit(){
        if(canBuild){
            cellRenderer.material.SetColor("_Color",baseColor);
        }else{
            cellRenderer.material.SetColor("_Color",deactivated);
        }
        
    }
  
  
}
