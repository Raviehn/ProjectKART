using UnityEngine;
using System.Collections;

public class GetParentSprite : MonoBehaviour {
	
	void Update () {
	
		GetComponentInChildren<SpriteRenderer>().sprite = transform.parent.GetComponentInChildren<SpriteRenderer>().sprite;
	}
}
