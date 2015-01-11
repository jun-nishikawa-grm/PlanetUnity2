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

public class PUTableUpdateScript : MonoBehaviour {

	public PUTable table;

	public void LateUpdate() {
		table.LateUpdate ();
	}
}

public class PUTableHeaderScript : MonoBehaviour {

	public PUTable table;
	public PUTableCell tableCell;
	private float originalY;

	RectTransform cellTransform;
	RectTransform nextHeaderTransform;
	RectTransform tableTransform;
	RectTransform tableContentTransform;

	public void Start()
	{
		if (cellTransform == null) {
			cellTransform = tableCell.puGameObject.rectTransform;
		}
		if (tableTransform == null) {
			tableTransform = table.rectTransform as RectTransform;
		}
		if (tableContentTransform == null) {
			tableContentTransform = table.contentObject.transform as RectTransform;
		}
		if (nextHeaderTransform == null) {
			if ((cellTransform.GetSiblingIndex () + 1) < tableContentTransform.childCount) {
				nextHeaderTransform = tableContentTransform.GetChild (cellTransform.GetSiblingIndex () + 1) as RectTransform;
				if (nextHeaderTransform.GetComponent<PUTableHeaderScript> () == null) {
					nextHeaderTransform = null;
				}
			}
		}

		tableCell.puGameObject.CheckCanvasGroup ();

		originalY = cellTransform.anchoredPosition.y;
	}

	public Vector2 DesiredAnchorPosition() {
		float bottomOfCell = (cellTransform.anchoredPosition.y) + tableContentTransform.anchoredPosition.y - (tableContentTransform.rect.height - tableTransform.rect.height);
		float topOfCell = bottomOfCell + cellTransform.rect.height;
		float tableViewHeight = tableTransform.rect.height;

		if (topOfCell > tableViewHeight) {
			Vector2 pos = cellTransform.anchoredPosition;
			pos.y = originalY + (tableViewHeight - topOfCell);
			return pos;
		} else if(cellTransform.anchoredPosition.y.Equals(originalY) == false) {
			Vector2 pos = cellTransform.anchoredPosition;
			pos.y = originalY;
			return pos;
		}

		return cellTransform.anchoredPosition;
	}

	public void LateUpdate()
	{
		cellTransform.anchoredPosition = DesiredAnchorPosition ();

		float bottomOfCell = (cellTransform.anchoredPosition.y) + tableContentTransform.anchoredPosition.y - (tableContentTransform.rect.height - tableTransform.rect.height);
		Vector2 otherPos = cellTransform.anchoredPosition;
		float distance = 99999.0f;

		// Get my sibling's top of cell, allow it to push me out of the way
		if (nextHeaderTransform != null) {
			PUTableHeaderScript otherScript = nextHeaderTransform.GetComponent<PUTableHeaderScript> ();
			otherPos = otherScript.DesiredAnchorPosition ();
			distance = cellTransform.anchoredPosition.y - otherPos.y;
		}

		if (distance < cellTransform.rect.height) {
			cellTransform.anchoredPosition = new Vector2 (otherPos.x, otherPos.y + cellTransform.rect.height);
			tableCell.puGameObject.canvasGroup.alpha = LeanTween.easeInCubic (0, 1, distance / cellTransform.rect.height);
		} else if (bottomOfCell < 0) {
			tableCell.puGameObject.canvasGroup.alpha = LeanTween.easeInCubic (0, 1, (bottomOfCell+cellTransform.rect.height) / cellTransform.rect.height);
		} else {
			if (tableCell.puGameObject.canvasGroup.alpha.Equals (1.0f) == false) {
				tableCell.puGameObject.canvasGroup.alpha = 1.0f;
			}
		}

	}
}

public class PUTableCell {

	public PUTable table = null;
	public PUGameObject puGameObject = null;
	public object cellData = null;


	private GameObject cellGameObject;
	private RectTransform cellTransform;
	private RectTransform tableTransform;
	private RectTransform tableContentTransform;

	public virtual bool IsHeader() {
		// Subclasses override this method to specify this cell should act as a section header
		return false;
	}

	public virtual string XmlPath() {
		// Subclasses override this method to supply a path to a planet unity xml for this cell
		return null;
	}

	public virtual void LateUpdate() {
		// Subclasses can override to dynamically check their current size
	}

	public void TestForVisibility() {

		// Note to self: cell are arranged from bottom of content object (pos 0) to the top (pos height)
		// Note to self: table scrolling is positive (as you scroll up, the anchored position increases)
		if (IsHeader () == false) {
			float bottomOfCell = (cellTransform.anchoredPosition.y) + tableContentTransform.anchoredPosition.y - (tableContentTransform.rect.height - tableTransform.rect.height);
			float topOfCell = bottomOfCell + cellTransform.rect.height;
			float tableViewHeight = tableTransform.rect.height;

			if (cellGameObject.activeSelf) {
				if (bottomOfCell > tableViewHeight || topOfCell < 0) {
					cellGameObject.SetActive (false);
				}
			} else {
				if (bottomOfCell < tableViewHeight && topOfCell > 0) {
					cellGameObject.SetActive (true);
				}
			}
		}
	}

	public virtual void LoadIntoPUGameObject(PUTable parent, object data) {

		table = parent;
		cellData = data;

		string xmlPath = XmlPath ();

		if (xmlPath != null) {
			puGameObject = (PUGameObject)PlanetUnity2.loadXML (PlanetUnityOverride.xmlFromPath (xmlPath), parent.contentObject, null);

			// Attach all of the PlanetUnity objects
			try {
				FieldInfo field = this.GetType ().GetField ("scene");
				if (field != null) {
					field.SetValue (this, puGameObject);
				}

				puGameObject.PerformOnChildren (val => {
					PUGameObject oo = val as PUGameObject;
					if (oo != null && oo.title != null) {
						field = this.GetType ().GetField (oo.title);
						if (field != null) {
							field.SetValue (this, oo);
						}
					}
					return true;
				});
			} catch (Exception e) {
				UnityEngine.Debug.Log ("TableCell error: " + e);
			}

			try {
				// Attach all of the named GameObjects
				FieldInfo[] fields = this.GetType ().GetFields ();
				foreach (FieldInfo field in fields) {
					if (field.FieldType == typeof(GameObject)) {

						GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll (typeof(GameObject));

						foreach (GameObject pObject in pAllObjects) {
							if (pObject.name.Equals (field.Name)) {
								field.SetValue (this, pObject);
							}
						}
					}
				}
			} catch (Exception e) {
				UnityEngine.Debug.Log ("TableCell error: " + e);
			}
		} else {
			puGameObject = new PUGameObject ();
			puGameObject.SetFrame (0, 0, 0, 60, 0, 0, "bottom,stretch");
			puGameObject.LoadIntoPUGameObject (parent);
			puGameObject.gameObject.transform.SetParent(parent.contentObject.transform, false);
		}

		if (IsHeader ()) {
			PUTableHeaderScript script = (PUTableHeaderScript)puGameObject.gameObject.AddComponent (typeof(PUTableHeaderScript));
			script.table = table;
			script.tableCell = this;
		}

		puGameObject.parent = table;

		// We want to bridge all notifications to my scope; this allows developers to handle notifications
		// at the table cell level, or at the scene controller level, with ease
		NotificationCenter.addObserver (this, "*", puGameObject, (args,name) => {
			NotificationCenter.postNotification(table.Scope(), name, args);
		});

		cellGameObject = puGameObject.gameObject;
		cellTransform = cellGameObject.transform as RectTransform;
		tableTransform = table.rectTransform;
		tableContentTransform = table.contentObject.transform as RectTransform;
	}

	public void unload() {
		NotificationCenter.removeObserver (this);
		puGameObject.unload ();
	}
}

public partial class PUTable : PUTableBase {

	public List<object> allObjects = null;
	List<PUTableCell> allCells = new List<PUTableCell>();


	public void SetObjectList(List<object> objects) {
		allObjects = new List<object> (objects);
	}

	public PUTableCell TableCellForPUGameObject(PUGameObject baseObject){
		foreach(PUTableCell cell in allCells){
			if (cell.puGameObject.Equals (baseObject)) {
				return cell;
			}
		}
		return null;
	}

	public object ObjectForTableCell(PUTableCell cell){
		int idx = allCells.IndexOf (cell);
		return allObjects [(allObjects.Count-idx)-1];
	}

	public void LoadCellForData(object cellData) {

		CalculateContentSize ();

		string className = cellData.GetType ().Name + "TableCell";

		Type cellType = Type.GetType (className, true);

		PUTableCell cell = (Activator.CreateInstance (cellType)) as PUTableCell;
		cell.LoadIntoPUGameObject (this, cellData);
		allCells.Add (cell);

		cell.puGameObject.rectTransform.anchoredPosition = new Vector3(0,(contentObject.transform as RectTransform).rect.height,0);
	}

	public void ReloadTable() {

		if(gameObject.GetComponent<PUTableUpdateScript>() == null){
			PUTableUpdateScript script = (PUTableUpdateScript)gameObject.AddComponent (typeof(PUTableUpdateScript));
			script.table = this;
		}

		// 0) Remove all previous content
		foreach (PUTableCell cell in allCells) {
			cell.puGameObject.unload ();
		}
		allCells.Clear ();

		if (allObjects == null || allObjects.Count == 0) {
			return;
		}

		// 1) Run through allObjects; instantiate a cell object based on said object class
		for(int i = allObjects.Count-1; i >= 0; i--) {
			LoadCellForData(allObjects[i]);
		}

		//foreach (PUTableCell cell in allCells) {
		for(int i = allCells.Count-1; i >= 0; i--){
			PUTableCell cell = allCells [i];
			if (cell.IsHeader ()) {
				// TODO: Move me to the end of the stuff
				cell.puGameObject.gameObject.transform.SetParent (cell.puGameObject.gameObject.transform.parent, false);
			}
		}

		CalculateContentSize ();
	}


	public void LateUpdate() {

		float y = 0;

		// 0) Remove all previous content
		foreach (PUTableCell cell in allCells) {
			cell.LateUpdate ();

			cell.TestForVisibility ();

			cell.puGameObject.rectTransform.anchoredPosition = new Vector2 (0, y);

			y += cell.puGameObject.rectTransform.rect.height;
		}

		CalculateContentSize ();

	}

	public override void unload() {

		foreach (PUTableCell cell in allCells) {
			cell.unload ();
		}
		base.unload ();
	}

}
