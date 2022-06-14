using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Az egyes egységekre rakott szkript
public class UnitController : MonoBehaviour
{
    
    public GameObject Unit;
  

    public int health;
    public float moveSpeed;
    public float baseMoveSpeed;

    public GridController gridController;
    public UnitManager unitManager;

    Rigidbody unitRB;
    Vector3 moveDirection;
    Renderer unitRenderer;
    bool dangerZone;
    //bool healZone;

    public UnitController Init(int _health,float _moveSpeed){
        this.health = _health;
        this.baseMoveSpeed = _moveSpeed;
        this.moveSpeed = _moveSpeed;
        return this;
    }

    void Awake(){
        //A Unit változó beállítása az aktuális objektumra
        Unit = this.gameObject;

        //A játék hierarchiájában megkeressük a GridController és a UnitController nevű objektumokat és a szkript komponensét lekérjük
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
        unitManager = GameObject.Find("UnitManager").GetComponent<UnitManager>();


        baseMoveSpeed = 3;
        
        moveSpeed = baseMoveSpeed;
        health = 200;
    }

    // Start is called before the first frame update
    void Start()
    {
       //A renderer komponenst be kell állítanunk, át kell adni a változónak, ha azon keresztül akarjuk egyszerűbben állítani a színét az objektumnak
       unitRenderer = Unit.GetComponent<Renderer>();
       unitRenderer.material.SetColor("_Color", Color.white);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gridController.curFlowField == null) { return; }
        //Folyamatosan tároljuk az egység alatt lévő cellát
        Cell cellBelow = gridController.curFlowField.GetCellByWorldPos(Unit.transform.position);
        //A mozgás iránya egy 3 dimenziós vektor lesz, amit beállítunk az alatta lévő cella irányvetorává.
        //bár 3 dimenziósat vár az y koordináta egyszerűen nulla lesz
        moveDirection = new Vector3(cellBelow.bestDirection.Vector.x, 0, cellBelow.bestDirection.Vector.y);

        //Az egység színének változtatása ahogy fogy az életereje
        switch(health){
            case 250:
                unitRenderer.material.SetColor("_Color", Color.green);
                break;
            case 200:
                unitRenderer.material.SetColor("_Color", Color.white);
                break;
            case 150:
                unitRenderer.material.SetColor("_Color", Color.yellow);
                break;
            case 100:
                unitRenderer.material.SetColor("_Color", Color.magenta);
                break;
            case 50:
                unitRenderer.material.SetColor("_Color", Color.red);
                break;
        }   
			
        //Az egység fizikáért felelős komponensének lekérdezése
        unitRB = Unit.GetComponent<Rigidbody>();
        //A gyorsulást az egység mozgási sebessége alapján állítjuk (mivel ez az update-ben van ez folyamatosan ellenőrzés alatt van, így ha a movespeed változik, az erre is hatással van)
        unitRB.velocity = moveDirection * moveSpeed;

        if(health < 0){
            health = 0;
        }else if (health == 0){
            //ha elfogy az életereje kitöröljük.
            unitManager.DeleteUnit(Unit);
            
            
        }


    }

    void OnTriggerStay(Collider other){
        if(other.gameObject.CompareTag("DmgTower")){
            if(health == 0){
                //Ha a torony közelében halt meg az egység, vegye ki a toronyközeli egységek listájából
                other.gameObject.GetComponent<TowerController>().unitsNearby.Remove(gameObject);
            }
        }
    }

    //ütközés ellenőrzése (nem fizikai ütközés, csak a Collider komponensek találkozása)
    public void OnTriggerEnter(Collider other){
        //Ha nehéz terepen vagyunk, l
		if (other.gameObject.CompareTag("RoughTerrain"))
        {
            setMS(1);
        }

        
        //Ha a torony hatókörében vagyunk, folyamatosan csökken az életerő
        if(other.gameObject.CompareTag("DmgTower")){
            dangerZone = true;
            //StartCoroutine("DecayHealth");

        }
    }

    //ütköző elhagyása
    void OnTriggerExit(Collider other){
            //mozgási sebesség visszaállítása
            setMS(baseMoveSpeed);
            dangerZone = false;
            StopCoroutine("DecayHealth");
    }

    //a torony hatókörében másodpercenként csökken az életerő
    IEnumerator DecayHealth(){
        while(dangerZone){
            health -= 50;
            yield return new WaitForSeconds(1);
        }
        
    }

    public void TakeDamage(int amount){
        this.health -= amount;
    }

 


    public void setMS(float amount){
        this.moveSpeed = amount;
    }

    public float getBaseMS(){
        return this.baseMoveSpeed;
    }

    public void setBaseMS(float amount){
        this.baseMoveSpeed = amount;
    }

    public float getMS(){
        return this.moveSpeed;
    }





}
