using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Az egységek teremtéséért és közös kezeléséért felelős objektum
public class UnitManager : MonoBehaviour
{
	
    public GridController gridController;
    public GameObject unitPrefab;

	//Ezek az értékek az Editorban állíthatóak
    public int unitsPerWave;
	public int unitHealth;
    public float unitMoveSpeed;

	public bool canSpawn = false;
	public bool startedOnce = false;
	public int waveCount = 1;
	public int maxWaveCount;
	public int waveTimer;
	public float waveCountDown;

	public bool winCondition = false;


    public List<GameObject> unitsInGame;

    UnitController unitController;

    public GameController gameController;
 
    GoalController goalController;

	private void Awake()
	{
		unitsInGame = new List<GameObject>();
	}

	private void Start(){

        	gameController = GameObject.Find("GameController").GetComponent<GameController>();
			//this.statusText = GameObject.Find("STATUSTEXT").GetComponent<Text>();
			//this.waveCountText = GameObject.Find("WaveCountText").GetComponent<Text>();
			

       		goalController = GameObject.Find("goal").GetComponent<GoalController>();
			InitSpeed();
	}

	public void BeginSpawning(){
		//Ne lehessen elindítani csak egyszer
		if(!startedOnce){
			//Egységek teremtésének elindítása
			StartCoroutine("SpawnTimer");
			startedOnce = true;
		}
		
		
	}

	void Update()
	{

		if(canSpawn)
		{
			SpawnUnits();
		}
		else if(gameController.ended)
		{
			DestroyUnits();
		}

		if(gameController.devMode){
			if (Input.GetKeyDown(KeyCode.S)){
				//egységek lefagyasztása az S gombbal
				StunUnits();
			}

			if (Input.GetKeyDown(KeyCode.D)){
				//Egységek megfélemlítésa a D gombbal
				FearUnits();
			}


		}
		


	}


	public void StunUnits(){
		if(unitsInGame.Count != 0){

			gameController.CastSpell();
			if(gameController.scrolls > 0 && !gameController.isCasting){
				foreach (GameObject unit in unitsInGame)
				{
					//egységek irányító szkriptjének lekérése
					unitController = unit.GetComponent<UnitController>();
						StartCoroutine(Stun(unitController));
				}
			}
		}
		
	}

	public void FearUnits(){
		if(unitsInGame.Count != 0){
			gameController.CastSpell();
			if(gameController.scrolls > 0 && !gameController.isCasting){
				foreach (GameObject unit in unitsInGame)
				{
					//egységek irányító szkriptjének lekérése
					unitController = unit.GetComponent<UnitController>();
					StartCoroutine(Fear(unitController));
				}
			}
		}
		
	}

	public void BoostUnits()
	{
		foreach (GameObject unit in unitsInGame)
				{
					//egységek irányító szkriptjének lekérése
					unitController = unit.GetComponent<UnitController>();
					StartCoroutine(Boost(unitController));
				}
	}

	public void InitSpeed(){
		foreach(GameObject unit in unitsInGame){
			unitController = unit.GetComponent<UnitController>();
			unitController.setBaseMS((float)unitMoveSpeed);
		}
	}

	public void StopUnits(){
		foreach(GameObject unit in unitsInGame){
			unitController = unit.GetComponent<UnitController>();
			unitController.setMS(0);
		}
	}

	IEnumerator Stun(UnitController unitController){
		//A mozgási sebesség 0-ra állítása, majd 7 másodperc múlva visszaállítása
		unitController.setMS(0);
		yield return new WaitForSeconds(gameController.spellTimer);
		unitController.setMS(unitController.getBaseMS());
	}

	IEnumerator Fear(UnitController unitController){
		//A mozgási sebesség negatívra állítása, ennek hatására a mezők irányával ellentétesen fognak haladni, majd 3 másodperc múlva visszaállítása
		unitController.setMS(unitController.getBaseMS() * (-1.0f));
		yield return new WaitForSeconds(gameController.spellTimer);
		unitController.setMS(unitController.getBaseMS());
	}

	IEnumerator Boost(UnitController unitController){
		//A mozgási sebesség negatívra állítása, ennek hatására a mezők irányával ellentétesen fognak haladni, majd 3 másodperc múlva visszaállítása
		unitController.setMS(unitController.getBaseMS() * (2.0f));
		yield return new WaitForSeconds(3);
		unitController.setMS(unitController.getBaseMS());
	}
	

	public IEnumerator SpawnTimer(){
		
		while(waveCount <= maxWaveCount){
			if(!canSpawn){
				//3 másodperc eltöltése
				for(waveCountDown = waveTimer;waveCountDown > 0;waveCountDown -= Time.deltaTime){
					yield return null;
				}
				//egységek teremtése
				canSpawn = true;
				//Teremtés után egy rövid pillanatig várunk és utána állítjuk false-ra, az azonnali visszaállítás nem működött megfelelően
				yield return new WaitForSeconds(0.05f);
				canSpawn = false;
			}
			waveCount++;
			if(!gameController.devMode)
			{
				gameController.unitsPerWave += 5;
			}
		}
		winCondition = true;

	}

	public void SpawnUnits()
	{
		//A mátrix méretének egy kis részén teremhetnek
		Vector2Int gridSize = gridController.gridSize/6;
		float nodeRadius = gridController.cellRadius;
		Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);

		int colMask = LayerMask.GetMask("Impassible", "Units");

		Vector3 newPos;
		for (int i = 0; i < unitsPerWave; i++)
		{
			if(canSpawn){
				//egység fizikai teremtése
				GameObject newUnit = Instantiate(unitPrefab);
				newUnit.GetComponent<UnitController>().Init(unitHealth,unitMoveSpeed);
				//egység objektum szülője ez a UnitController objektum lesz a hierarchiában
				newUnit.transform.parent = transform;
				//egységek listához hozzáadás
				unitsInGame.Add(newUnit);
				do
				{
					//random hely megadása a lehetséges területen belül
					newPos = new Vector3(Random.Range(0, maxSpawnPos.x), 0, Random.Range(0, maxSpawnPos.y));
					newUnit.transform.position = newPos;
				}
				while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);
			}
		
			
		}
	}



	public void DestroyUnits()
	{
		//Az objektumok törlése
		foreach (GameObject go in unitsInGame)
		{
			Destroy(go);
		}
		unitsInGame.Clear();
	}

	public void DeleteUnit(GameObject unit){
		//Ezt a függvényt használom törlésre, mert nem elég az objektumot törölni fizikailag a játéktérből
		//Hiszen ekkor a nyilvántartott egyégek lista nem fog mutatni semmire, ezért abból is ki kell szedni a Remove metódussal
		unitsInGame.Remove(unit);
		Destroy(unit);
		//Mivel ezt a torony tudja elérni, az egységek semlegesítéséért arany jár a játékosnak
		gameController.changeGold(30);
	}

	public void InitUnitStats(){
		waveCount = 1;
        DestroyUnits();
        StopAllCoroutines();
        startedOnce = false;
        winCondition = false;
	}


	
}
