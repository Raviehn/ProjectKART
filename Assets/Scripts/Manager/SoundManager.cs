using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public AudioSource titleIntro;
    public AudioSource titleLoop;
    public AudioSource chooseDriver;
    public AudioSource marioCircuit;
    public AudioSource raceStart;
    public AudioSource qualiStart;
    public AudioSource goal;
    public AudioSource newRecord;

    bool startSoundDone = false;

    float fadeSpeed;
    float fadeOutTime;
    float time;
    float volume;

    GameManager manager;
    RaceManager rm;

    Fader fader;

    void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
            DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        volume = titleLoop.volume;
        chooseDriver.volume = titleLoop.volume;

        if (SceneManager.GetActiveScene().buildIndex < 4)
            manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        else
            rm = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();
        
        fader = GameObject.FindGameObjectWithTag("FadeManager").GetComponentInChildren<Fader>();
        fadeSpeed = fader.fadeSpeed;

        if (SceneManager.GetActiveScene().buildIndex == 1)
            titleIntro.Play();
    }

    void Update()
    {
        fadeOutTime = fader.fadeOutTime;

        // no fade while menu is active
        if (fader.noFade)
            time = 0f;

        if (SceneManager.GetActiveScene().buildIndex == 1 && !titleIntro.isPlaying && !titleLoop.isPlaying)
            titleLoop.Play();
        else if ((SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 3) && !chooseDriver.isPlaying)
        {
            titleLoop.Stop();
            chooseDriver.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            if (!raceStart.isPlaying && !startSoundDone)
            {
                chooseDriver.Stop();

                if (rm.isQualification)
                    qualiStart.Play();
                else
                    raceStart.Play();
                startSoundDone = true;
            }

            if (rm.raceStart && !rm._playerFinished && !marioCircuit.isPlaying)
                marioCircuit.Play();
        }

        if (time >= fadeOutTime && SceneManager.GetActiveScene().buildIndex == 1)
        {
            titleIntro.volume = Mathf.Lerp(volume, 0, fadeSpeed * (time - fadeOutTime));
            titleLoop.volume = titleIntro.volume;
            chooseDriver.volume = titleIntro.volume;
        }
        
        time += Time.deltaTime;
    }

    public void PlayGoalSound()
    {
        goal.Play();
        newRecord.Play();
        marioCircuit.Stop();
    }
}
