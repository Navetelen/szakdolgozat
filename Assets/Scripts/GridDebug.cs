using UnityEditor;
using UnityEngine;

//Erre azért van szükség, hogy a mezők, és az útvonalak ne csak kódban létezzenek, hanem a futtatható kliensben is látszódjon
//Ez szükséges a demozáshoz is, illetve teszteléshez/debugoláshoz is hasznos

//Különböző megjelenítési beállítások
public enum FlowFieldDisplayType { None, AllIcons, OnlyDestination, CostField, IntegrationField };

public class GridDebug : MonoBehaviour
{
	public GridController gridController;
	public bool displayGrid;

	public FlowFieldDisplayType curDisplayType;

	//A [SerializeField] attribútummal a Unity Editorban is megjeleni az érték. Mivel privát, nem módosítható onnan, de valós időben látjuk
	[SerializeField] private Vector2Int gridSize;
	[SerializeField] private float cellRadius;
	[SerializeField] private FlowField curFlowField;

	[SerializeField] private Sprite[] FFAssets;

	private void Start()
	{
		//képek, ikonok betöltése
		FFAssets = Resources.LoadAll<Sprite>("FFicons_made");
	}



	//Flowfield inicializálása
	public void SetFlowField(FlowField newFlowField)
	{
		curFlowField = newFlowField;
		cellRadius = newFlowField.cellRadius;
		gridSize = newFlowField.gridSize;
	}


	//egy cella vizuális reprezentációja a játékprogramban
	private void DisplayCell(Cell cell)
	{
		//létrehozunk egy objektumot amiez hozzáadunk egy Renderelőt, ez fogja kezelni a hozzáadott ikont (Spritenak hívják a 2D-s képet)
		GameObject iconGO = new GameObject();
		SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
		//hierarchia és játéktérbeli elhelyezés
		iconGO.transform.parent = transform;
		iconGO.transform.position = cell.worldPos;

		//A költségek alapján a megfelelő ikonok beállítása, forgatása

		//cél ikon
		if (cell.cost == 0)
		{
			iconSR.sprite = FFAssets[3];
			Quaternion newRot = Quaternion.Euler(90, 0, 0);
			iconGO.transform.rotation = newRot;
		}
		//áthatolhatatlan
		else if (cell.cost == byte.MaxValue)
		{
			iconSR.sprite = FFAssets[2];
			Quaternion newRot = Quaternion.Euler(90, 0, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.N)
		{
			iconSR.sprite = FFAssets[0];
			Quaternion newRot = Quaternion.Euler(90, 0, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.S)
		{
			iconSR.sprite = FFAssets[0];
			Quaternion newRot = Quaternion.Euler(90, 180, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.E)
		{
			iconSR.sprite = FFAssets[0];
			Quaternion newRot = Quaternion.Euler(90, 90, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.W)
		{
			iconSR.sprite = FFAssets[0];
			Quaternion newRot = Quaternion.Euler(90, 270, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.NE)
		{
			iconSR.sprite = FFAssets[1];
			Quaternion newRot = Quaternion.Euler(90, 0, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.NW)
		{
			iconSR.sprite = FFAssets[1];
			Quaternion newRot = Quaternion.Euler(90, 270, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.SE)
		{
			iconSR.sprite = FFAssets[1];
			Quaternion newRot = Quaternion.Euler(90, 90, 0);
			iconGO.transform.rotation = newRot;
		}
		else if (cell.bestDirection == GridDirection.SW)
		{
			iconSR.sprite = FFAssets[1];
			Quaternion newRot = Quaternion.Euler(90, 180, 0);
			iconGO.transform.rotation = newRot;
		}
		else
		{
			iconSR.sprite = FFAssets[0];
		}
		
	}

	private void DisplayAllCells()
	{
		if (curFlowField == null) { return; }
		foreach (Cell curCell in curFlowField.grid)
		{
			DisplayCell(curCell);
		}
	}

	private void DisplayDestinationCell()
	{
		if (curFlowField == null) { return; }
		DisplayCell(curFlowField.destCell);
	}

	public void ClearCellDisplay()
	{
		foreach (Transform t in transform)
		{
			GameObject.Destroy(t.gameObject);
		}
	}

	public void DrawFlowField()
	{
		ClearCellDisplay();

		switch (curDisplayType)
		{
			case FlowFieldDisplayType.AllIcons:
				DisplayAllCells();
				break;

			case FlowFieldDisplayType.OnlyDestination:
				DisplayDestinationCell();
				break;

			default:
				break;
		}
	}

	//A négyzetháló kirajzolása
	//A Gizmok megjeleníthető indikátorok pont ilyen célra, hogy láthatóvá tegyünk csak kódban létező objektumokat, viszont a játék menetébe ne zavarjon bele
	private void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
	{
		Gizmos.color = drawColor;
		for (int x = 0; x < drawGridSize.x; x++)
		{
			for (int y = 0; y < drawGridSize.y; y++)
			{
				Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
				Vector3 size = Vector3.one * drawCellRadius * 2;
				Gizmos.DrawWireCube(center, size);
			}
		}
	}
	
	//A DrawGizmos az az esemény, amikor a Unity keretrendszer kirajzolja a négyzethálót a játéktérbe. Ez a játékban nem jelenik meg, csak a szerkesztő nézetben. Így fizikailag megjelenik a mátrixunk kerete
	private void OnDrawGizmos()
	{
		if (displayGrid)
		{
			if (curFlowField == null)
			{
				DrawGrid(gridController.gridSize, Color.yellow, gridController.cellRadius);
			}
			
		}
		
		if (curFlowField == null) { return; }

		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.MiddleCenter;

		//CostField és IntegrationField költségek megjelenítése (szintén cska a szerkesztő nézetben látható)
		switch (curDisplayType)
		{
			case FlowFieldDisplayType.CostField:

				foreach (Cell curCell in curFlowField.grid)
				{
					//Handles.Label(curCell.worldPos, curCell.cost.ToString(), style);
				}
				break;
				
			case FlowFieldDisplayType.IntegrationField:

				foreach (Cell curCell in curFlowField.grid)
				{
					//Handles.Label(curCell.worldPos, curCell.bestCost.ToString(), style);
				}
				break;
				
			default:
				break;
		}
	}
}
