using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventHandler : MonoBehaviour {

    public GameObject tutorial;
    public bool tutorialVisibility;

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
    public void closeGame()
    {
        Application.Quit();
    }
    public void ToggleTutorial()
    {
        tutorialVisibility = !tutorialVisibility;
        tutorial.SetActive(tutorialVisibility);
    }
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (tutorialVisibility)
            {
                tutorialVisibility = !tutorialVisibility;
                tutorial.SetActive(tutorialVisibility);
            }
            else
            {
                closeGame();
            }
        }
    }
}
