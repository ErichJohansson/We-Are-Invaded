using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    public List<GameObject> popups;
    public GameObject nextButton;
    public GameObject tutorialPanel;
    private int currentPopup;
    private Button btn;

    public bool TutorialCompleted { get; set; }

    public void StartTutorial()
    {
        Time.timeScale = 0;
        currentPopup = 0;
        tutorialPanel.SetActive(true);
        popups[currentPopup].SetActive(true);
        nextButton.SetActive(true);
        btn = nextButton.GetComponent<Button>();
    }

    public void NextSlide(float loadLen)
    {
        popups[currentPopup].SetActive(false);
        btn.interactable = false;
        currentPopup++;
        if (currentPopup < popups.Count)
            UIController.Instance.ActivateLoadEffect(loadLen, currentPopup % 2 == 0, 
                () => { 
                    popups[currentPopup].SetActive(true); 
                    UIController.Instance.loadEffect.gameObject.SetActive(false);
                    btn.interactable = true;
            });
        else
        {
            TutorialCompleted = true;
            tutorialPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ReactivateTutorial()
    {
        TutorialCompleted = false;
        GameController.Instance.SaveGame();
    }
}
