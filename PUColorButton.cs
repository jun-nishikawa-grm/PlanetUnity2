
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
using System.Xml;
using System.Collections;

public partial class PUColorButton : PUColorButtonBase {

	public Button button;

	public override void gaxb_init ()
	{
		base.gaxb_init ();

		if (title == null) {
			gameObject.name = "<ColorButton/>";
		}

		button = gameObject.AddComponent<Button> ();

		ColorBlock colors = button.colors;

		if (pressedColor != null) {
			colors.pressedColor = pressedColor.Value;
		}

		colors.fadeDuration = 0;
		button.colors = colors;





		if (onTouchUp != null) {
		
			button.onClick.AddListener(() => { 
				NotificationCenter.postNotification (Scope (), this.onTouchUp, NotificationCenter.Args("sender", this));
			}); 
		}
	}

}
