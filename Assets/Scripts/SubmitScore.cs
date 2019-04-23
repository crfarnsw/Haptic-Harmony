using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SubmitScore : MonoBehaviour
{
    private void OnEnable()
    {
        GameObject.Find("GreenTotal").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[0].GetTotalNotes().ToString();
        GameObject.Find("GreenHit").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[0].GetTotalHit().ToString();
        GameObject.Find("GreenMissed").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[0].GetTotalMissed().ToString();

        GameObject.Find("RedTotal").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[1].GetTotalNotes().ToString();
        GameObject.Find("RedHit").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[1].GetTotalHit().ToString();
        GameObject.Find("RedMissed").GetComponent<TextMeshProUGUI>().text =GameManager.instance.stats[1].GetTotalMissed().ToString();

        GameObject.Find("YellowTotal").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[2].GetTotalNotes().ToString();
        GameObject.Find("YellowHit").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[2].GetTotalHit().ToString();
        GameObject.Find("YellowMissed").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[2].GetTotalMissed().ToString();

        GameObject.Find("BlueTotal").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[3].GetTotalNotes().ToString();
        GameObject.Find("BlueHit").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[3].GetTotalHit().ToString();
        GameObject.Find("BlueMissed").GetComponent<TextMeshProUGUI>().text = GameManager.instance.stats[3].GetTotalMissed().ToString();

        // Totals
        GameObject.Find("NotesTotal").GetComponent<TextMeshProUGUI>().text = $"{GameManager.instance.TotalNotes()}";
        GameObject.Find("NotesHit").GetComponent<TextMeshProUGUI>().text = $"{GameManager.instance.TotalMissed()}";
        GameObject.Find("NotesMissed").GetComponent<TextMeshProUGUI>().text = $"{GameManager.instance.TotalHit()}";
    }

    public void Submit()
    {
        GameManager.instance.SubmitScore();
        SceneManager.LoadScene("LeaderBoard");
    }
}
