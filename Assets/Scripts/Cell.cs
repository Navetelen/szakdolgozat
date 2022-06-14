using UnityEngine;

public class Cell
{
	public Vector3 worldPos;
	public Vector2Int index;
	public byte cost;
	public ushort bestCost;
	public GridDirection bestDirection;

	//konstruktor
	public Cell(Vector3 _worldPos, Vector2Int _index)
	{
		
		worldPos = _worldPos;
		index = _index;
		cost = 1;
		bestCost = ushort.MaxValue;
		bestDirection = GridDirection.None;
	}

	public void IncreaseCost(int amnt)
	{
		//beállítjuk 255-re
		if (cost == byte.MaxValue) { return; }
		//korlát, ha több lenne
		if (amnt + cost >= 255) { cost = byte.MaxValue; }
		//alap viselkedés
		else { cost += (byte)amnt; }
	}
}