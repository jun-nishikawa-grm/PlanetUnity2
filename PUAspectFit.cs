using UnityEngine;
using UnityEngine.UI;

public partial class PUAspectFit : PUAspectFitBase {

	AspectRatioFitter fitter = null;

	public override void gaxb_init ()
	{
		gameObject = new GameObject ("<AspectFit/>", typeof(RectTransform));

		fitter = gameObject.AddComponent<AspectRatioFitter> ();
		fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
		fitter.aspectRatio = contentSize.Value.x / contentSize.Value.y;
	}		
}
