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

using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public partial class PUGameObject : PUGameObjectBase {

	public GameObject gameObject;
	public RectTransform rectTransform;

	public CanvasGroup canvasGroup = null;

	private static Dictionary<string, Vector4> stringToAnchorLookup = null;

	public void SetParentGameObject(GameObject p)
	{
		gameObject.transform.SetParent (p.transform, false);
	}

	public virtual void gaxb_load(XmlReader reader, object _parent, Hashtable args)
	{
		if (stringToAnchorLookup == null) {
			stringToAnchorLookup = new Dictionary<string, Vector4> ();

			stringToAnchorLookup.Add ("top,left", new Vector4 (0, 1, 0, 1));
			stringToAnchorLookup.Add ("top,center", new Vector4 (0.5f, 1, 0.5f, 1));
			stringToAnchorLookup.Add ("top,right", new Vector4 (1, 1, 1, 1));
			stringToAnchorLookup.Add ("top,stretch", new Vector4 (0, 1, 1, 1));

			stringToAnchorLookup.Add ("middle,left", new Vector4 (0, 0.5f, 0, 0.5f));
			stringToAnchorLookup.Add ("middle,center", new Vector4 (0.5f, 0.5f, 0.5f, 0.5f));
			stringToAnchorLookup.Add ("middle,right", new Vector4 (1, 0.5f, 1, 0.5f));
			stringToAnchorLookup.Add ("middle,stretch", new Vector4 (0, 0.5f, 1, 0.5f));

			stringToAnchorLookup.Add ("bottom,left", new Vector4 (0, 0, 0, 0));
			stringToAnchorLookup.Add ("bottom,center", new Vector4 (0.5f, 0, 0.5f, 0));
			stringToAnchorLookup.Add ("bottom,right", new Vector4 (1, 0, 1, 0));
			stringToAnchorLookup.Add ("bottom,stretch", new Vector4 (0, 0, 1, 0));

			stringToAnchorLookup.Add ("stretch,left", new Vector4 (0, 0, 0, 1));
			stringToAnchorLookup.Add ("stretch,center", new Vector4 (0.5f, 0, 0.5f, 1));
			stringToAnchorLookup.Add ("stretch,right", new Vector4 (1, 0, 1, 1));
			stringToAnchorLookup.Add ("stretch,stretch", new Vector4 (0, 0, 1, 1));
		}

		base.gaxb_load (reader, _parent, args);
	}
		
	public virtual void gaxb_final(XmlReader reader, object _parent, Hashtable args)
	{
		if (gameObject == null) {
			gameObject = new GameObject ("<GameObject />", typeof(RectTransform));
		}

		gameObject.AddComponent<GameObjectRemoveFromNotificationCenter> ();

		if (titleExists) {
			gameObject.name = title;
		}

		if (_parent is GameObject) {
			SetParentGameObject (_parent as GameObject);
		} else if (_parent is PUGameObject) {
			PUGameObject parentEntity = (PUGameObject)_parent;
			SetParentGameObject (parentEntity.gameObject);
		}

		if (boundsExists) {
			this.SetPosition (new Vector3 (bounds.x, bounds.y, 0.0f));
			this.SetSize (new Vector2 (bounds.z, bounds.w));
		}

		UpdateRectTransform ();

		gameObject.layer = LayerMask.NameToLayer ("UI");

		if (mask) {
			gameObject.AddComponent<Mask> ();

			// Mask requires a Graphic; if we don't have one, add one and tell it now to draw it...
			if (gameObject.GetComponent<Graphic> () == null) {
				gameObject.AddComponent<RawImage> ();

				Mask maskComponent = gameObject.GetComponent<Mask> ();
				maskComponent.showMaskGraphic = false;
			}
		}

		if (outline) {
			gameObject.AddComponent<Outline> ();
		}

		if (shaderExists) {
			Graphic graphic = gameObject.GetComponent<Graphic> ();
			if (graphic != null) {
				graphic.material = new Material (Shader.Find (shader));
			}
		}

		gameObject.SetActive (active);
	}

	public virtual void removeChild(object child){
		children.Remove (child);
	}

	public virtual void unload(){

		PUGameObject p = parent as PUGameObject;
		if (p != null) {
			p.removeChild (this);
		}

		unloadAllChildren ();

		NotificationCenter.removeObserver (this);

		GameObject.Destroy (gameObject);
		gameObject = null;
		rectTransform = null;
	}

	public virtual void unloadAllChildren(){
		for(int i = children.Count-1; i >= 0; i--) {
			PUGameObject p = children [i] as PUGameObject;
			p.unload ();
		}
		children.Clear ();
	}

	public void UpdateRectTransform() {

		rectTransform = gameObject.GetComponent<RectTransform> ();
		if (rectTransform != null) {
			rectTransform.pivot = pivot;

			rectTransform.localPosition = new Vector3 (0, 0, position.z);
			rectTransform.anchoredPosition = position;
			rectTransform.localScale = scale;
			rectTransform.localEulerAngles = rotation;

			RectTransform parentTransform = (RectTransform)gameObject.transform.parent;
			float parentW = Screen.width;
			float parentH = Screen.height;

			if (parentTransform != null) {
				Canvas rootCanvas = parentTransform.GetComponent<Canvas> ();

				if (rootCanvas == null || rootCanvas.isRootCanvas == false) {
					// Work around for unity issue where the rect transform of a sub canvas does not update soon enough
					if (rootCanvas != null && ((int)parentTransform.rect.width == 100 && (int)parentTransform.rect.height == 100) ) {

					} else {
						parentW = parentTransform.rect.width;
						parentH = parentTransform.rect.height;
					}
				}

				if ((int)size.x == 0) {
					size.x = parentW;
				}
				if ((int)size.y == 0) {
					size.y = parentH;
				}
			}

			rectTransform.sizeDelta = size;

			if (anchorExists) {
				int numCommas = anchor.NumberOfOccurancesOfChar (',');
				Vector4 values = new Vector4 ();

				if (numCommas == 1) {
					// english representation
					values = stringToAnchorLookup [anchor];
				}

				if (numCommas == 3) {
					// math representation
					values.PUParse (anchor);
				}

				rectTransform.anchorMin = new Vector2 (values.x, values.y);
				rectTransform.anchorMax = new Vector2 (values.z, values.w);

				// the sizeDelta is the amount left over after the anchors are calculated; therefore,
				// if we have set the anchors we need to adjust the sizeDelta
				float anchorDeltaX = rectTransform.anchorMax.x - rectTransform.anchorMin.x;
				float anchorDeltaY = rectTransform.anchorMax.y - rectTransform.anchorMin.y;

				float mySizeDeltaX = rectTransform.sizeDelta.x;
				float mySizeDeltaY = rectTransform.sizeDelta.y;

				mySizeDeltaX -= (parentW * anchorDeltaX);
				mySizeDeltaY -= (parentH * anchorDeltaY);

				rectTransform.sizeDelta = new Vector2 (mySizeDeltaX, mySizeDeltaY);
			}
		}
	}

	public void SetFrame(float x, float y, float w, float h, float pivotX, float pivotY, string anchor) {
		SetPosition (new Vector3 (x, y, 0));
		SetSize (new Vector2 (w, h));
		SetPivot (new Vector2 (pivotX, pivotY));
		SetAnchor (anchor);
	}

	public void LoadIntoPUGameObject(PUGameObject _parent)
	{
		// Sanity check to make sure this wasn't called multiple times
		if (gameObject == null) {
			gaxb_load (null, null, null);
			gaxb_init ();
			gaxb_final (null, _parent, null);
		}

		parent = _parent;

		if (_parent != null) {
			if (_parent is PUScrollRect) {
				gameObject.transform.SetParent ((_parent as PUScrollRect).contentObject.transform, false);
			} else {
				gameObject.transform.SetParent (_parent.gameObject.transform, false);
			}

			_parent.children.Add (this);
		}

		gaxb_complete ();
	}


	public void CheckCanvasGroup () {
		if (canvasGroup == null) {
			canvasGroup = gameObject.AddComponent<CanvasGroup> ();
		}
	}

	public void IgnoreMouse(bool i) {
		CheckCanvasGroup ();
		canvasGroup.blocksRaycasts = !i;
	}



	public void UnscheduleForUpdates() {
		GameObjectLateUpdateScript script1 = gameObject.GetComponent<GameObjectLateUpdateScript> ();
		if (script1 != null) {
			script1.enabled = false;
		}

		GameObjectUpdateScript script2 = gameObject.GetComponent<GameObjectUpdateScript> ();
		if (script2 != null) {
			script2.enabled = false;
		}
	}

	public void ScheduleForLateUpdate() {
		gameObject.AddComponent<GameObjectLateUpdateScript> ();
		GameObjectLateUpdateScript script = gameObject.GetComponent<GameObjectLateUpdateScript> ();
		script.entity = this;
	}

	public void ScheduleForUpdate() {
		gameObject.AddComponent<GameObjectUpdateScript> ();
		GameObjectUpdateScript script = gameObject.GetComponent<GameObjectUpdateScript> ();
		script.entity = this;
	}

	public virtual void Update() {

	}

	public virtual void LateUpdate() {

	}

	#region Performance Wrapped Setters For RectTransform
	public void RectTransformSafeSetLocalScale(Vector2 t){
		if (rectTransform != null && Vector2.Distance(rectTransform.localScale, t) > 0.001f) {
			rectTransform.localScale = t;
		}
	}

	public void RectTransformSafeSetPivot(Vector2 t){
		if (rectTransform != null && Vector2.Distance(rectTransform.pivot, t) > 0.001f) {
			rectTransform.pivot = t;
		}
	}

	public void RectTransformSafeSetAnchoredPosition(Vector2 t){
		if (rectTransform != null && Vector2.Distance(rectTransform.anchoredPosition, t) > 0.001f) {
			rectTransform.anchoredPosition = t;
		}
	}

	public void RectTransformSafeSetLocalEulerAngles(Vector3 t){
		if (rectTransform != null && Vector2.Distance(rectTransform.localEulerAngles, t) > 0.001f) {
			rectTransform.localEulerAngles = t;
		}
	}
	#endregion
}

public class GameObjectRemoveFromNotificationCenter : MonoBehaviour {
	void OnDestroy() {
		NotificationCenter.removeObserver (this);
	}
}

public class GameObjectLateUpdateScript : MonoBehaviour {
	public PUGameObject entity;

	public void LateUpdate() {
		entity.LateUpdate ();
	}
}

public class GameObjectUpdateScript : MonoBehaviour {
	public PUGameObject entity;

	public void Update() {
		entity.Update ();
	}
}
