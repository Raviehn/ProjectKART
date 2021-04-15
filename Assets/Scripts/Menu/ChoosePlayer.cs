using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ChoosePlayer : MonoBehaviour
{

    public float blinkFrequency;

    public Sprite[] icons;

    public Sprite[] okay;

    public AudioSource choose;
    public AudioSource forward;
    public AudioSource back;

    public SpriteRenderer[] driverSelection;

    public SpriteRenderer[] driverBackground;

    public SpriteRenderer[] confirmation;

    public GameObject[] driver;

    bool twoPlayers = false;

    int p1CurrentChoice = 0;
    int p2CurrentChoice = 4;

    float time;
    float waitingTime;
    float waitFor = 3f;

    enum State
    {
        Hovered, Selected, Confirmed
    }

    State state;

    Fader fader;

    void Start()
    {

        for (int i = 0; i < driverBackground.Length; i++)
            driverBackground[i].enabled = false;
        choose = GetComponent<AudioSource>();
        fader = GameObject.FindGameObjectWithTag("FadeManager").GetComponent<Fader>();
        twoPlayers = PlayerPrefs.GetString("Mode").StartsWith("2");
    }

    void Update()
    {

        GetInput();

        if (state == State.Confirmed && fader.black)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        for (int i = 0; i < driverSelection.Length; i++)
            driverSelection[i].sprite = icons[0];

        driverSelection[p1CurrentChoice].sprite = icons[1];

        for (int i = 0; i < driverBackground.Length; i++)
            driverBackground[i].enabled = false;

        switch (state)
        {

            case State.Hovered:
                confirmation[0].sprite = null;
                driver[p1CurrentChoice].GetComponentInChildren<ChoosePlayerAnimationController>().SetStateToDriving();

                // blinking icon
                if (time >= blinkFrequency * 2f)
                    driverSelection[p1CurrentChoice].sprite = icons[0];

                if (time >= blinkFrequency * 3f)
                    time -= blinkFrequency * 3f;
                break;
            case State.Selected:
                driverBackground[p1CurrentChoice].enabled = true;
                confirmation[0].sprite = okay[0];
                driver[p1CurrentChoice].GetComponentInChildren<ChoosePlayerAnimationController>().SetStateToSpinning();
                break;
            case State.Confirmed:
                driverBackground[p1CurrentChoice].enabled = true;
                confirmation[0].sprite = okay[1];
                driver[p1CurrentChoice].GetComponentInChildren<ChoosePlayerAnimationController>().SetStateToFacing();
                if (time >= waitFor)
                    fader.fadeOutTime = 0f;

                PlayerPrefs.SetInt("Player1", p1CurrentChoice);
                PlayerPrefs.SetInt("Player2", -1);

                waitingTime += Time.deltaTime;
                break;
        }

        time += Time.deltaTime;
    }

    void GetInput()
    {

        if (state == State.Hovered)
        {
            // Input Vertical
            if (Input.GetButtonDown("Up"))
            {
                if (p1CurrentChoice >= 4)
                    p1CurrentChoice -= 4;
                choose.Play();
            }

            if (Input.GetButtonDown("Down"))
            {
                if (p1CurrentChoice <= 3)
                    p1CurrentChoice += 4;
                choose.Play();
            }

            // Input Horizontal
            if (Input.GetButtonDown("Left"))
            {
                p1CurrentChoice -= 1;
                if (p1CurrentChoice < 0)
                    p1CurrentChoice = 7;
                choose.Play();
            }

            if (Input.GetButtonDown("Right"))
            {
                p1CurrentChoice += 1;
                if (p1CurrentChoice > 7)
                    p1CurrentChoice = 0;
                choose.Play();
            }
        }

        // Input Forward
        if (Input.GetButtonDown("A") || Input.GetButtonDown("Start"))
        {
            if (state != State.Confirmed)
            {
                if (state == State.Hovered)
                    state = State.Selected;
                else
                    state = State.Confirmed;
                forward.Play();
            }
        }

        // Input Back
        if (Input.GetButtonDown("B"))
        {
            if (state == State.Selected)
            {
                state = State.Hovered;
                back.Play();
            }
        }

    }

}
