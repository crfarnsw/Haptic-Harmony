using DSPLib;
using NLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using NAudio.Wave;
using TMPro;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector3 = UnityEngine.Vector3;

public class AudioController : MonoBehaviour
{
    public SpectralFluxAnalyzer FluxAnalyzer;
    public AudioSource source;
    private TextMeshProUGUI pointsText;
    private TextMeshProUGUI audioDataText;

    //public Dictionary<int, Color> colors = new Dictionary<int, Color> { { 0, Color.green }, { 1, Color.red }, { 2, Color.yellow }, { 3, Color.blue } };
    private Dictionary<int, GameObject> cubes;
    private Dictionary<int, GameObject> rumbled;
    public float[] multiChannelSamples;
    private float[] realTimeSpectrum;

    void Start()
    {
        cubes = new Dictionary<int, GameObject>();
        rumbled = new Dictionary<int, GameObject>();
        var textElements = FindObjectsOfType<TextMeshProUGUI>();
        //audioDataText = textElements.Where(t => t.name == "SongPosition").First();
        pointsText = textElements.Where(t => t.name == "Points").First();

        SetSongData();

        multiChannelSamples = new float[source.clip.samples * source.clip.channels];
        source.clip.GetData(multiChannelSamples, 0);
        print("GetData done");

        FluxAnalyzer = new SpectralFluxAnalyzer(source.clip.channels, source.clip.samples, source.clip.frequency, source.clip.length, multiChannelSamples);
        Thread bgThread = new Thread(FluxAnalyzer.GetFullSpectrumThreaded);
        Debug.Log("Starting Background Thread");
        bgThread.Start();
    }

    void Update()
    {
        if (FluxAnalyzer.SpectralFluxSamples.Count > 0)
        {
            var index = GetIndexFromTime(source.time) / 1024;
            SetCameraPosition(index);
            DrawSpectrumLines(index);
            SetGuiText();
        }
    }

    void SetSongData()
    {
        string dest = GameManager.instance.song;

        if (string.IsNullOrEmpty(dest))
        {
#if UNITY_EDITOR
            dest = EditorUtility.OpenFilePanel("Import Song", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "mp3,wav,aac,ogg,wma,flac,alac,aif,mid");
#endif
        }

        FileInfo info = new FileInfo(dest);
        string newDest = Path.Combine(Application.streamingAssetsPath, info.Name);
        File.Copy(dest, newDest);

        if (File.Exists(Path.Combine(Application.streamingAssetsPath, info.Name)))
        {
            var www = new WWW("file://" + Path.Combine(Application.streamingAssetsPath, info.Name));
            source.clip = NAudioPlayer.LoadFromMp3(www.bytes); ;
            File.Delete(newDest);
        }

        GC.Collect();
    }

    void SetCameraPosition(int index)
    {
        var r = GameManager.instance.cam.velocity;
        var cameraPosition =
            Vector3.SmoothDamp(GameManager.instance.cam.transform.position,
                                new Vector3(0, y: index + 9.5f, GameManager.instance.cam.transform.position.z), ref r, source.pitch * Time.deltaTime);

        GameManager.instance.cam.transform.position = new Vector3(0f, index, GameManager.instance.cam.transform.position.z);
    }

    void SetGuiText()
    {
        //audioDataText.SetText($"\n Time:      {source.time}");
        pointsText.SetText($"Score: {GameManager.instance.points}");
    }

    void DrawSpectrumLines(int index)
    {
        float height = Screen.height;

        int windowStart = 0;
        int windowEnd = 0;

        if (index > 0)
        {
            windowStart = Mathf.Max(1, index - Mathf.FloorToInt(height) / 2);
            windowEnd = Mathf.Min(index + Mathf.FloorToInt(height) / 2, FluxAnalyzer.SpectralFluxSamples.Count - 1);
        }
        else
        {
            windowStart = Mathf.Max(1, FluxAnalyzer.SpectralFluxSamples.Count - Mathf.FloorToInt(height) - 1);
            windowEnd = Mathf.Min(windowStart + Mathf.FloorToInt(height), FluxAnalyzer.SpectralFluxSamples.Count);
        }

        for (int i = windowStart; i < windowEnd; i++)
        {
            Vector3 start = new Vector3(0, i, 0);
            Vector3 end = new Vector3(FluxAnalyzer.SpectralFluxSamples[i].spectralFlux, i, 0);
            Debug.DrawLine(start, end);

            if (FluxAnalyzer.SpectralFluxSamples[i].isPeak)
            {
                if (!cubes.ContainsKey(i))
                {
                    int rand = GameManager.instance.difficulty < 3f
                        ? UnityEngine.Random.Range(0, 3)
                        : UnityEngine.Random.Range(0, 2);

                    //int rand = UnityEngine.Random.Range(0, 4);
                    switch (rand)
                    {
                        case 0:
                            var greenNote = Instantiate(Resources.Load("GreenNote"), transform) as GameObject;
                            greenNote.transform.position = new Vector3(0, i, 1.5f);
                            (greenNote.GetComponent<CubeScript>()).keyToPress = 1;
                            cubes.Add(i, greenNote);
                            break;
                        case 1:
                            var redNote = Instantiate(Resources.Load("RedNote"), transform) as GameObject;
                            redNote.transform.position = new Vector3(0, i, 1.5f);
                            (redNote.GetComponent<CubeScript>()).keyToPress = 2;
                            cubes.Add(i, redNote);
                            break;
                        case 2:
                            if (GameManager.instance.difficulty < 3f)
                            {
                                var yellowNote = Instantiate(Resources.Load("YellowNote"), transform) as GameObject;
                                yellowNote.transform.position = new Vector3(0, i, 1.5f);
                                (yellowNote.GetComponent<CubeScript>()).keyToPress = 3;
                                cubes.Add(i, yellowNote);
                            }
                            break;
                        case 3:
                            if (GameManager.instance.difficulty < 3f)
                            {
                                var blueNote = Instantiate(Resources.Load("BlueNote"), transform) as GameObject;
                                blueNote.transform.position = new Vector3(0, i, 1.5f);
                                (blueNote.GetComponent<CubeScript>()).keyToPress = 4;
                                cubes.Add(i, blueNote);
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (!rumbled.ContainsKey(i))
                    {
                        if (FluxAnalyzer.SpectralFluxSamples[i].time - source.time <= 0.5f && FluxAnalyzer.SpectralFluxSamples[i].time - source.time > 0f)
                        {
                            if (cubes[i] != null)
                            {
                                GameManager.instance.arduinoListener.SendVibration(cubes[i].GetComponent<CubeScript>().keyToPress);
                                rumbled.Add(i, cubes[i]);
                            }
                            //if (cubes.TryGetValue(i, out GameObject go))
                            //{
                            //    //go.GetComponent<CubeScript>().SendVibration();
                            //}
                        }
                    }
                }
            }
        }
    }

    public int GetIndexFromTime(float curTime)
    {
        float lengthPerSample = source.clip.length / (float)source.clip.samples;

        return Mathf.FloorToInt(curTime / lengthPerSample);
    }

    public float GetTimeFromIndex(int index)
    {
        return ((1f / (float)this.source.clip.frequency) * index);
    }

    private void VisualizerTest()
    {
        float[] spectrum = new float[256];
        source.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }
}
