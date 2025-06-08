using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _tutorWindow;
    [SerializeField] private VideoPlayer _tutorialVideo;
    [SerializeField] private TextMeshProUGUI _tutorialText;
    [SerializeField] private TextMeshProUGUI _tutorialCount;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _backButton;

    [Header("Map Guide")]
    [SerializeField] private List<VideoClip> videoClipsMap;
    [SerializeField, TextArea(3, 10)] private List<string> textMap;

    [Header("Battle Guide")]
    [SerializeField] private List<VideoClip> videoClipsBattle;
    [SerializeField, TextArea(3, 10)] private List<string> textBattle;

    [Header("Team Guide")]
    [SerializeField] private List<VideoClip> videoClipsTeam;
    [SerializeField, TextArea(3, 10)] private List<string> textTeam;

    private List<VideoClip> currentVideoClips;
    private List<string> currentTexts;
    private int currentStep = 0;
    private TutorialType currentTutorialType;
    public bool isActive = false;
    private enum TutorialType
    {
        Map,
        Battle,
        Team
    }

    private void Start()
    {
        _tutorWindow.SetActive(false);
        _closeButton.onClick.AddListener(CloseTutorial);
        MenuMNG.Instance.AddButtonListener(_nextButton,()=> NextStep());
        MenuMNG.Instance.AddButtonListener(_backButton,()=> BackStep());
    }


    public void ShowTutorial()
    {
        if (!IsTutorialNeeded())
            return;

        if (!DetermineCurrentTutorialSet())
            return;
        isActive = true;
        currentStep = 0;
        MenuMNG.Instance.CallFreezePanel(true);
        ShowCurrentStep();
        _tutorWindow.SetActive(true);
    }

    private bool IsTutorialNeeded()
    {
        return GameDataMNG.Instance.PlayerData.IsTutorialNeeded;
    }

    private bool DetermineCurrentTutorialSet()
    {
        var windowController = GlobalWindowController.Instance;
        var playerData = GameDataMNG.Instance.PlayerData;

        if (windowController.IsBattleActive() && playerData.IsTutorialBattle)
        {
            currentVideoClips = videoClipsBattle;
            currentTexts = textBattle;
            currentTutorialType = TutorialType.Battle;
            return true;
        }
        else if (windowController.IsTeamActive() && playerData.IsTutorialTeam)
        {
            currentVideoClips = videoClipsTeam;
            currentTexts = textTeam;
            currentTutorialType = TutorialType.Team;
            return true;
        }
        else if ((windowController.IsGlobalCanvasActive() || windowController.IsMenuActive()) && playerData.IsTutorialMap)
        {
            currentVideoClips = videoClipsMap;
            currentTexts = textMap;
            currentTutorialType = TutorialType.Map;
            return true;
        }

        // ≈сли ни один туториал не нужен дл€ текущего окна
        return false;
    }

    private void ShowCurrentStep()
    {
        if (currentVideoClips == null || currentTexts == null ||
            currentVideoClips.Count == 0 || currentTexts.Count == 0)
        {
            CloseTutorial();
            return;
        }

        int videoIndex = currentStep % currentVideoClips.Count;
        int textIndex = currentStep % currentTexts.Count;

        _tutorialVideo.clip = currentVideoClips[videoIndex];
        _tutorialVideo.Play();
        _tutorialText.text = currentTexts[textIndex];
        _tutorialCount.text = $"{currentStep+1}/{currentVideoClips.Count}";
        _nextButton.gameObject.SetActive(currentStep < Mathf.Min(currentVideoClips.Count, currentTexts.Count) - 1);
        _backButton.gameObject.SetActive(currentStep > 0);
    }

    private void NextStep()
    {
        currentStep++;
        ShowCurrentStep();
    }
    private void BackStep()
    {
        currentStep--;
        ShowCurrentStep();
    }

    private void CloseTutorial()
    {
        isActive = false;
        MenuMNG.Instance.CallFreezePanel(false);
        _tutorWindow.SetActive(false);

        MarkTutorialAsCompleted();

        GameDataMNG.Instance.SaveGame();
    }

    private void MarkTutorialAsCompleted()
    {
        var playerData = GameDataMNG.Instance.PlayerData;

        switch (currentTutorialType)
        {
            case TutorialType.Map:
                playerData.IsTutorialMap = false;
                break;
            case TutorialType.Battle:
                playerData.IsTutorialBattle = false;
                break;
            case TutorialType.Team:
                playerData.IsTutorialTeam = false;
                break;
        }

        // ѕровер€ем, все ли туториалы пройдены
        if (!playerData.IsTutorialMap && !playerData.IsTutorialBattle && !playerData.IsTutorialTeam)
        {
            playerData.IsTutorialNeeded = false;
        }
    }
}