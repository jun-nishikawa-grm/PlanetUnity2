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

	public void Start()
	{
		originalY = gameObject.transform.localPosition.y;
	}

	public void LateUpdate()
	{
		// Test world position is not above the table top; if so, clamp it?
		float diff = (table.contentObject.transform.localPosition.y - table.rectTransform.rect.height) + (gameObject.transform.localPosition.y+tableCell.puGameObject.rectTransform.rect.height);

		if (diff > 0) {
			Vector2 pos = gameObject.transform.localPosition;
			pos.y = originalY - diff;
			gameObject.transform.localPosition = pos;
		} else if(gameObject.transform.localPosition.y.Equals(originalY) == false) {
			Vector2 pos = gameObject.transform.localPosition;
			pos.y = originalY;
			gameObject.transform.localPosition = pos;
		}
	}
}

public class PUTableCell {

	public PUTable table = null;
	public PUGameObject puGameObject = null;
	public object cellData = null;

	public virtual bool IsHeader() {
		// Subclasses override this method to specify this cell should act as a section header
		return false;
	}

	public virtual string XmlPath() {
		// Subclasses override this method to supply a path to a planet unity xml for this cell
		return "(subclass needs to define an XmlPath)";
	}

	public virtual void LateUpdate() {
		// Subclasses can override to dynamically check their current size
	}

	public virtual void LoadIntoPUGameObject(PUTable parent, object data) {

		table = parent;
		cellData = data;

		puGameObject = (PUGameObject)PlanetUnity2.loadXML (PlanetUnityOverride.xmlFromPath(XmlPath ()), parent.contentObject, null);

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

		if (IsHeader ()) {
			PUTableHeaderScript script = (PUTableHeaderScript)puGameObject.gameObject.AddComponent (typeof(PUTableHeaderScript));
			script.table = table;
			script.tableCell = this;
		}

		// We want to bridge all notifications to my scope; this allows developers to handle notifications
		// at the table cell level, or at the scene controller level, with ease
		NotificationCenter.addObserver (this, "*", puGameObject, (args,name) => {
			NotificationCenter.postNotification(table.Scope(), name, args);
		});
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
