using UnityEngine;
using System.Collections;
using Text = UnityEngine.UI.Text;
using Image = UnityEngine.UI.Image;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CourseSelectController : MonoBehaviour
{

    public RectTransform cursor;

    public AudioSource forward;
    public AudioSource back;
    public AudioSource choose;

    public Text[] columnLeft;
    public Text[] columnRight;

    public Text trackName;

    public SpriteRenderer courseRenderer;
    public SpriteRenderer groundRenderer;
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer foregroundRenderer;

    public Sprite[] courses;
    public Sprite[] grounds;
    public Sprite[] backgrounds;
    public Sprite[] foregrounds;

    public GameObject confirmation;
    public GameObject columnLeftGO;
    public GameObject columnRightGO;

    bool playSound = false;
    bool loadNextLevel;

    int currentChoice = 0;

    float time;
    float frequency = 100f;

    string choice = "0";

    Vector2 cursorPosition;
    Vector2 translatedCursorPosition;

    enum Column
    {
        Cups, Tracks, Confirm
    }

    Column column = Column.Cups;

    enum Sound
    {
        Forward, Backward
    }

    Sound soundToPlay;

    List<List<string>> cups = new List<List<string>>();

    List<string> mushroom = new List<string>();
    List<string> flower = new List<string>();
    List<string> star = new List<string>();
    List<string> special = new List<string>();

    Fader fader;
            
    void Start()
    {
        cursorPosition = cursor.anchoredPosition;
        translatedCursorPosition = cursorPosition;

        cups.Add(mushroom);
        cups.Add(flower);
        cups.Add(star);
        cups.Add(special);

        mushroom.Add("Mario Circuit 1");
        mushroom.Add("Donut Plains 1");
        mushroom.Add("Ghost Valley 1");
        mushroom.Add("Bowser Castle 1");
        mushroom.Add("Mario Circuit 2");

        flower.Add("Choco Island 1");
        flower.Add("Ghost Valley 2");
        flower.Add("Donut Plains 2");
        flower.Add("Bowser Castle 2");
        flower.Add("Mario Circuit 3");

        star.Add("Koopa Beach 1");
        star.Add("Choco Island 2");
        star.Add("Vanilla Lake 1");
        star.Add("Bowser Castle 3");
        star.Add("Mario Circuit 4");

        special.Add("Donut Plains 3");
        special.Add("Koopa Beach 2");
        special.Add("Ghost Valley 3");
        special.Add("Vanilla Lake 2");
        special.Add("Rainbow Road");

        columnLeft[0].text = "Mushroom Cup";
        columnLeft[1].text = "Flower Cup";
        columnLeft[2].text = "Star Cup";
        columnLeft[3].text = "Special Cup";

        SetText(mushroom);

        fader = GameObject.FindGameObjectWithTag("FadeManager").GetComponentInChildren<Fader>();
    }

    void Update()
    {
        if (loadNextLevel && fader.black)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            foreach (GameObject sm in GameObject.FindGameObjectsWithTag("SoundManager"))
                Destroy(sm);
        }

        confirmation.SetActive(false);
        columnLeftGO.SetActive(true);
        columnRightGO.SetActive(true);
        GetComponentInChildren<Image>().canvasRenderer.SetAlpha(1f);
        trackName.fontSize = 0;

        switch (choice.Length)
        {

            case 0:
                column = Column.Cups;
                translatedCursorPosition.x = -108.7f;
                break;
            case 1:
                column = Column.Tracks;
                translatedCursorPosition.x = -3.1f;
                break;
            case 2:
                column = Column.Confirm;
                translatedCursorPosition.y = -124f;
                confirmation.SetActive(true);
                columnLeftGO.SetActive(false);
                columnRightGO.SetActive(false);
                trackName.fontSize = 16;
                GetComponentInChildren<Image>().canvasRenderer.SetAlpha(0f);

                string track = cups[int.Parse(choice.Remove(1))][int.Parse(choice.Substring(1))];

                trackName.text = track;

                for (int i = 0; i < courses.Length; i++)
                {
                    if (courses[i].name == choice)
                        courseRenderer.sprite = courses[i];
                }
                for (int i = 0; i < grounds.Length; i++)
                {
                    if (grounds[i].name.Contains(track.Remove(5)))
                        groundRenderer.sprite = grounds[i];
                }
                for (int i = 0; i < backgrounds.Length; i++)
                {
                    if (backgrounds[i].name.Contains(track.Remove(5)))
                        backgroundRenderer.sprite = backgrounds[i];
                }
                for (int i = 0; i < foregrounds.Length; i++)
                {
                    if (foregrounds[i].name.Contains(track.Remove(5)))
                        foregroundRenderer.sprite = foregrounds[i];
                }

                break;
        }

        SetColour();
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

        time += Time.deltaTime;
    }

    void GetInput()
    {

        // Input Vertical
        if (Input.GetButtonDown("Up"))
        {

            if (column != Column.Confirm && currentChoice > 0)
            {

                if (column == Column.Cups && currentChoice == cups.Count - 1)
                    translatedCursorPosition.y += 18f;
                else
                    translatedCursorPosition.y += 14f;

                currentChoice--;
                choose.Play();
            }
        }

        if (Input.GetButtonDown("Down"))
        {

            if (column != Column.Confirm)
            {

                if (column == Column.Cups && currentChoice < cups.Count - 1 || column == Column.Tracks && currentChoice < cups[int.Parse(choice)].Count - 1)
                {

                    if (column == Column.Cups && currentChoice == cups.Count - 2)
                        translatedCursorPosition.y -= 18f;
                    else
                        translatedCursorPosition.y -= 14f;

                    currentChoice++;
                    choose.Play();
                }
            }
        }

        // Input Horizontal
        if (Input.GetButtonDown("Left"))
        {

            if (column == Column.Confirm && currentChoice > 0)
            {
                currentChoice--;
                choose.Play();
                translatedCursorPosition.x = 8f;

            }
            else if (column == Column.Tracks)
            {
                choice = choice.Remove(choice.Length - 1);

                currentChoice = 0;
                translatedCursorPosition.y = -38f;

                soundToPlay = Sound.Backward;
                playSound = true;
            }
        }

        if (Input.GetButtonDown("Right"))
        {

            if (column == Column.Confirm && currentChoice < 1)
            {

                currentChoice++;
                choose.Play();
                translatedCursorPosition.x = 47f;

            }
            else if (column == Column.Cups)
            {

                choice += currentChoice;
                SetText(cups[currentChoice]);
                currentChoice = 0;
                translatedCursorPosition.y = -38f;

                soundToPlay = Sound.Forward;
                playSound = true;
            }
        }

        // Input Forward
        if (Input.GetButtonDown("A") || Input.GetButtonDown("Start"))
        {

            if (column == Column.Tracks)
            {
                translatedCursorPosition.x = 8;
                choice += currentChoice;
                translatedCursorPosition.y = -38f;

                soundToPlay = Sound.Forward;
                playSound = true;

            }
            else if (column == Column.Confirm)
            {

                if (currentChoice == 1)
                {
                    choice = "";

                    translatedCursorPosition.y = -38f;
                    soundToPlay = Sound.Backward;
                    playSound = true;
                }
                else
                {
                    PlayerPrefs.SetInt("Course", currentChoice);
                    fader.fadeOutTime = 0f;
                    loadNextLevel = true;
                    soundToPlay = Sound.Forward;
                    playSound = true;

                }

            }
            else
            {
                choice += currentChoice;
                SetText(cups[currentChoice]);
                translatedCursorPosition.y = -38f;

                soundToPlay = Sound.Forward;
                playSound = true;
            }
            currentChoice = 0;
        }

        // Input Back
        if (Input.GetButtonDown("B"))
        {
            if (column != Column.Cups)
            {
                if (column == Column.Confirm)
                    choice = "";
                else if (choice.Length > 0)
                    choice = choice.Remove(choice.Length - 1);

                currentChoice = 0;
                translatedCursorPosition.y = -38f;
            }

            soundToPlay = Sound.Backward;
            playSound = true;
        }

        cursor.anchoredPosition = translatedCursorPosition;
    }

    void SetColour()
    {

        for (int i = 0; i < columnLeft.Length; i++)
        {
            if (column == Column.Cups)
                columnLeft[i].color = Color.white;
            else
                columnLeft[i].color = Color.red;
        }
        for (int i = 0; i < columnRight.Length; i++)
        {
            if (column == Column.Cups)
                columnRight[i].color = Color.red;
            else
                columnRight[i].color = Color.white;
        }

        if (choice.Length == 1)
            columnLeft[int.Parse(choice)].color = new Vector4(1f, Mathf.Sin(time * frequency) + 1 / 2f, 1f, 1f);

    }

    void SetText(List<string> trackList)
    {

        for (int i = 0; i < trackList.Count; i++)
            columnRight[i].text = trackList[i];
    }
}
