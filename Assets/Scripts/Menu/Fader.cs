using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Fader : MonoBehaviour
{
    public bool isMenu;

    [HideInInspector]
    public bool black = true;
    [HideInInspector]
    public bool noFade = false;

    public SpriteRenderer background;
    public float fadeSpeed;
    public float fadeOutTime;

    float time;
    float fadeTime;

    GameManager manager;

    void Awake()
    {
        GetComponent<Image>();

        if (isMenu)
            manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // no fade while menu is active
        if (noFade)
            time = 0f;

        if (!black && time >= fadeOutTime && fadeOutTime != -1f)
            FadeOut();
        else if (black && time <= 1f / fadeSpeed)
            FadeIn();

        time += Time.deltaTime;
    }

    void FadeIn()
    {
        background.color = Color.Lerp(Color.black, Color.clear, fadeSpeed * fadeTime);
        fadeTime += Time.deltaTime;

        if (fadeSpeed * fadeTime >= 1f)
        {
            black = false;
            fadeTime = 0f;
        }        
    }

    void FadeOut()
    {
        background.color = Color.Lerp(Color.clear, Color.black, fadeSpeed * fadeTime);
        fadeTime += Time.deltaTime;

        if (fadeSpeed * fadeTime >= 1f)
        {
            black = true;
            fadeTime = 0f;
        }        
    }
}
