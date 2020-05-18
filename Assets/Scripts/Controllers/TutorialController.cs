using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public List<GameObject> popups;
    public GameObject nextButton;
    public GameObject tutorialPanel;
    private int currentPopup;

    public bool TutorialCompleted { get; set; }
    public static TutorialController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void StartTutorial()
    {
        Time.timeScale = 0;
        currentPopup = 0;
        tutorialPanel.SetActive(true);
        popups[currentPopup].SetActive(true);
        nextButton.SetActive(true);
    }

    public void NextSlide()
    {
        popups[currentPopup].SetActive(false);
        currentPopup++;
        if (currentPopup < popups.Count)
            popups[currentPopup].SetActive(true);
        else
        {
            TutorialCompleted = true;
            tutorialPanel.SetActive(false);
            Time.timeScale = 1;
            GameController.Instance.SaveGame();
        }
    }

    public void ReactivateTutorial()
    {
        TutorialCompleted = false;
        GameController.Instance.SaveGame();
    }
}
