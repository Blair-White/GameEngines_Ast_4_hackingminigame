using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public AudioSource Aud;
    public AudioClip[] sfx;
    private int sequenceProgress, sequenceStep;
    private int sequenceLengthStart, sequenceLength;
    private int difficulty;
    private float countFlashButton, countDelaySequence, count;
    private float maxCountFlashButton, maxCountDelaySequence, maxCount;
    private bool flashing;
    private bool win; 

    public GameObject[] buttons; //red blue green yellow;
    public GameObject[] checks;
    public GameObject lock1, lock2, lock3, lock4;
    public GameObject MainCanvas, GameCanvas, WinLoseCanvas, WinText, LoseText;
    public int[] randomSequence;
    private int inputProgress;
    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(this.gameObject);            
    }
    enum MainStates
    {
        EnterMainMenu, MainMenu, EnterGameplay, GamePlay, EnterWin, Win, EnterLose, Lose
    }
    enum GameStates
    {
        EnterSequence, Sequence, EnterInput, Input, EnterChecking, Checking, EnterResults, Results
    }
    MainStates MainState;
    GameStates GameState;

    // Start is called before the first frame update
    void Start()
    {
        MainState = MainStates.EnterMainMenu;
    }

    // Update is called once per frame
    void Update()
    {

        switch (MainState)
        {
            case MainStates.EnterMainMenu:
                MainCanvas.SetActive(true);
                GameCanvas.SetActive(false);
                WinLoseCanvas.SetActive(false);
                GameState = GameStates.EnterSequence;
                sequenceStep = 0;
                sequenceProgress = 0;
                break;
            case MainStates.MainMenu:

                break;
            case MainStates.EnterGameplay:

                MainCanvas.SetActive(false);
                GameCanvas.SetActive(true);
                WinLoseCanvas.SetActive(false);

                switch (difficulty)
                {
                    case 0:
                        maxCountDelaySequence = 1.0f;
                        maxCountFlashButton = 1.0f;
                        maxCount = 1.0f;
                        sequenceLengthStart = 2;
                        sequenceLength = 5;
                        break;
                    case 1:
                        maxCountDelaySequence = 0.750f;
                        maxCountFlashButton = 0.750f;
                        maxCount = 1.0f;
                        sequenceLengthStart = 3;
                        sequenceLength = 6;
                        break;
                    case 2:
                        maxCountDelaySequence = 0.50f;
                        maxCountFlashButton = 0.50f;
                        maxCount = 0.50f;
                        sequenceLengthStart = 4;
                        sequenceLength = 7;
                        break;
                    default:
                        break;
                }

                for(int i = 0; i < randomSequence.Length; i++)
                {
                    randomSequence[i] = Random.Range(0, 3);
                }

                sequenceProgress = 0;
                sequenceStep = 0;
                inputProgress = 0;

                MainState = MainStates.GamePlay;
                break;

            case MainStates.GamePlay:
                switch (GameState)
                {
                    case GameStates.EnterSequence:
                        if (sequenceProgress > 3)
                        {
                            GameState = GameStates.EnterResults;
                            sequenceStep = 0;
                            sequenceProgress = 0;
                        }
                        sequenceStep = 0;
                        GameState = GameStates.Sequence;
                        break;

                    case GameStates.Sequence:
                        
                        if(sequenceStep >= sequenceLengthStart + sequenceProgress)
                        {
                            GameState = GameStates.EnterInput;
                            sequenceStep = 0;
                        }

                        if (!flashing)
                        {
                            countDelaySequence += Time.deltaTime; ;
                            if (countDelaySequence > maxCountDelaySequence)
                            {
                                flashing = true;
                                buttons[randomSequence[sequenceStep]].SetActive(true);
                                PlaySoundOneShot(sfx[randomSequence[sequenceStep]]);
                                countDelaySequence = 0;
                            }
                        }

                        if (flashing)
                        {
                            countFlashButton += Time.deltaTime;
                            if (countFlashButton > maxCountFlashButton)
                            {
                                buttons[randomSequence[sequenceStep]].SetActive(false);
                                countFlashButton = 0;
                                sequenceStep++;
                                flashing = false;

                            }
                        }

                        break;
                    case GameStates.EnterInput:
                        Debug.Log("entered input phase");
                        GameState = GameStates.Input;
                        break;
                    case GameStates.Input:
                        if (inputProgress >= sequenceLengthStart + sequenceProgress)
                            GameState = GameStates.EnterChecking;
                        break;
                    case GameStates.EnterChecking:
                        if(sequenceProgress <= 3) checks[sequenceProgress].SetActive(true);
                        sequenceProgress++;
                        inputProgress = 0;
                       
                        if(sequenceProgress > 3)
                        {
                            count += Time.deltaTime;
                            if(count > 2)
                            {
                                GameState = GameStates.EnterResults;
                                win = true;
                            }    
                        }
                        else
                        {
                            GameState = GameStates.EnterSequence;
                        }
                        break;
                    case GameStates.Checking:
                        break;
                    case GameStates.EnterResults:
                        if (win) MainState = MainStates.EnterWin;
                        else MainState = MainStates.EnterLose;
                                  
                        break;
                    case GameStates.Results:
                        break;
                    default:
                        break;
                }
                break;
            case MainStates.EnterWin:
                MainCanvas.SetActive(false);
                GameCanvas.SetActive(false);
                WinLoseCanvas.SetActive(true);
                WinText.SetActive(true);
                LoseText.SetActive(false);
                break;
            case MainStates.Win:
                break;
            case MainStates.EnterLose:
                MainCanvas.SetActive(false);
                GameCanvas.SetActive(false);
                WinLoseCanvas.SetActive(true);
                WinText.SetActive(false);
                LoseText.SetActive(true);
                break;
            case MainStates.Lose:
                break;
            default:
                break;
        }
    }


    public void PushedButton(int color) //red green blue yellow
    {

        if (GameState != GameStates.Input) return;

        switch (color)
        {
            case 0:
                PlaySoundOneShot(sfx[color]);
                if (CheckPush(0)) inputProgress++;
                else {FailedPush();}
                break;
            case 1:
                PlaySoundOneShot(sfx[color]);
                if (CheckPush(1)) inputProgress++;
                else { FailedPush(); }
                break;
            case 2:
                PlaySoundOneShot(sfx[color]);
                if (CheckPush(2)) inputProgress++;
                else { FailedPush(); }
                break;
            case 3:
                PlaySoundOneShot(sfx[color]);
                if (CheckPush(3)) inputProgress++;
                else { FailedPush(); }
                break;

            default:
                break;
        }
    }


    public void EasyPushed() { difficulty = 0; MainState = MainStates.EnterGameplay; }
    public void NormalPushed() { difficulty = 1; MainState = MainStates.EnterGameplay; }
    public void HardPushed() { difficulty = 2; MainState = MainStates.EnterGameplay; }
    public void BackToMain() { MainState = MainStates.EnterMainMenu; }

    private bool CheckPush(int inp)
    {
        if (randomSequence[inputProgress] == inp)
        {
            Debug.Log("true input");
            Debug.Log(inp);
             return true;
        }
        else
        {
            Debug.Log("false input");
            Debug.Log(inp);
             return false;
        }
    }
    private void FailedPush()
    {
        GameState = GameStates.EnterResults;
        win = false;
    }
    public void PlaySoundOneShot(AudioClip clip)
    {
        Aud.PlayOneShot(clip);
    }
}
