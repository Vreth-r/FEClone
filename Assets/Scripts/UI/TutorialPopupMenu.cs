using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialPopupMenu : MonoBehaviour, IGameMenu
{
    [Header("UI")]
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;
    public Image illustrationImage;
    public Button nextButton;
    public Button closeButton;

    private Queue<TutorialStep> steps = new();
    private TutorialStep currentStep;

    public bool IsOpen { get; private set; }
    public MenuType MenuID => MenuType.TutorialPopup;
    public bool escapable => currentStep != null && currentStep.escapable;

    private void Awake()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        nextButton.onClick.AddListener(NextStep);
        closeButton.onClick.AddListener(Close);
    }

    // public
    public void Open(List<TutorialStep> tutorialSteps)
    {
        steps.Clear();
        foreach (var step in tutorialSteps)
        {
            steps.Enqueue(step);
        }

        Open();
        ShowNext();
    }

    public void Open()
    {
        IsOpen = true;
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        IsOpen = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        CampInputBlocker.SetBlocked(false);
        UIManager.Instance.CloseTopMenu();
    }

    // ---- Internal ---

    private void ShowNext()
    {
        if (steps.Count == 0)
        {
            Close();
            return;
        }

        currentStep = steps.Dequeue();

        titleText.text = currentStep.title;
        bodyText.text = currentStep.bodyText;

        illustrationImage.gameObject.SetActive(currentStep.illustration != null);
        if (currentStep.illustration != null)
        {
            illustrationImage.sprite = currentStep.illustration;
        }

        CampInputBlocker.SetBlocked(currentStep.blockInput);
    }

    private void NextStep()
    {
        ShowNext();
    }
}