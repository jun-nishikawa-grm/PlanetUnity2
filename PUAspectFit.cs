

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class AspectRatioContentSize : MonoBehaviour
{
	public Vector2 contentSize;

	private RectTransform rectTransform;

	protected void LateUpdate()
	{
		rectTransform = gameObject.transform as RectTransform;
		float parentWidth = rectTransform.rect.size.x;
		float parentHeight = rectTransform.rect.size.y;

		if (parentWidth > 0 && parentHeight > 0) {
			float scaleX = parentWidth / contentSize.x;
			float scaleY = parentHeight / contentSize.y;
			if (scaleX > scaleY) {
				this.rectTransform.localScale = new Vector2 (scaleX, scaleX);
			} else {
				this.rectTransform.localScale = new Vector2 (scaleY, scaleY);
			}
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
	}		
}
