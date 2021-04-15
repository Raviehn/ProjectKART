using UnityEngine;
using System.Collections;

public class CameraRatio : MonoBehaviour {

	public int width;
	public int height;

	private float ratio;
	private float windowRatio;
	private float scaleHeight;
	private float scaleWidth;

	private Camera cam;
	private Rect rect;

	void Update () {

		ratio = width / height;
		windowRatio = (float)Screen.width / (float)Screen.height;
		scaleHeight = windowRatio / ratio;

		cam = GetComponent<Camera>();
		rect = cam.rect;

		if ( scaleHeight < 1f )	{
			
			rect.width = 1f;
			rect.height = scaleHeight;
			rect.x = 0;
			rect.y = ( 1f - scaleHeight ) / 2f;
			
			cam.rect = rect;
		} else {

			scaleWidth = 1f / scaleHeight;
			
			rect.width = scaleWidth;
			rect.height = 1f;
			rect.x = (1f - scaleWidth) / 2f;
			rect.y = 0;
			
			cam.rect = rect;
		}
	}
}
