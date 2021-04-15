using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class BlinkingCursor : MonoBehaviour {

	private float frequency = 100f;
	private float time;

	private Image image;

	void Start() {

		image = GetComponentInChildren<Image>();
	}

	void Update () {

		image.color = new Vector4( 1f, Mathf.Sin ( time * frequency ) + 1 / 2f, 1f, 1f );

		time += Time.deltaTime;
	}
}
