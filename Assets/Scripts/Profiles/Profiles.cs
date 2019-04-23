using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Profiles : MonoBehaviour
{
    public float difficulty = 0f;

    private void Start()
    {
        
    }

    public void SetDifficulty()
    {
        GameManager.instance.difficulty = this.difficulty;
        SceneManager.LoadScene("MainScene");
    }
}
