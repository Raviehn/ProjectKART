using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuPanelController : MonoBehaviour
{

    [HideInInspector]
    public bool menuActive;

    public float speed;
    public float restartTime;

    public AudioSource forward;
    public AudioSource back;
    public AudioSource choose;

    public RectTransform choice;

    public Sprite[] menuBackgrounds;
    public Sprite[] choiceIcons;

    bool finalConfirm;
    bool changeSprite = true;
    bool opaque = false;
    bool playSound = false;

    int numOfChoices;
    int currentChoice = 0;
    
    float time;

    string menu = "";

    Vector2 scale;
    Vector2 panelPosition;
    Vector2 translatedPanelPosition;
    Vector2 choosePosition;

    Vector2 finalTop = new Vector2(-5f, -(((Screen.height - 224f) / 2f) + 176f));
    Vector2 finalBottom = new Vector2(-5f, -(((Screen.height - 224f) / 2f) + 185f));

    RectTransform background;

    Image image;
    Image choiceImage;

    GameManager manager;

    Fader fader;

    enum State
    {
        Show, Hide, Keep, Finish
    }

    State state;

    enum Sound
    {
        Forward, Backward
    }

    Sound soundToPlay;

    void Start()
    {
        state = State.Keep;
        background = gameObject.GetComponentInChildren<RectTransform>();
        image = gameObject.GetComponentInChildren<Image>();
        choiceImage = choice.GetComponentInChildren<Image>();
        panelPosition.x = background.anchoredPosition.x;
        panelPosition.y = background.anchoredPosition.y;
        choosePosition = choice.anchoredPosition;
        choice.GetComponentInChildren<Image>().enabled = false;
        manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        fader = GameObject.FindGameObjectWithTag("FadeManager").GetComponent<Fader>();
    }

    void Update()
    {
        if (menu.Length > 0)
        {
            menuActive = true;
            fader.noFade = true;
        }
        else if (!opaque || state == State.Finish)
        {
            menuActive = false;
            fader.noFade = false;
        }

        if (state == State.Finish && fader.black)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        manager.menuActive = menuActive;

        GetInput();

        if (playSound)
        {
            switch (soundToPlay)
            {
                case Sound.Forward:
                    forward.Play();
                    break;
                case Sound.Backward:
                    back.Play();
                    break;
            }
            playSound = false;
        }

        if (finalConfirm)
        {
            if (currentChoice == 0)
                choice.gameObject.transform.GetChild(0).GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(22f, 0f);
            else
                choice.gameObject.transform.GetChild(0).GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(14f, 0f);
        }

        switch (state)
        {
            case State.Show:
                Show();
                break;
            case State.Hide:
                Hide();
                break;
            case State.Finish:
                fader.fadeOutTime = 0f;
                break;
            default:
                break;
        }

        // restart menu
        if (time >= restartTime)
        {
            if (menuActive)
                time -= restartTime;
            else
                SceneManager.LoadScene(1);
        }

        time += Time.deltaTime;
    }

    void GetInput()
    {

        // Input Vertical
        if (Input.GetButtonDown("Up"))
        {
            if (currentChoice > 0 && !finalConfirm)
            {
                choosePosition.y += 16f;
                currentChoice--;
                choose.Play();
            }
        }

        if (Input.GetButtonDown("Down"))
        {
            if (currentChoice < numOfChoices && !finalConfirm)
            {
                choosePosition.y -= 16f;
                currentChoice++;
                choose.Play();
            }
        }

        // Input Horizontal
        if (Input.GetButtonDown("Left"))
        {
            if (currentChoice > 0 && finalConfirm)
            {
                choosePosition.x -= 40f;
                currentChoice--;
                choose.Play();
            }
        }

        if (Input.GetButtonDown("Right"))
        {
            if (currentChoice < numOfChoices && finalConfirm)
            {
                choosePosition.x += 40f;
                currentChoice++;
                choose.Play();
            }
        }

        choice.anchoredPosition = choosePosition;

        // Input Forward
        if (Input.GetButtonDown("A") || Input.GetButtonDown("Start"))
        {
            if (state != State.Keep)
                return;

            menu += currentChoice;
            changeSprite = true;
            if (menu.Length == 1)
                state = State.Show;
            else
                state = State.Hide;
            soundToPlay = Sound.Forward;
            playSound = true;
            ProceedChoice();
        }

        // Input Back
        if (Input.GetButtonDown("B"))
        {
            if (menu.Length == 0 || state != State.Keep)
                return;

            if (menu.Length > 1)
            {
                menu = menu.Remove(menu.Length - 1);
                changeSprite = true;
            }
            else
                menu = "";
            state = State.Hide;
            soundToPlay = Sound.Backward;
            playSound = true;
            ProceedChoice();
        }
    }

    void ProceedChoice()
    {

        if (changeSprite)
        {

            finalConfirm = true;

            switch (menu)
            {

                case "":
                    finalConfirm = false;
                    break;
                case "0":
                    numOfChoices = 1;
                    choosePosition.x = -38f;
                    finalConfirm = false;
                    break;
                case "00":
                    numOfChoices = 1;
                    choosePosition.x = -50f;
                    finalConfirm = false;
                    PlayerPrefs.SetInt("Players", 1);
                    break;
                case "000":
                    numOfChoices = 2;
                    choosePosition.x = -46f;
                    finalConfirm = false;
                    PlayerPrefs.SetInt("Mode", 0);
                    break;
                case "0000":
                    numOfChoices = 1;
                    choosePosition = finalBottom;
                    PlayerPrefs.SetInt("Difficulty", 0);
                    break;
                case "001":
                    numOfChoices = 1;
                    choosePosition = finalTop;
                    PlayerPrefs.SetInt("Mode", 1);
                    break;
                case "01":
                    numOfChoices = 2;
                    choosePosition.x = -50f;
                    finalConfirm = false;
                    PlayerPrefs.SetInt("Players", 2);
                    break;
                case "0001":
                    numOfChoices = 1;
                    choosePosition = finalBottom;
                    PlayerPrefs.SetInt("Difficulty", 1);
                    break;
                case "010":
                    numOfChoices = 2;
                    choosePosition.x = -46f;
                    finalConfirm = false;
                    PlayerPrefs.SetInt("Mode", 0);
                    break;
                case "011":
                    numOfChoices = 1;
                    choosePosition = finalTop;
                    PlayerPrefs.SetInt("Mode", 2);
                    break;
                case "012":
                    numOfChoices = 1;
                    choosePosition = finalTop;
                    PlayerPrefs.SetInt("Mode", 3);
                    break;
                case "0002":
                    numOfChoices = 1;
                    choosePosition = finalBottom;
                    PlayerPrefs.SetInt("Difficulty", 2);
                    break;
                case "0100":
                    numOfChoices = 1;
                    choosePosition = finalBottom;
                    PlayerPrefs.SetInt("Difficulty", 0);
                    break;
                case "0101":
                    numOfChoices = 1;
                    choosePosition = finalBottom;
                    PlayerPrefs.SetInt("Difficulty", 1);
                    break;
                case "0102":
                    numOfChoices = 1;
                    choosePosition = finalBottom;
                    PlayerPrefs.SetInt("Difficulty", 2);
                    break;
                case "00000":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "1PGP50");
                    break;
                case "00010":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "1PGP100");
                    break;
                case "00020":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "1PGP150");
                    break;
                case "01000":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "2PGP50");
                    break;
                case "01010":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "2PGP100");
                    break;
                case "01020":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "2PGP150");
                    break;
                case "0010":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "1PTT");
                    break;
                case "0110":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "2PMR");
                    break;
                case "0120":
                    state = State.Finish;
                    //PlayerPrefs.SetString("Mode", "2PBM");
                    break;
                default:
                    menu = "0";
                    numOfChoices = 1;
                    choosePosition.x = -38f;
                    finalConfirm = false;
                    soundToPlay = Sound.Backward;
                    break;
            }

            if (state == State.Finish)
                time = 0f;

            currentChoice = 0;
        }
    }

    void LoadImage()
    {

        if (!finalConfirm)
            choosePosition.y = -(((Screen.height - 224f) / 2f) + 133f);


        for (int i = 0; i < menuBackgrounds.Length; i++)
        {
            if (menuBackgrounds[i].name == menu)
                image.sprite = menuBackgrounds[i];
        }

        if (finalConfirm)
        {
            choiceImage.sprite = choiceIcons[1];

            choice.sizeDelta = new Vector2(14f, 8f);
            choice.gameObject.transform.GetChild(0).GetComponentInChildren<RectTransform>().anchoredPosition = new Vector2(22f, 0f);

            translatedPanelPosition.y = -(((Screen.height - 224f) / 2f) + 152f);
        }
        else
        {
            choiceImage.sprite = choiceIcons[0];
            choice.sizeDelta = new Vector2(10f, 10f);
            translatedPanelPosition.y = -(((Screen.height - 224f) / 2f) + image.sprite.rect.height / 2f + 121f);
        }

        choice.anchoredPosition = choosePosition;

        background.sizeDelta = new Vector2(image.sprite.rect.width, image.sprite.rect.height);
        background.anchoredPosition = translatedPanelPosition;
        changeSprite = false;

        state = State.Show;
    }

    void Show()
    {

        LoadImage();

        if (scale.x < 125f)
        {
            opaque = false;
            scale.x += speed * Time.deltaTime;
            scale.y += speed * Time.deltaTime * image.sprite.rect.height / image.sprite.rect.width;

            background.sizeDelta = scale;
        }
        else
        {
            background.sizeDelta = new Vector2(125f, image.sprite.rect.height);
            opaque = true;
            state = State.Keep;
            choice.GetComponentInChildren<Image>().enabled = true;
            if (finalConfirm)
                choice.gameObject.transform.GetChild(0).GetComponentInChildren<Image>().enabled = true;
        }
    }

    void Hide()
    {
        if (state == State.Finish)
            return;

        if (scale.x > 0f)
        {
            choice.GetComponentInChildren<Image>().enabled = false;
            if (!finalConfirm)
                choice.gameObject.transform.GetChild(0).GetComponentInChildren<Image>().enabled = false;
            opaque = true;
            scale.x -= speed * Time.deltaTime;
            scale.y -= speed * Time.deltaTime * image.sprite.rect.height / image.sprite.rect.width;

            background.sizeDelta = scale;
        }
        else
        {
            background.sizeDelta = Vector2.zero;
            opaque = false;

            if (menu.Length == 0)
                state = State.Keep;
            else
                state = State.Show;
        }
    }
}
