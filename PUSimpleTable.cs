/* Copyright (c) 2012 Small Planet Digital, LLC
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
 * (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class PUSimpleTableUpdateScript : MonoBehaviour {

	public PUSimpleTable table;

	public void LateUpdate() {
		if (table != null) {
			table.LateUpdate ();
		}
	}

	public IEnumerator ReloadTableCellsCoroutine() {
		return table.ReloadTableAsync ();
	}

	public void ReloadTableCells() {
		StopCoroutine("ReloadTableCellsCoroutine");
		StartCoroutine("ReloadTableCellsCoroutine");
	}

}

public class PUSimpleTableCell : PUTableCell {

	public static Vector2 CellSize(RectTransform tableTransform) {
		return new Vector2 (tableTransform.rect.width, 60.0f);
	}

	public static bool TestForVisibility(float cellPosY, RectTransform tableTransform, RectTransform tableContentTransform) {

		float scrolledPos = (-cellPosY) - tableContentTransform.anchoredPosition.y;
		if (Mathf.Abs (scrolledPos - tableTransform.rect.height / 2) < tableTransform.rect.height) {
			return true;
		}
		return false;
	}
		
	public override bool TestForVisibility() {
		if (TestForVisibility (cellTransform.anchoredPosition.y, tableTransform, tableContentTransform)) {
			return true;
		}
		return false;
	}
}


public partial class PUSimpleTable : PUSimpleTableBase {

	int currentScrollY = -1;

	public List<object> allObjects = null;
	PUSimpleTableUpdateScript tableUpdateScript;

	public void SetObjectList(List<object> objects) {
		allObjects = new List<object> (objects);
	}

	public PUSimpleTableCell LoadCellForData(object cellData, PUSimpleTableCell reusedCell) {

		PUSimpleTableCell cell = reusedCell;
		if (reusedCell == null) {
			string className = cellData.GetType ().Name + "TableCell";
			Type cellType = Type.GetType (className, true);

			cell = (Activator.CreateInstance (cellType)) as PUSimpleTableCell;
			cell.LoadIntoPUGameObject (this, cellData);
		} else {
			cell.cellData = cellData;
			cell.puGameObject.parent = this;
			cell.puGameObject.rectTransform.SetParent (this.contentObject.transform, false);

			cell.UpdateContents ();
		}

		cell.puGameObject.rectTransform.anchorMin = new Vector2 (0, 1);
		cell.puGameObject.rectTransform.anchorMax = new Vector2 (0, 1);
		cell.puGameObject.rectTransform.pivot = new Vector2 (0, 1);

		return cell;
	}


	public void ClearTable() {
		for(int i = activeTableCells.Count-1; i >= 0; i--) {
			PUSimpleTableCell cell = activeTableCells [i];
			EnqueueTableCell (cell);
		}
	}


	private List<PUSimpleTableCell> activeTableCells = new List<PUSimpleTableCell>();
	private Dictionary<string, List<PUSimpleTableCell>> pooledTableCells = new Dictionary<string, List<PUSimpleTableCell>>();


	public PUSimpleTableCell DequeueTableCell(object cellData){
		string dataKey = cellData.GetType ().Name;
		if (pooledTableCells.ContainsKey (dataKey) == false) {
			pooledTableCells [dataKey] = new List<PUSimpleTableCell> ();
		}

		PUSimpleTableCell cell = null;
		if (pooledTableCells [dataKey].Count > 0) {
			cell = pooledTableCells[dataKey][0];
			pooledTableCells [dataKey].RemoveAt (0);
		}

		cell = LoadCellForData(cellData, cell);
		cell.puGameObject.gameObject.SetActive (true);

		activeTableCells.Add (cell);

		return cell;
	}

	public void EnqueueTableCell(PUSimpleTableCell cell){
		string dataKey = cell.cellData.GetType ().Name;
		if (pooledTableCells.ContainsKey (dataKey) == false) {
			pooledTableCells [dataKey] = new List<PUSimpleTableCell> ();
		}
		pooledTableCells [dataKey].Add (cell);
		cell.puGameObject.gameObject.SetActive (false);

		activeTableCells.Remove (cell);
	}


	public IEnumerator ReloadTableAsync() {
		RectTransform contentRectTransform = contentObject.transform as RectTransform;

		currentScrollY = (int)contentRectTransform.anchoredPosition.y;

		// 1) Run through allObjects; instantiate a cell object based on said object class
		float y = 0;
		float nextY = 0;
		float x = 0;

		// Unload any cells which are not on the screen currently; store the object data for cells which are
		// still visible
		Dictionary<object,PUSimpleTableCell> visibleCells = new Dictionary<object, PUSimpleTableCell>();
		for(int i = activeTableCells.Count-1; i >= 0; i--) {
			PUSimpleTableCell cell = activeTableCells [i];
			if (cell.TestForVisibility () == false) {
				EnqueueTableCell (cell);
			} else {
				visibleCells [cell.cellData] = cell;
			}
		}


		float cellWidth = cellSize.Value.x;
		float cellHeight = cellSize.Value.y;

		// Can I skip a known quantity in the beginning and end?
		int totalVisibleCells = Mathf.CeilToInt(rectTransform.rect.height / cellHeight) + 1;
		int firstVisibleCell = Mathf.FloorToInt(Mathf.Abs (contentRectTransform.anchoredPosition.y) / cellHeight);


		// Update the content size
		contentRectTransform.sizeDelta = new Vector2 (cellWidth, cellHeight * allObjects.Count);

		y = -firstVisibleCell * cellHeight;
		nextY = y - cellHeight;

		for(int i = firstVisibleCell; i < firstVisibleCell + totalVisibleCells; i++) {
			if (i < allObjects.Count) {
				object myCellData = allObjects [i];

				PUSimpleTableCell cell = null;

				// Can I fit on the current line?
				if (x + cellWidth > (contentRectTransform.rect.width + 1)) {
					x = 0;
					y = nextY;
				}

				if (PUSimpleTableCell.TestForVisibility (y, rectTransform, contentRectTransform)) {
					if (visibleCells.ContainsKey (myCellData)) {
						cell = visibleCells [myCellData];
					} else {
						cell = DequeueTableCell (myCellData);
					}

					cell.puGameObject.rectTransform.anchoredPosition = new Vector2 (x, y);

					yield return null;
				}

				x += cellWidth;
				nextY = y - cellHeight;
			}
		}
	}

	public void ReloadTable() {

		if(gameObject.GetComponent<PUSimpleTableUpdateScript>() == null){
			tableUpdateScript = (PUSimpleTableUpdateScript)gameObject.AddComponent (typeof(PUSimpleTableUpdateScript));
			tableUpdateScript.table = this;
		}

		ClearTable ();

		if (asynchronous) {
			tableUpdateScript.ReloadTableCells ();
			return;
		}

		IEnumerator t = ReloadTableAsync();
		while (t.MoveNext ()) {}
	}

	public override void LateUpdate() {
		// If we've scrolled, retest cells to see who needs to load/unload
		RectTransform tableContentTransform = contentObject.transform as RectTransform;
		if (currentScrollY != (int)tableContentTransform.anchoredPosition.y) {
			
			IEnumerator t = ReloadTableAsync();
			while (t.MoveNext ()) {}

			currentScrollY = (int)tableContentTransform.anchoredPosition.y;
		}
	}

	public override void gaxb_complete() {

		base.gaxb_complete ();

		NotificationCenter.addObserver (this, "OnAspectChanged", null, (args, name) => {
			IEnumerator t = ReloadTableAsync();
			while (t.MoveNext ()) {}
		});

	}

	public override void unload() {

		foreach (PUSimpleTableCell cell in activeTableCells) {
			cell.unload ();
		}
		foreach (string key in pooledTableCells.Keys) {
			foreach (PUSimpleTableCell cell in pooledTableCells[key]) {
				cell.unload ();
			}
		}


		base.unload ();
	}

}
