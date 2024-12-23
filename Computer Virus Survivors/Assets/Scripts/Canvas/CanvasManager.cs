using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public interface IState
{
    void OnEnter();
    void OnExit();
}

public class CanvasManager : Singleton<CanvasManager>, IPlayerStatObserver
{

    [SerializeField] private PlayerStatEventCaller playerStatEventCaller;

    [SerializeField] private ItemSelectCanvasManager itemSelectCanvas;
    [SerializeField] private PauseCanvas pauseCanvas;
    [SerializeField] private PlayerGUI playerGUI;

    private IState playingState;
    private IState itemSelectState;
    private IState pausedState;
    private IState gameOverState;
    private IState currentState;
    private bool isItemSelecting;

    public override void Initialize()
    {
        itemSelectCanvas.Initialize();
        pauseCanvas.Initialize();
        playerGUI.Initialize();

        playerStatEventCaller.StatChangedHandler += OnStatChanged;
        itemSelectCanvas.SelectionHandler += (selectableBehaviour) =>
        {
            StateMachine(Signal.OnItemSelectDone);
        };
        pauseCanvas.ResumeHandler += () =>
        {
            StateMachine(Signal.OnResumeClicked);
        };

        playingState = new PlayingState();
        itemSelectState = itemSelectCanvas;
        pausedState = pauseCanvas;
        // TODO : gameOverState 초기화
        currentState = playingState;
        isItemSelecting = false;
    }




    private enum Signal
    {
        LevelUp,
        GameOver,
        OnPauseClicked,
        OnResumeClicked,
        OnItemSelectDone
    }

    public void OnPauseBtnClicked()
    {
        StateMachine(Signal.OnPauseClicked);
    }

    public void OnStatChanged(object sender, StatChangedEventArgs args)
    {
        if (args.StatName == nameof(PlayerStat.PlayerLevel))
        {
            if ((int) args.NewValue > 1)
            {
                StateMachine(Signal.LevelUp);
            }

        }
    }

    private void StateMachine(Signal signal)
    {
        void SetState(IState newState)
        {
            currentState.OnExit();
            Debug.Log("State Change : " + currentState + " -> " + newState);
            currentState = newState;
            currentState.OnEnter();
        }

        if (currentState == playingState)
        {
            switch (signal)
            {
                case Signal.LevelUp:
                    itemSelectCanvas.gameObject.SetActive(true);
                    isItemSelecting = true;
                    SetState(itemSelectState);
                    break;
                // case Signal.GameOver:
                //     SetState(gameOverState);
                //     break;
                case Signal.OnPauseClicked:
                    pauseCanvas.gameObject.SetActive(true);
                    SetState(pausedState);
                    break;
                    // default:
                    //     throw new Exception("Unresolved Signal : " + nameof(signal));
            }
        }
        else if (currentState == pausedState)
        {
            switch (signal)
            {
                case Signal.OnResumeClicked:
                    pauseCanvas.gameObject.SetActive(false);
                    if (isItemSelecting)
                    {
                        SetState(itemSelectState);
                    }
                    else
                    {
                        SetState(playingState);
                    }
                    break;
                    // default:
                    //     throw new Exception("Unresolved Signal : " + nameof(signal));
            }
        }
        else if (currentState == itemSelectState)
        {
            switch (signal)
            {
                case Signal.OnItemSelectDone:
                    itemSelectCanvas.gameObject.SetActive(false);
                    isItemSelecting = false;
                    SetState(playingState);
                    break;
                case Signal.OnPauseClicked:
                    pauseCanvas.gameObject.SetActive(true);
                    SetState(pausedState);
                    break;
                    // default:
                    //     throw new Exception("Unresolved Signal : " + nameof(signal));
            }
        }
        // else if (currentState == gameOverState)
        // {
        //     // TODO : GameOverState에서의 Signal 처리
        // }
    }

    private class PlayingState : IState
    {
        public void OnEnter()
        {
            Time.timeScale = 1;
        }

        public void OnExit()
        {
            Time.timeScale = 0;
        }

    }
}


