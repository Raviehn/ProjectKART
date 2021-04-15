using UnityEngine;
using System.Collections;

public class FitToScreen : MonoBehaviour {

	public int offset;

	private Vector2 position;
	private Vector2 translatedPanelPosition;

	private RectTransform rect;

	void Start () {

		rect = GetComponentInChildren<RectTransform>();
		position = rect.anchoredPosition;
		translatedPanelPosition.x = position.x;
	}

	void Update () {
	
		translatedPanelPosition.y = - ( ( ( Screen.height - 224f ) / 2f ) + offset );
		rect.anchoredPosition = translatedPanelPosition;
	}
}
