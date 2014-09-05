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

public partial class PUGameObject : PUGameObjectBase {

	public GameObject gameObject;
	public RectTransform rectTransform;

	public void SetParentGameObject(GameObject p)
	{
		gameObject.transform.parent = p.transform;
	}
		
	public void gaxb_final(XmlReader reader, object _parent, Hashtable args)
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
		rectTransform.localPosition = position;
		rectTransform.localScale = scale;
		rectTransform.localEulerAngles = rotation;
		rectTransform.pivot = pivot;

		// If the width or height is 0, inherit the parents width or height
		RectTransform parentTransform = null;
		if (gameObject.transform.parent) {
			parentTransform = gameObject.transform.parent.GetComponent<RectTransform> ();
		}

		if (parentTransform != null) {
			if ((int)size.x == 0) {
				size.x = parentTransform.sizeDelta.x;
			}
			if ((int)size.y == 0) {
				size.y = parentTransform.sizeDelta.y;
			}
		}

		if (this is PUCanvas == false) {
			rectTransform.sizeDelta = size;
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
