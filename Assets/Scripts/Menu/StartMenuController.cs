using SimpleFileBrowser;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class StartMenuController : MonoBehaviour
{
    private readonly string musicFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
    
    private void LoadNextScene()
    {
        SceneManager.LoadScene("Difficulty");
    }

    public void QuickPlay()
    {
        SetRandomSong();
        LoadNextScene();
    }

    public void Play()
    {
        OpenFileExplorer();
    }

    public void SetRandomSong()
    {
        var mpegDirectories = new DirectoryInfo(musicFolderPath).GetDirectories();
        List<FileInfo> files = new List<FileInfo>();

        foreach (var dir in mpegDirectories)
        {
            foreach (var file in dir.GetFiles().Where(f => f.Extension.Contains("mp3")))
            {
                files.Add(file);
            }
        }

        if (files.Any())
        {
            var rand = new Random();

            GameManager.instance.song = files.ElementAt(rand.Next(0, files.Count)).FullName;
        }
    }

    private void OpenFileExplorer()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Audio Files", ".mp3", ".wav"));
        FileBrowser.SetDefaultFilter(".mp3");
        FileBrowser.AddQuickLink("Music", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    private IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(false, musicFolderPath, "Load File", "Load");

        if (FileBrowser.Success)
        {
            GameManager.instance.song = FileBrowser.Result;
            LoadNextScene();
        }
    }
}
