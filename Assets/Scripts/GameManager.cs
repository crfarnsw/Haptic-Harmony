using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Manages data for persistence between levels.</summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ArduinoListener arduinoListener;
    public AudioSource audioSource;
    public Camera cam;
    public bool isPlaying;
    public float difficulty;
    public int totalNotes;
    public int playerId;
    public string song { get; set; }
    public float points = 0f;
    private bool cameraFound = false;
    public int numMissed = 0;
    public int numHit = 0;
    public bool songEnded = false;
    public int currValue = 0;
    public HapticHarmonyDB db;
    public Stat[] stats = new Stat[4];
    public Stat yellowStat;
    public Stat greenStat;
    public Stat redStat;
    public Stat blueStat;

    void Awake()
    {
        cam = (Camera)FindObjectOfType(typeof(Camera));
        arduinoListener = GetComponent<ArduinoListener>();
        yellowStat = new Stat();
        greenStat = new Stat();
        redStat = new Stat();
        blueStat = new Stat();

        stats[0] = greenStat;
        stats[1] = yellowStat;
        stats[2] = redStat;
        stats[3] = blueStat;

        if (instance == null)
        {
            instance = this;
            db = new HapticHarmonyDB("leaderboard.db");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainScene")
        {
            if (!cameraFound)
            {
                var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                cam = gameObjects[0].GetComponent<Camera>();
                audioSource = gameObjects.First(c => c.name == "Audio Source").GetComponent<AudioSource>();
                cameraFound = true;
            }

            if (!isPlaying)
            {
                if (arduinoListener.port.IsOpen)
                {
                    if (arduinoListener.ReceiveInput() > 0)
                    {
                        isPlaying = true;
                        audioSource.Play();
                        StartCoroutine(WaitAudio());
                    }
                }
                else
                {
                    if (Input.anyKeyDown)
                    {
                        isPlaying = true;
                        audioSource.Play();
                        StartCoroutine(WaitAudio());
                    }
                }
            }

            if (songEnded)
            {
                var gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                var modal = gameObjects.First(c => c.name == "Modal");
                modal.SetActive(true);
            }
        }
    }

    private IEnumerator WaitAudio()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        songEnded = true;
    }

    public void NoteHit(int color)
    {
        stats[color - 1].AddHit();
        points++;
    }

    public void NoteMissed(int color)
    {
        stats[color - 1].AddMissed();
        points--;
    }

    public int TotalNotes()
    {
        return stats.Sum(s => s.GetTotalNotes());
    }

    public int TotalHit()
    {
        return stats.Sum(s => s.GetTotalHit());
    }

    public int TotalMissed()
    {
        return stats.Sum(s => s.GetTotalMissed());
    }

    public void SubmitScore()
    {
        db.InsertScore(playerId, points, song);
    }
}

public class Stat
{
    protected int TotalNotes;
    protected int Hit;
    protected int Missed;

    public Stat()
    {
        Hit = Missed = 0;
    }

    public int GetTotalNotes()
    {
        return TotalNotes;
    }

    public int GetTotalHit()
    {
        return Hit;
    }

    public int GetTotalMissed()
    {
        return Missed;
    }

    public void AddHit()
    {
        Hit++;
        TotalNotes++;
    }

    public void AddMissed()
    {
        Missed++;
        TotalNotes++;
    }
}