using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextAlignment : MonoBehaviour {

    public bool centreX;

    public float positionTop;
    public float positionLeft;
    public float fontSize;

    Text text;
    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        if(!centreX)
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + Screen.width / positionLeft, - Screen.height / positionTop);
        else
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, - Screen.height / positionTop);

        text = GetComponent<Text>();
        text.fontSize = Mathf.RoundToInt(Screen.width / fontSize);
    }
}
