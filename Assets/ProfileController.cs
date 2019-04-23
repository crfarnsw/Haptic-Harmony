using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    private string playerName;
    public InputField txtPlayerName;
    public TMP_Dropdown ddlPlayerProfiles;

    private HapticHarmonyDB db;
    // Start is called before the first frame update
    void Start()
    {
        db = GameManager.instance.db;
        ddlPlayerProfiles = GameObject.Find("ddlPlayerProfiles").GetComponent<TMP_Dropdown>();
        txtPlayerName = GameObject.Find("txtPlayerName").GetComponent<InputField>();
        ddlFill();
    }

    void ddlFill()
    {
        ddlPlayerProfiles.ClearOptions();
        ddlPlayerProfiles.AddOptions(new List<string>(db.GetPlayers().Select(p => p.Name)));
    }

    public void SignIn()
    {
        var pName = ddlPlayerProfiles.options[ddlPlayerProfiles.value].text;
        var player = db.GetPlayers().First(p => p.Name == pName);
        GameManager.instance.playerId = player.ID;
        LoadNextScene();
    }

    public void SubmitProfile()
    {
        if (!string.IsNullOrWhiteSpace(txtPlayerName.textComponent.text))
        {
            playerName = txtPlayerName.textComponent.text;
            db.InsertPlayer(playerName);

            var player = db.GetPlayerByName(playerName);
            GameManager.instance.playerId = player.ID;
            LoadNextScene();
            print("success");
        }
        else
        {
            print("error");
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("Menu");
    }
}
