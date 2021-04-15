using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LogoFadeOut : MonoBehaviour {

	public float fadeSpeed;
	public float delay;
	public float changeScene;

	private SpriteRenderer sr;

	void Start () {

		sr = gameObject.GetComponent< SpriteRenderer >();
	}

	void Update () {
	
		sr.color = Color.Lerp( Color.white, Color.clear, fadeSpeed * ( Time.time - delay ) );

		if( Time.time >= changeScene )
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}
}
