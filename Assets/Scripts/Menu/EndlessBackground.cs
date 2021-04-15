using UnityEngine;
using System.Collections;

public class EndlessBackground : MonoBehaviour {

	public bool isChild;
	public bool isMainMenu;

	public float translationSpeed;
	public float adjustment;

	private float time;

	private Vector3 defaultPosition;
	private Vector3 newPosition;

	private SpriteRenderer sr;

	private GameManager manager;

	void Start () {

		sr = gameObject.GetComponentInChildren<SpriteRenderer> ();
		manager = GameObject.FindGameObjectWithTag("GameManager").GetComponentInChildren<GameManager>();
		defaultPosition = transform.position;
		newPosition = defaultPosition;

		if( isChild )
			time = -sr.bounds.size.x / -translationSpeed;
	}

	void Update () {

		newPosition.x = translationSpeed * time;
		transform.position = newPosition;

		if (Mathf.Abs (newPosition.x - defaultPosition.x) > sr.bounds.size.x)
			time -= sr.bounds.size.x / -translationSpeed * 2f + adjustment;

		if( !isMainMenu || isMainMenu && !manager.menuActive )
			time += Time.deltaTime;
	}
}
