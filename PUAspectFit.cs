

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class AspectRatioContentSize : AspectRatioFitter
{
	public Vector2 contentSize;

	private RectTransform rectTransform;

	protected override void OnRectTransformDimensionsChange()
	{
		rectTransform = gameObject.transform as RectTransform;
		float parentWidth = rectTransform.rect.size.x;
		float parentHeight = rectTransform.rect.size.y;

		if (parentWidth < contentSize.x || parentHeight < contentSize.y) {
			base.OnRectTransformDimensionsChange ();
		}
	}
}


public partial class PUAspectFit : PUAspectFitBase {

	AspectRatioContentSize fitter = null;

	public override void gaxb_init ()
	{
		gameObject = new GameObject ("<AspectFit/>", typeof(RectTransform));

		gameObject.AddComponent<AspectRatioContentSize> ();
		fitter = gameObject.GetComponent<AspectRatioContentSize> ();
		fitter.contentSize = contentSize;

		fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
		fitter.aspectRatio = contentSize.x / contentSize.y;
	}		
}
