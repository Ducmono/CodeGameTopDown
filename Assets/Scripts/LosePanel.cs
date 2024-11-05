using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LosePanel : MonoBehaviour
{
    public TextMeshProUGUI score;

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        int scoreI = FindObjectOfType<Killed>().currentKilled * 10;
        score.text = "You get: " + scoreI.ToString() + " Score";
        Time.timeScale = 0;
    }

    public void Hide()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        this.Replay();
    }
    protected void Replay()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ReloadScene();
        }
    }
    private void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
