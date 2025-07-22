using UnityEngine;
using YG;

public class UIStateMachine : MonoBehaviour
{
    public enum UIState { Tutorial, Settings, MainMenu, GamveOver, ActiveGameUi, StartMenu, LeaderBoard, MobileControl, PlayerInfo }

    [Header("Canvas References")]
    [SerializeField] private Canvas _startMenuCanvas;
    [SerializeField] private Canvas _mainMenuCanvas;
    [SerializeField] private Canvas _settingsCanvas;
    [SerializeField] private Canvas _tutorialCanvas;
    [SerializeField] private Canvas _gamveOverCanvas;
    [SerializeField] private Canvas _activeGameUiCanvas;
    [SerializeField] private Canvas _leaderBoard;
    [SerializeField] private Canvas _mobileControl;
    [SerializeField] private Canvas _playerInfo;

    [SerializeField] private AudioSource _audioSource;

    private UIState _currentState = UIState.Tutorial;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SwitchState(UIState.StartMenu);
    }

    public void ShowTutorial() => SwitchState(UIState.Tutorial);
    public void ShowSettings() => SwitchState(UIState.Settings);
    public void ShowMainMenu() => SwitchState(UIState.MainMenu);
    public void ShowGameOver() => SwitchState(UIState.GamveOver);
    public void ShowActiveGameUi() => SwitchState(UIState.ActiveGameUi);
    public void ShowStartMenu() => SwitchState(UIState.StartMenu);
    public void ShowLeaderBoard() => SwitchState(UIState.LeaderBoard);

    private void SwitchState(UIState newState)
    {
        if (_currentState == newState)
            return;

        if (_audioSource != null)
            _audioSource.Play();

        if (YG2.player.auth && newState != UIState.ActiveGameUi)
            ShowPlayerInfo();
        else
            DisablePlayerInfo();

        if (newState == UIState.LeaderBoard && YG2.player.auth)
        {
            YG2.OpenAuthDialog();
            SetCanvasActive(UIState.StartMenu, false);
        }
        else
        {
            SetCanvasActive(_currentState, false);
        }

        ChangeGameState(newState);
        CheckDevice(newState);

        _currentState = newState;

        SetCanvasActive(newState, true);
    }

    private void SetCanvasActive(UIState state, bool isActive)
    {
        switch (state)
        {
            case UIState.MainMenu:
                _mainMenuCanvas.gameObject.SetActive(isActive);
                break;
            case UIState.Settings:
                _settingsCanvas.gameObject.SetActive(isActive);
                break;
            case UIState.Tutorial:
                _tutorialCanvas.gameObject.SetActive(isActive);
                break;
            case UIState.GamveOver:
                _gamveOverCanvas.gameObject.SetActive(isActive);
                break;
            case UIState.ActiveGameUi:
                _activeGameUiCanvas.gameObject.SetActive(isActive);
                break;
            case UIState.StartMenu:
                _startMenuCanvas.gameObject.SetActive(isActive);
                break;
            case UIState.LeaderBoard:
                _leaderBoard.gameObject.SetActive(isActive);
                break;
        }
    }

    private static void ChangeGameState(UIState newState)
    {
        if (newState == UIState.ActiveGameUi)
            StateGame.ResumeGame();
        else
            StateGame.PauseGame();
    }

    private void CheckDevice(UIState newState)
    {
        if (newState == UIState.ActiveGameUi && YG2.envir.isMobile)
        {
            EnableMobileControl();
        }
        else
        {
            DisableMobileControl();
        }
    }

    private void DisableMobileControl()
    {
        _mobileControl.gameObject.SetActive(false);
    }

    private void EnableMobileControl()
    {
        _mobileControl.gameObject.SetActive(true);
    }

    private void ShowPlayerInfo()
    {
        _playerInfo.gameObject.SetActive(true);
    }

    private void DisablePlayerInfo()
    {
        _playerInfo.gameObject.SetActive(false);
    }
}
