using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class importButton : MonoBehaviour
{
    string path;
    public AudioClip clip;
    public AudioSource source;
    
    public void OpenExplorer()
    {
        //path = EditorUtility.OpenFilePanel("Import Song", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "mp3,wav,aac,ogg,wma,flac,alac,aif,mid");
        //GameManager.instance.Song = path;
        //SceneManager.LoadScene("MainScene");
        //GetAudio();
    }

    void GetAudio()
    {
        if (path != null)
        {
            UpdateAudio();
        }
    }

    void UpdateAudio()
    {
        var www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/result.wav");
        clip = Resources.Load<AudioClip>(path);
    }
}
