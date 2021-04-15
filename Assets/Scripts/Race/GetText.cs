using UnityEngine;
using UnityEngine.UI;

public class GetText : MonoBehaviour {

    public Transform targetTransform;

    Text text;
    Text targetText;

    void Start()
    {
        text = GetComponent<Text>();
        targetText = targetTransform.GetComponent<Text>();
    }
	
	void Update () {

        text.text = targetText.text;
    }
}
