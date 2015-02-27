

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public partial class PUClearButton : PUClearButtonBase {

	public Button button;
	public InvisibleHitGraphic graphic;
	public CanvasRenderer canvasRenderer;

	public override void gaxb_init ()
	{
		gameObject = new GameObject ("<ClearButton/>", typeof(RectTransform));

		canvasRenderer = gameObject.AddComponent<CanvasRenderer> ();
		graphic = gameObject.AddComponent<InvisibleHitGraphic> ();

		button = gameObject.AddComponent<Button> ();

		if (onTouchUp != null) {

			button.onClick.AddListener(() => { 
				NotificationCenter.postNotification (Scope (), this.onTouchUp, NotificationCenter.Args("sender", this));
			}); 
		}
	}

}
