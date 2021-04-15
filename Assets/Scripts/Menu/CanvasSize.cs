using UnityEngine;
using System.Collections;

public class CanvasSize : MonoBehaviour {
	
	void Update () {
	
		gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2 ( Screen.width, Screen.height );
	}
}
