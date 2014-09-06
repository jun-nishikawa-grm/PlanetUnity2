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

public partial class PUText : PUTextBase {

	public Text text;
	public CanvasRenderer canvasRenderer;

	public override void gaxb_init ()
	{
		gameObject = new GameObject ("<Text/>", typeof(RectTransform));
		gameObject.AddComponent<CanvasRenderer> ();
		gameObject.AddComponent<Text> ();

		text = gameObject.GetComponent<Text> ();
		canvasRenderer = gameObject.GetComponent<CanvasRenderer> ();

		if (titleExists == false && valueExists) {
			gameObject.name = string.Format("\"{0}\"", value);
		}

		if (valueExists) {
			text.text = value;
		}

		if (fontColorExists) {
			text.color = fontColor;
		}

		if (fontExists) {
			text.font = PlanetUnityResourceCache.GetFont (font);
		} else {
			text.font = PlanetUnityResourceCache.GetFont("Arial");
		}

		if (fontSizeExists) {
			text.fontSize = fontSize;
		}

		if (fontStyleExists) {
			if(fontStyle == PlanetUnity2.FontStyle.bold)
				text.fontStyle = FontStyle.Bold;
			if(fontStyle == PlanetUnity2.FontStyle.italic)
				text.fontStyle = FontStyle.Italic;
			if(fontStyle == PlanetUnity2.FontStyle.normal)
				text.fontStyle = FontStyle.Normal;
			if(fontStyle == PlanetUnity2.FontStyle.boldAndItalic)
				text.fontStyle = FontStyle.BoldAndItalic;
		}

		if (alignmentExists) {
			if(alignment == PlanetUnity2.TextAlignment.center){
				text.alignment = TextAnchor.MiddleCenter;
			}
			if(alignment == PlanetUnity2.TextAlignment.left){
				text.alignment = TextAnchor.UpperLeft;
			}
			if(alignment == PlanetUnity2.TextAlignment.right){
				text.alignment = TextAnchor.UpperRight;
			}
		}
	}

}
