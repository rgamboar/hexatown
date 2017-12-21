using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour {

    public GameObject tutorial;
    public bool tutorialVisibility;
    public BuildingInterface buildingInterface;
    public Text turnScore;

    public GameObject rightInterface;
    public GameObject[] tutorialPages;

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            int turn = PlayerPrefs.GetInt("Turn");
            turnScore.text= "You lasted for "+ turn + " turns\n Better luck next time";
        }
    }

    public void startGame()
    {
        SceneManager.LoadScene(1);
    }
    public void closeGame()
    {
        Application.Quit();
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ShowTutorial(int page = 0)
    {
        if (page == 0)
        {
            rightInterface.SetActive(false);
        }
        else if (page == 1)
        {
            rightInterface.SetActive(true);
            tutorialPages[page].SetActive(false);
            tutorialPages[page - 1].SetActive(true);
        }
        else if (page == 2)
        {
            rightInterface.SetActive(true);
            tutorialPages[page - 2].SetActive(false);
            tutorialPages[page - 1].SetActive(true);
        }
    }

    public void ToggleTutorial()
    {
        tutorialVisibility = !tutorialVisibility;
        if (tutorialVisibility) ShowTutorial(1);
        else ShowTutorial(0);
    }
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            buildingInterface.CloseLeftInterface();
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
