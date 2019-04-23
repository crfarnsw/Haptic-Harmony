using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void ScoreBoard()
    {
        SceneManager.LoadScene("LeaderBoard");
    }
    
    public void Profile()
    {
        SceneManager.LoadScene("Profile");
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}

