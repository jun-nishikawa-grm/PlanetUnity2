using System;


public partial class PUSwitcher : PUSwitcherBase {

	private int currentIndex = -1;

	public override void gaxb_complete() {
		foreach (object childObj in children) {
			PUGameObject child = childObj as PUGameObject;
			child.CheckCanvasGroup ();
			child.canvasGroup.alpha = 0;
			child.gameObject.SetActive (false);
		}

		SwitchTo (0);
	}

	public int CurrentIndex() {
		return currentIndex;
	}

	public void SwitchTo(int i) {
		if (currentIndex == i) {
			return;
		}
			
		foreach (PUGameObject child in children) {
			#if PU2_LEANTWEEN
				LeanTween.cancel (child.gameObject);
			#endif
			child.gameObject.SetActive (false);
		}

		HideIndex (currentIndex, 0.0f);
		currentIndex = i;
		ShowIndex (currentIndex, 0.15f);
	}

	private void HideIndex(int idx, float delay) {
		if (idx >= 0 && idx < children.Count) {
			PUGameObject child = children [idx] as PUGameObject;

			child.gameObject.SetActive (true);
			child.canvasGroup.alpha = 1;
			#if PU2_LEANTWEEN
				LeanTween.alpha (child.gameObject, 0.0f, 1.13f).setEase (LeanTweenType.easeOutCubic).setDelay(delay).setOnComplete (() => {
					child.gameObject.SetActive (false);
				});
			#else
				child.gameObject.SetActive (false);
			#endif
		}
	}

	private void ShowIndex(int idx, float delay) {
		if (idx >= 0 && idx < children.Count) {
			PUGameObject child = children [idx] as PUGameObject;

			child.gameObject.SetActive (true);
			child.canvasGroup.alpha = 0;
			#if PU2_LEANTWEEN
				LeanTween.alpha (child.gameObject, 1.0f, 1.13f).setEase (LeanTweenType.easeOutCubic).setDelay(delay).setOnComplete (() => {

				});
			#endif
		}
	}

	public void Close(Action block) {
		if (currentIndex >= 0 && currentIndex < children.Count) {
			PUGameObject child = children [currentIndex] as PUGameObject;

			child.gameObject.SetActive (true);
			child.canvasGroup.alpha = 1;
			#if PU2_LEANTWEEN
			LeanTween.alpha (child.gameObject, 0.0f, 1.13f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
				child.gameObject.SetActive (false);
				block ();
			});
			#else
			child.gameObject.SetActive (false);
			block();
			#endif
		} else {
			block();
		}
	}

}
