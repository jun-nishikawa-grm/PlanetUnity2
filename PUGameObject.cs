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

public partial class PUGameObject : PUGameObjectBase {

	public GameObject gameObject;
	public RectTransform rectTransform;

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
			gameObject = new GameObject ("<GameObject />");
		}

		if (titleExists) {
			gameObject.name = title;
		}

		if (_parent is GameObject) {
			SetParentGameObject (_parent as GameObject);
		} else if (_parent is PUGameObject) {
			PUGameObject parentEntity = (PUGameObject)_parent;
			SetParentGameObject (parentEntity.gameObject);
		}

		rectTransform = gameObject.GetComponent<RectTransform> ();
		if (rectTransform != null) {

			// Never modofiy the RectTransform of the Canvas directly, it does bad things
			if (this is PUCanvas == false) {

				rectTransform.pivot = pivot;
				rectTransform.anchoredPosition = position;
				rectTransform.localScale = scale;
				rectTransform.localEulerAngles = rotation;

				RectTransform parentTransform = (RectTransform)gameObject.transform.parent;
				if ((int)size.x == 0) {
					size.x = parentTransform.sizeDelta.x;
				}
				if ((int)size.y == 0) {
					size.y = parentTransform.sizeDelta.y;
				}
				rectTransform.sizeDelta = size;


				if (anchorExists) {
					int numCommas = anchor.NumberOfOccurancesOfChar (',');
					Vector4 values = new Vector4 ();

					if (numCommas == 1) {
						// english representation
						values = stringToAnchorLookup[anchor];
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

					mySizeDeltaX -= (parentTransform.sizeDelta.x * anchorDeltaX);
					mySizeDeltaY -= (parentTransform.sizeDelta.y * anchorDeltaY);

					rectTransform.sizeDelta = new Vector2(mySizeDeltaX, mySizeDeltaY);

					/*
					if (values.x.Equals(values.z) == false) {
						rectTransform.offsetMax = new Vector2 (0, 0);
					}
					if (values.y.Equals(values.w) == false) {
						rectTransform.offsetMin = new Vector2 (0, 0);
					}
*/
					/*
					rectTransform.offsetMax = new Vector2 (0, 0);
					rectTransform.offsetMin = new Vector2 (0, 0);

					rectTransform.pivot = pivot;
					rectTransform.localPosition = position;
					rectTransform.localScale = scale;
					rectTransform.localEulerAngles = rotation;*/
				}
			}

		}

		gameObject.layer = LayerMask.NameToLayer ("UI");

		if (hidden) {
			gameObject.SetActive (false);
		}
	}

	public void unload(){
		GameObject.Destroy (gameObject);
		gameObject = null;
	}

}
