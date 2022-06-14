using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int gridSize;
    public float cellRadius = 0.5f;
    public FlowField curFlowField;
	public GridDebug gridDebug;
	public GameController gameController;

	public int dummyTimer;

	public GameObject gridCell;
	public GameObject goal;
	bool goalSpawned;
	Renderer goalRenderer;



	public List<GameObject> gridObjects;

	public Vector3 pos;


	

    public void InitializeFlowField()
	{
		//új FlowField létrehozása és a Grid (mátrix) feltöltése
        curFlowField = new FlowField(cellRadius, gridSize);
        curFlowField.CreateGrid();
		//Ezt átadjuk a Grid Debugnak, ami a bemutatást segíti elő
		gridDebug.SetFlowField(curFlowField);
	}

	//Az Awake function történik meg a leghamarabb, már mindenféle játéktérbeli kirajzolás előtt lefut
	private void Awake(){
		InitializeFlowField();
	}

	

	//A start funkció az Awakenél később fut le, ekkor már biztosan léteznek a játéktérbe elhelyezett objektumok
	private void Start(){

		int width = curFlowField.gridSize.x;
		int height = curFlowField.gridSize.y;
		goalSpawned = gameController.goalSpawned;

		//A mátrixunk a logikában már létezik, és a szerkesztőben látható is hozzá a háló
		//Azonban a játék futtatásakor nem lehet vele interakcióba lépni. Ezért egy konkrét 3D objektumot is teremtünk a mátrix minden egyes mezőjéhez.
		//Ez a játékossal való interakcióhoz használatos
		for(int i = 0;i< width;i++){
			for(int j = 0;j < height;j++){
				
				Cell curCell = curFlowField.grid[i,j];
				pos = new Vector3(curCell.worldPos.x, -1,curCell.worldPos.z);
				//Az instantiate metódus teremt a játéktérbe egy 3d objektumot, amit az Editorban tudunk megadni, hogy mi legyen
				//paraméterként vár egy objektumot, egy pozíciót, és egy forgatást
				GameObject newGridCell = Instantiate(gridCell,pos,Quaternion.identity);
				cellManager cm = newGridCell.GetComponent<cellManager>();
				cm.gridIndex = new Vector2Int(i,j);
				cm.cost = curCell.cost;
				cm.bestCost = curCell.bestCost;
				//Ez csupán a hierarchia miatt, a jelenlegi GridController objektum alá fogja szervezni az összes létrehozott cellát
				newGridCell.GetComponent<Transform>().SetParent(this.transform,false);
				//tároljuk is őket egy listában
				gridObjects.Add(newGridCell);

			}
			
		}
	}

	public void SetGridCellCosts(int type){
		//A cella objektumokon található szöveg beállítása
		int width = curFlowField.gridSize.x;
		int height = curFlowField.gridSize.y;

		for(int i = 0;i< width;i++){
			for(int j = 0;j < height;j++){
				
				Cell curCell = curFlowField.grid[i,j];
				pos = new Vector3(curCell.worldPos.x, -1,curCell.worldPos.z);
				//Az instantiate metódus teremt a játéktérbe egy 3d objektumot, amit az Editorban tudunk megadni, hogy mi legyen
				//paraméterként vár egy objektumot, egy pozíciót, és egy forgatást
				//GameObject newGridCell = Instantiate(gridCell,pos,Quaternion.identity);
				foreach(GameObject go in gridObjects)
				{
					cellManager cm = go.GetComponent<cellManager>();
					if(cm.gridIndex == curCell.index){
						if(type == 1){
							cm.cost = curCell.cost;
							cm.costText.text = cm.cost.ToString();
						}
						if(type == 2){
							cm.bestCost = curCell.bestCost;
							cm.costText.text = cm.bestCost.ToString();
						}
						else
						{
							cm.costText.text = "";
						}
					}
				}
			}
		}
	}

	public void ControlGridCellCosts(){
		if(gridDebug.curDisplayType == FlowFieldDisplayType.CostField)
			SetGridCellCosts(1);
		else if(gridDebug.curDisplayType == FlowFieldDisplayType.IntegrationField)
			SetGridCellCosts(2);
		else
			SetGridCellCosts(3);
	}

	

	void Update(){
		goalSpawned = gameController.goalSpawned;
	}


	//segédfüggvény ami a rendes célt elhalványítja amikor ideiglenes célt rakunk le
	private void changeAlpha(float alphaValue){
			Color oldColor = goalRenderer.sharedMaterial.color;
			Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);
			goalRenderer.sharedMaterial.SetColor("_Color", newColor);
	}


	public Vector3 FindGoal(){
			GameObject goal = GameObject.FindWithTag("goal");
			Debug.Log("Goal" + goal);
			if(goal == null){
				Debug.Log("Nincs cél");
				return new Vector3(0,0,0);
			}
			return goal.transform.position;
		
        
    }
	

	//cél mező beállítása
	public void setGoal(){
		//Lenullázzul a FlowFieldet, megnézzük a költségeket (azóta történhetett újabb dolgok építése)
		InitializeFlowField();
		curFlowField.CreateCostField();

		//Egy elég nagy értéket adunk meg, ami biztosan kívül van a pályán, de ez csak azt jelenti, hogy a legszélén lesz a cél. Ez kattintás helyett egy konkrét pozívció a játéktérben
		//Vector3 mousePos = new Vector3(10000f,2020f,10f);
		Vector3 goalPos = FindGoal() + new Vector3 (0.5f,0,0.5f);
		
		//Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(goalPos);
		
		//Ez alapján lekérdezzük ez melyik mezőre fordítható le
		
		//Cell destinationCell = curFlowField.GetCellByWorldPos(goalPos);
		
		Vector2Int gridIndexLocal = new Vector2Int((int)(goalPos.x+0.5f),(int)(goalPos.z+0.5f));
		Cell destinationCell = new Cell(goalPos,gridIndexLocal);

		//Az INtegrationField-et is létrehozzuk ami a céltól kiszámolja az útvonalat, amit át is adunk neki paraméterként
		curFlowField.CreateIntegrationField(destinationCell);

		//irányvektorok beállítása
		curFlowField.CreateFlowField();

		//Grid debuggal az ikonok kirajzolása
		gridDebug.DrawFlowField();
		//Ez egy ellenőrzés, hogy ne lehessen csak egyszer fizikailag teremteni a célpont objektumot
		//Ez a szín változtatásához szükséges komponens "megszerzése"
		goalRenderer = goal.GetComponent<Renderer>();

		changeAlpha(1.0f);
		
	}

	public void setGoal(Vector3 goalPosition){
		//Lenullázzul a FlowFieldet, megnézzük a költségeket (azóta történhetett újabb dolgok építése)
		InitializeFlowField();
		curFlowField.CreateCostField();

		//Egy elég nagy értéket adunk meg, ami biztosan kívül van a pályán, de ez csak azt jelenti, hogy a legszélén lesz a cél. Ez kattintás helyett egy konkrét pozívció a játéktérben
		//Vector3 mousePos = new Vector3(10000f,2020f,10f);
		//Vector3 goalPos = findGoal() + new Vector3 (0.5f,0,0.5f);
		
		//Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(goalPos);
		
		//Ez alapján lekérdezzük ez melyik mezőre fordítható le
		
		Cell destinationCell = curFlowField.GetCellByWorldPos(goalPosition);
		
		//Az INtegrationField-et is létrehozzuk ami a céltól kiszámolja az útvonalat, amit át is adunk neki paraméterként
		curFlowField.CreateIntegrationField(destinationCell);

		//irányvektorok beállítása
		curFlowField.CreateFlowField();

		//Grid debuggal az ikonok kirajzolása
		gridDebug.DrawFlowField();
		//Ez egy ellenőrzés, hogy ne lehessen csak egyszer fizikailag teremteni a célpont objektumot
		if(!goalSpawned){
			//Instantiate(goal, destinationCell.worldPos, Quaternion.identity);
			
			goalSpawned = true;

		}
		//Ez a szín változtatásához szükséges komponens "megszerzése"
		goalRenderer = goal.GetComponent<Renderer>();

		changeAlpha(1.0f);
		
	}



	//Ez egy időzítő metódus (IEnumerator)
	public IEnumerator DummyGoal(Vector3 _worldPos, Vector2Int _cellIndex){
			if(gameController.stock >10)
			{
				bool dummy = true;
				gameController.CastSpell(dummy);
				
				//Megint mindent lenullázunk és újraszámolunk
				InitializeFlowField();
				curFlowField.CreateCostField();

				Cell destinationCell = curFlowField.GetCellByWorldPos(_worldPos);

				curFlowField.CreateIntegrationField(destinationCell);
				curFlowField.CreateFlowField();
				gridDebug.DrawFlowField();

				changeAlpha(0.2f);
				//Ez az időzítő, a dummyTimer értékben van megadva meddig vár
				yield return new WaitForSeconds(dummyTimer);
				//Az idő leteltével visszaállítja az eredeti célpontot, megkeresve annak helyét
				Vector3 goalPosition = FindGoal();
				setGoal(goalPosition);
			}
			
			
			
			
	}

	public void setDummyGoal(Vector3 _worldPos, Vector2Int _cellIndex){
		StartCoroutine(DummyGoal(_worldPos,_cellIndex));
	}

	//építés letiltása -> a cellák fizikai manifesztációjának deaktiválása
	public void disableBuild(){
		foreach(GameObject obj in gridObjects){
			cellManager cm = obj.GetComponent<cellManager>();
			Renderer cellRenderer = obj.GetComponent<Renderer>();
        	
			cellRenderer.material.SetColor("_Color",cm.deactivated);
			cm.canBuild = false;
		}
	}

	//építés engedélyezése -> a cellák fizikai manifesztációjának aktiválása
	public void enableBuild(){
		foreach(GameObject obj in gridObjects){
			cellManager cm = obj.GetComponent<cellManager>();
			Renderer cellRenderer = obj.GetComponent<Renderer>();

			cellRenderer.material.SetColor("_Color",cm.baseColor);
			cm.canBuild = true;
		}
	}
}
