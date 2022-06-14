using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A játékban lévő torony objektumok összefogó objektuma
public class TowerManager : MonoBehaviour
{
    public List<GameObject> towersInGame;
    TowerController towerController;

    public GameController gameController;

    // Start is called before the first frame update
    void Awake()
    {
        towersInGame = new List<GameObject>();
    }

    void Start(){
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    void Update(){
        if(gameController.devMode){
            if(Input.GetKeyDown(KeyCode.A)){
                boostTowers();
            }
        }
        
    }

    public void addTower(GameObject tower){
        towersInGame.Add(tower);
    }


    //Ez azon képesség implementálása, ami az összes torony képességét azonnal elsüti
    public void boostTowers(){
        gameController.CastSpell();
        foreach (GameObject tower in towersInGame)
		{
			towerController = tower.GetComponent<TowerController>();
            towerController.setMana(100);
		}
    }

    //Tornyok alaphelyzetbe állítása - a reset funkcióhoz szükséges
    public void InitTowers(){
        foreach (GameObject tower in towersInGame)
		{
			towerController = tower.GetComponent<TowerController>();
            towerController.setMana(0);
            towerController.StopAllCoroutines();
            towerController.CancelInvoke();
            towerController.unitsNearby = new List<GameObject>();
		}
    }
}
