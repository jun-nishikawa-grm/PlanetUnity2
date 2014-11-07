
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
using UnityEngine.UI;
using System.Reflection;

public partial class PUInputField : PUInputFieldBase {


	public GameObject textGameObject;

	public InputField field;

	public override void gaxb_init ()
	{
		// We call Text's gaxb_init, which creates the appropriate text component on gameObject
		base.gaxb_init ();

		textGameObject = gameObject;

		// Next, we create a new gameObject, and put the Text-created gameObject inside me
		gameObject = new GameObject ("<InputField/>", typeof(RectTransform));
		gameObject.AddComponent<CanvasRenderer> ();
		gameObject.AddComponent<InputField> ();

		field = gameObject.GetComponent<InputField> ();

		// Move the text to be the child of the input field
		textGameObject.transform.SetParent (gameObject.transform, false);
		textGameObject.FillParentUI ();

		text.supportRichText = false;
		text.alignment = TextAnchor.UpperLeft;

		field.transition = Selectable.Transition.None;


		// This is probably not the best way to do this, but 4.60.f1 removed the onSubmit event
		field.onEndEdit.AddListener ((value) => {
			if(onValueChanged != null){
				NotificationCenter.postNotification (Scope (), this.onValueChanged, NotificationCenter.Args("sender", this));
			}
		});
	}

}
