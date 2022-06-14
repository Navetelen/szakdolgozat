using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
	public Cell[,] grid { get; private set; }
	public Vector2Int gridSize { get; private set; }
	public float cellRadius { get; private set; }
	public Cell destCell;

	private float cellDiameter;

	public GridController gridController;


	public void Start()
	{
		gridController = GameObject.Find("GridController").GetComponent<GridController>();
	}
	//konstruktor
	public FlowField(float _cellRadius, Vector2Int _gridSize)
	{
		
		cellRadius = _cellRadius;
		cellDiameter = cellRadius * 2f;
		gridSize = _gridSize;
	}

	public void CreateGrid()
	{
		grid = new Cell[gridSize.x, gridSize.y];
		
		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				//a felülnézet miatt -> a 3D térben az x,y koordinátákból x,z lesz
				Vector3 worldPos = new Vector3(cellDiameter * x + cellRadius, 0, cellDiameter * y + cellRadius);
				//Mátrix feltöltése a létrehozott Cell példányokkal
				grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
			}	
		}
	}
 
	//A mezők alap költségének beállítása az alapján hogy sima mező, nehéz terep, vagy áthatolhatatlan fal
	public void CreateCostField()
	{

		//Az OverLapBox függvénynek szüksége van a halfExtent-re ami az objektum méretének fele minden dimenzióban.
		//Ezt megkapjuk ha a megadott sugárral felszorozzuk az egységvektort.
		Vector3 halfCell = Vector3.one * cellRadius;
		//Rétegmaszk létrehozása. Obstacle = áthatolhatatlan fal, RoughTerrain = nehéz terep
		//Ezeket be kell állítani a játéktérben létrehozott objektumokra
		int terrainMask = LayerMask.GetMask("Obstacle", "RoughTerrain");
		foreach (Cell currCell in grid)
		{
			//Overlapbox függvény megnézi mivel van ütközésben, de nem csak fizikailag, hanem a rétegeket is figyelembe veszi. Paraméterei:
			//Egy középpont
			//a megadott halfExtent
			//az objektum orientációja/forgása a játéktérben. Ez általában egységesen a Quaternion.identity-t kapja meg vagyis a "nullforgatás"-t
			//a rétegmaszk amihez viszonyít
			Collider[] obstacles = Physics.OverlapBox(currCell.worldPos, halfCell, Quaternion.identity, terrainMask);
			bool hasIncreasedCost = false;
			foreach (Collider col in obstacles)
			{
				
				// OBSTACLE, áthatolhatatlan 
				if (col.gameObject.layer == 6)
				{
					currCell.IncreaseCost(255);
					continue;
				}
				//Az Editorban a 7-es réteg a nehéz terep, megnövelt költség, és majd lassított mozgásis sebesség
				else if (!hasIncreasedCost && col.gameObject.layer == 7)
				{
					currCell.IncreaseCost(3);
					hasIncreasedCost = true;
				}
			}
		}
	}

	//a cél mezőtől számolva a távolságok kiszámolása minden mezőre
	//ehhez kellett az előző számolás, mert az egyes mezők költsége hozzáadódik. sima mező esetén 1, nehéz terep esetén 4
	public void CreateIntegrationField(Cell _destCell)
	{
		destCell = _destCell;

		destCell.cost = 0;
		destCell.bestCost = 0;

		//objektumok FIFO gyűjteménye
		Queue<Cell> cellsToCheck = new Queue<Cell>();

		//Sor inicializálása
		cellsToCheck.Enqueue(destCell);

		while(cellsToCheck.Count > 0)
		{
			//sorból kiszedés
			Cell currCell = cellsToCheck.Dequeue();
			List<Cell> currNeighbours = GetNeighbours(currCell.index, GridDirection.CardinalDir);
			foreach (Cell currNeighbour in currNeighbours)
			{
				//ha a költség 255, akkor nem változtatunk
				if (currNeighbour.cost == byte.MaxValue) { continue; }

				//minden cella bestCost-ja 65535-re van állítva (ushort típus maximuma)
				//A célponttól kezdve, megnézzük a szomszédok költségét. Ha az (1 vagy 4) hozzáadva a sajátunk hoz (0) kevesebb mint 65535, akkor beállítjuk a szomszéd bestcostját a költségére + a mi legjobb költségünkre.
				//a szomszédra is megnézzük ugyanezt, itt már alap cellák esetén 1+1 lesz, aztán 2 + 1. és így nő a költségük (ehhez kellett a costField, így a roughterrain nem fogja megérni sokszor, hosszabb útnak számolódik)
				//így épül ki az egész mátrix minden mezőjére a költség és így az optimális útvonalak minden pontból a cél felé

				if (currNeighbour.cost + currCell.bestCost < currNeighbour.bestCost)
				{
					currNeighbour.bestCost = (ushort)(currNeighbour.cost + currCell.bestCost);
					cellsToCheck.Enqueue(currNeighbour);
				}
			}
		}
	}

	//a beállított költségek alapján a végső irány meghatározása
	public void CreateFlowField()
	{

		foreach(Cell currCell in grid)
		{
			List<Cell> currNeighbours = GetNeighbours(currCell.index, GridDirection.AllDir);

			int bestCost = currCell.bestCost;

			foreach(Cell currNeighbour in currNeighbours)
			{
				//az összes szomszédunk közül a legkisebb költséggel rendelkezőt megkeressük
				if(currNeighbour.bestCost < bestCost)
				{
					bestCost = currNeighbour.bestCost;
					//Az irány egy vektor lesz ami annak irányába mutat
					//Ezt a 2 dimenziós vektort (Vector2Int típusút) átadjuk a segédfüggvényünknek és az visszatér egy GridDirectionnel, és be is állítja az aktuális mező bestDirectionjének
					currCell.bestDirection = GridDirection.GetDirectionFromV2I(currNeighbour.index - currCell.index);
				}
			}
		}
	}

	//szomszédok lista feltöltése
	private List<Cell> GetNeighbours(Vector2Int index, List<GridDirection> directions)
	{
		List<Cell> neighbours = new List<Cell>();

		foreach (Vector2Int currDir in directions)
		{

			Cell newNeighbour = GetCellByGridPos(index, currDir);
			if (newNeighbour != null)
			{
				neighbours.Add(newNeighbour);
			}
		}
		return neighbours;
	}

	//Egy Cella lekérdezése a mátrixban való elhelyezkedése alapján
	public Cell GetCellByGridPos(Vector2Int orignPos, Vector2Int relativePos)
	{
		Vector2Int finalPos = orignPos + relativePos;

		//Ha a mátrixon kívül esne
		if (finalPos.x < 0 || finalPos.x >= gridSize.x || finalPos.y < 0 || finalPos.y >= gridSize.y)
		{
			return null;
		}

		else { return grid[finalPos.x, finalPos.y]; }
	}

	//Egy Cella lekérdezése a 3D játéktérben való elhelyezkedése alapján
	//ezt alakítjuk át a mátrix egy [x,y] pontjára
	//Erre azért van szükség, mert amikor kattintunk, az az esemény egy world positiont ad vissza, ezt kell konvertálnunk
	public Cell GetCellByWorldPos(Vector3 worldPos)
	{
		
		float percentX = worldPos.x / (gridSize.x * cellDiameter);
		float percentY = worldPos.z / (gridSize.y * cellDiameter);

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.Clamp(Mathf.FloorToInt((gridSize.x) * percentX), 0, gridSize.x - 1);
		int y = Mathf.Clamp(Mathf.FloorToInt((gridSize.y) * percentY), 0, gridSize.y - 1);
		return grid[x,y];
	}
}
