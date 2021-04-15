using UnityEngine;
using System.Collections;
using Text = UnityEngine.UI.Text;

public class GetParentTextAndSize : MonoBehaviour {

	void Update () {

		GetComponentInChildren<Text>().text = transform.parent.GetComponentInChildren<Text>().text;
		GetComponentInChildren<Text>().fontSize = transform.parent.GetComponentInChildren<Text>().fontSize;
	}
}
