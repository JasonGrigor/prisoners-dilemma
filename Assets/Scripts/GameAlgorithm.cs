using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RockVR.Video;

public class GameAlgorithm : MonoBehaviour {

    #region Public Variables

    #region Files Info

    public GameObject setupCanvas;
    public GameObject CaptureCamera;

    readonly string timeStamp;
    public float startTime;

    public string userID;
    public string pathString;

    public int frames;
    public int videoNumber = 1;
    public bool videoFinished = false;

    #endregion

    #region Game Parameters
    [Header("Game Parameters")]

    public int reward = 2;
    public int tempation = 3;
    public int sucker = -1;
    public int punishment = 0;
    public int numberOfOpp;
    public int roundsPerOpp;
    public int totalRounds;
    public float oppDelay;
    public float newRoundDelay;

    int oppType;
    #endregion

    #region Game Info

    [Header("Game Info")]
    public bool[] playerDecisions = new bool[25];
    public bool[] opponentDecisions = new bool[25];

    //number of rounds
    public int totalRoundCounter = 0;
    public int roundCounter = 0;
    public int gameCounter = 0;

    int[] OpponentOrder = new int[7];

    public int pl_coop_counter;
    public int opp_coop_counter;

    public bool playerChosen = false;
    public bool oppChosen = false;

    //decisions
    public bool player_coop;
    public bool opponent_coop;
    public float totalSliderTime = 0f;

    [Header("Scores")]
    //player scores
    public int player_round_score;
    public int opponent_round_score;
    public int player_total_score = 0;
    public int opponent_total_score = 0;

    #endregion

    #region UI Elements
    [Header("UI Elements")]
    public Button plCooperate;
    public Button plBetray;
    public Button oppCooperate;
    public Button oppBetray;
    public Button exitButton;

    public GameObject stopGame;

    public GameObject scoreMessages;
    public Text Pl_rnd_sc;
    public Text Op_rnd_sc;
    public Text Pl_tot_sc;
    public Text Op_tot_sc;

    public Text round_text;

    public Text pl_coop_counter_text;
    public Text opp_coop_counter_text;

    public Dropdown oppSelect;

    public Slider slider;
    public GameObject sliderFill;

    //public Animation plMoveScore;
    #endregion

    #endregion

    #region Main Methods

    void Start() {

        //stopGame.SetActive(false);
        VideoCaptureCtrl.instance.StartCapture();
        Debug.Log("Video capture " + videoNumber + " started!");
        videoFinished = true;

        GetComponent<LogHandler>().startTime = Time.time;

        setupCanvas.SetActive(false);
        scoreMessages.SetActive(false);

        numberOfOpp = setupCanvas.GetComponent<SetupGame>().numberOfOpp;
        roundsPerOpp = setupCanvas.GetComponent<SetupGame>().roundsPerOpp;
        totalRounds = numberOfOpp * roundsPerOpp;

        oppDelay = setupCanvas.GetComponent<SetupGame>().oppDelay;
        newRoundDelay = setupCanvas.GetComponent<SetupGame>().newRoundDelay;

        Pl_rnd_sc.GetComponent<Animator>().speed = 1f / (0.001f + newRoundDelay);
        Op_rnd_sc.GetComponent<Animator>().speed = 1f / (0.001f + newRoundDelay);


        oppCooperate.GetComponent<Image>().color = Color.grey;
        oppBetray.GetComponent<Image>().color = Color.grey;

        //OppTypeLog(oppSelect.value);
        //System.Random rnd = new System.Random();
        //OpponentOrder = OpponentOrder.OrderBy(x => rnd.Next()).ToArray();
        Debug.Log("Order of the Opponents will be: ");
        for (int i = 0; i < numberOfOpp; i++)
        {
            OpponentOrder[i] = setupCanvas.GetComponent<SetupGame>().OpponentOrder[i];
            Debug.Log((i + 1) + ". " + OppType(OpponentOrder[i]).ToString());
        }

        OppTypeLog(OpponentOrder[gameCounter]);



        Debug.Log("Game Started at " + startTime + Environment.NewLine);

    }

    void Update() {

        if (playerChosen)
        {
            IncreaseSlider();
        }

        if (playerChosen && oppChosen)
        {
            //Debug.Log(Time.time);
            //oppSelect.enabled = false; //disable opponent selection

            if (player_coop && opponent_coop)
            {
                player_round_score = reward;
                opponent_round_score = reward;
                Debug.Log("Player COOPERATED");
                Debug.Log("Opponent COOPERATED" + Environment.NewLine);
            }
            else if (player_coop && !opponent_coop)
            {
                player_round_score = sucker;
                opponent_round_score = tempation;
                Debug.Log("Player COOPERATED");
                Debug.Log("Opponent BETRAYED" + Environment.NewLine);

            }
            else if (!player_coop && opponent_coop)
            {
                player_round_score = tempation;
                opponent_round_score = sucker;
                Debug.Log("Player BETRAYED");
                Debug.Log("Opponent COOPERATED" + Environment.NewLine);
            }
            else if (!player_coop && !opponent_coop)
            {
                player_round_score = punishment;
                opponent_round_score = punishment;
                Debug.Log("Player BETRAYED");
                Debug.Log("Opponent BETRAYED" + Environment.NewLine);
            }

            if (opponent_coop)
            {
                oppCooperate.GetComponent<Image>().color = Color.white;
            }
            else
                oppBetray.GetComponent<Image>().color = Color.white;


            // if game is not over
            if (totalRoundCounter < totalRounds)
            {
                round_text.text = "Έπαιξες " + totalRoundCounter.ToString() + " γύρους";
            }
            else
            {
                if (gameCounter == numberOfOpp)
                {
                    round_text.text = "GAME OVER";
                    DisablePlayerButtons();
                }
            }

            playerChosen = false;
            totalSliderTime = 0;
        }

        if (oppChosen)
        {
            DecreaseSlider();
        }

        if (player_round_score > 0)
        {
            Pl_rnd_sc.text = "+" + player_round_score.ToString();
        } else
            Pl_rnd_sc.text = player_round_score.ToString();

        if (opponent_round_score > 0)
        {
            Op_rnd_sc.text = "+" + opponent_round_score.ToString();
        }
        else
            Op_rnd_sc.text = opponent_round_score.ToString();

        Pl_tot_sc.text = "Δικό σου συνολικό σκορ: " + player_total_score.ToString();
        Op_tot_sc.text = "Συνολικό σκορ Αντιπάλου: " + opponent_total_score.ToString();
        //Debug.Log("Player score:");
        //Debug.Log(player_total_score);
        //Debug.Log("Opponent's score:");
        //Debug.Log(opponent_total_score);

        
        if (frames == 14400 || !videoFinished) //if video exceeds the 4 minutes mark or opponent changes
        {
            NewVideoSesion();
        }
        
        frames++;
    }

    public void ExitApp()
    {
        if (CaptureCamera.GetComponent<VideoCaptureCtrl>().status == VideoCaptureCtrlBase.StatusType.STARTED)
        {
            VideoCaptureCtrl.instance.StopCapture();
        }
        
        if (CaptureCamera.GetComponent<VideoCaptureCtrl>().status == VideoCaptureCtrlBase.StatusType.FINISH)
        {
            Debug.Log("Video capture " + videoNumber + " processing finished.");
           

            plBetray.GetComponent<Image>().color = Color.grey;
            plCooperate.GetComponent<Image>().color = Color.grey;
            plCooperate.enabled = false;
            plBetray.enabled = false;

            Debug.Log("Exiting application.");
            Application.Quit();
        }

    }

    #endregion

    #region Secondary Methods

    public string OppType(int option)
    {
        string opponent;

        if (option == 0)
        {
            opponent = "Always Cooperate";
            return opponent;
        }
        else if (option == 1)
        {
            opponent = "Always Betray ";
            return opponent;
        }
        else if (option == 2)
        {
            opponent = "Tif-For-Tat";
            return opponent;
        }
        else if (option == 3)
        {
            opponent = "Pavlov";
            return opponent;
        }
        else if (option == 4)
        {
            opponent = "Random Pavlov";
            return opponent;
        }
        //else if (option == 5)
        //{
        //    opponent = "";
        //    return opponent;
        //}
        //else if (option == 6)
        //{
        //    opponent = "";
        //    return opponent;
        //}
        else return "Select Opponent order";
    }

    public void OppTypeLog(int option)
    {
        //if (gameCounter > 0)
        //{
        //    Debug.Log("Changing player (" + gameCounter + ")");
        //}

        oppType = option;
        if (option == 0)
        {
            Debug.Log("Opponent is Always Cooperate" + Environment.NewLine);
        }
        else if (option == 1)
        {
            Debug.Log("Opponent is Always Betray " + Environment.NewLine);
        }
        else if (option == 2)
        {
            Debug.Log("Opponent is Tit-For-Tat" + Environment.NewLine);
        }
        else if (option == 3)
        {
            Debug.Log("Opponent is Pavlov" + Environment.NewLine);
        }
        else if (option == 4)
        {
            Debug.Log("Opponent is Random Pavlov" + Environment.NewLine);
        }
        //else if (option == 5)
        //{
        //    Debug.Log("Opponent is Random Pavlov" + Environment.NewLine);
        //}
        //else if (option == 6)
        //{
        //    Debug.Log("Opponent is Random Pavlov" + Environment.NewLine);
        //}
    }

    public void Cooperated()
    {
        player_coop = true; //assign player decision
        ++pl_coop_counter;
        //Debug.Log("Player cooperated");

        PlayerDecision();
    }

    public void Betrayed()
    {
        player_coop = false; //assign player decision
        //Debug.Log("Player betrayed");

        PlayerDecision();
    }

    public void PlayerDecision()
    {

        OpponentDecision(oppType); //get opponent decision based on selected strategy
        //opponent_coop = (Random.value > 0.5f); //assign random opponent decision

        Debug.Log(OppType(oppType));

        playerChosen = true;

        if (totalRoundCounter < totalRounds) // if game is not over
        {
            playerDecisions[roundCounter] = player_coop; //add player decision to array
            opponentDecisions[roundCounter] = opponent_coop; //add opponent decision to array

            totalRoundCounter++; //increase round
            Debug.Log("Round: " + totalRoundCounter);
            roundCounter++; //increase game round

            if (roundCounter == roundsPerOpp) //if game ended
            {
                NewVideoSesion();
                gameCounter++;
                if (gameCounter < numberOfOpp)
                {
                    Debug.Log("Game: " + gameCounter);
                    oppType = OpponentOrder[gameCounter]; // next opponent
                    //Debug.Log("Opponent type: " + gameCounter);
                    OppTypeLog(oppType);
                }
                else
                {
                    Debug.Log("Game Ended!");
                    stopGame.SetActive(true);
                }
                roundCounter = 0;
            }
        }

        //set opponent button colours
        if (opponent_coop)
        {
            ++opp_coop_counter;
        }

        //if (totalRoundCounter == totalRounds)
        //{
        //    Behaviours();
        //}
    }

    public void IncreaseSlider()
    {
        DisablePlayerButtons();

        sliderFill.SetActive(true);
        totalSliderTime += Time.deltaTime;
        slider.value = Mathf.Lerp(0f, 1f, totalSliderTime / oppDelay);
        //Debug.Log("Opponent deciding.");

        if (slider.value == 1)
        {
            oppChosen = true;
            //Debug.Log("Opponent decided.");
            totalSliderTime = 0;
        }
    }

    public void DecreaseSlider()
    {
        scoreMessages.SetActive(true);
        sliderFill.SetActive(false);
        totalSliderTime += Time.deltaTime;
        slider.value = 1 - Mathf.Lerp(0f, 1f, totalSliderTime / newRoundDelay);


        if (slider.value == 0)
        {
            player_total_score += player_round_score;
            opponent_total_score += opponent_round_score;

            oppChosen = false;

            if (gameCounter < numberOfOpp)
            {
                EnablePlayerButtons();
            }

            //reset opponent button colours to white
            oppCooperate.GetComponent<Image>().color = Color.grey;
            oppBetray.GetComponent<Image>().color = Color.grey;
            scoreMessages.SetActive(false);

            totalSliderTime = 0;
        }
    }

    public void OpponentDecision(int option)
    {
        bool rand;

        //Always Coop
        if (option == 0)
        {
            opponent_coop = true;
        }

        //Always Betray
        else if (option == 1)
        {
            opponent_coop = false;
        }

        //Tif-For-Tat
        else if (option == 2)
        {
            if (roundCounter == 0)
            {
                opponent_coop = true;
            }
            else
            {
                opponent_coop = playerDecisions[roundCounter - 1];
            }
        }

        //Pavlov
        else if (option == 3)
        {
            if (roundCounter == 0)
            {
                opponent_coop = true;
            }
            else
            {
                if (opponent_round_score == reward || opponent_round_score == tempation)
                {
                    opponent_coop = opponentDecisions[roundCounter - 1];
                }
                else
                {
                    opponent_coop = !opponentDecisions[roundCounter - 1];
                }
            }
        }

        //Random Pavlov
        else if (option == 4)
        {
            if (roundCounter == 0) //on first round cooperate
            {
                opponent_coop = true;
            }
            else
            {
                rand = (UnityEngine.Random.value > 0.5f);
                if (rand) //randomly with 50-50 chance, either
                {
                    Debug.Log("Opponent randomly decided to do Pavlov."); //do Pavlov strategy
                    if (opponent_round_score == reward || opponent_round_score == tempation) //if previous round was reward or temptation
                    {
                        opponent_coop = opponentDecisions[roundCounter - 1]; //do same as previous round
                        Debug.Log("Opponent got + score");
                    }
                    else
                    {
                        opponent_coop = !opponentDecisions[roundCounter - 1]; //do opposite as previous decision
                        Debug.Log("Opponent did not get + score");
                    }
                }
                else
                {
                    Debug.Log("Opponent randomly decided to choose randomly.");
                    opponent_coop = (UnityEngine.Random.value > 0.5f); //or choose randomly
                }

                //if (option == 0) //Random
                //{
                //    if (roundCounter == 0)
                //    {
                //        opponent_coop = true;
                //    }
                //    else
                //    {
                //        opponent_coop = (UnityEngine.Random.value > 0.5f); //assign random opponent decision
                //    }
                //}

                //Grudger
                //else if (option == 4)
                //{
                //    if (roundCounter == 0)
                //    {
                //        opponent_coop = true;
                //    }
                //    else
                //    {
                //        if (playerDecisions[roundCounter - 1] == false || opponent_coop == false)
                //        {
                //            opponent_coop = false;
                //        }
                //        else
                //            opponent_coop = true;
                //    }
                //}

                //Pavlov

            }
        }
    }

    public void Behaviours()
    {
        int i;
        bool copycat = true;
        //bool detective = true;
        bool grudger = true;

        bool[] Cooperator = { true, true, true, true, true, true, true, true, true, true };
        bool[] Cheater = { false, false, false, false, false, false, false, false, false, false };
        //bool[] Detective = { true, false, true, true };

        //check if player is a copycat
        i = 0;
        while (i < totalRounds - 1 && copycat)
        {
            if (playerDecisions[i + 1] != opponentDecisions[i])
            {
                copycat = false;
            }
            i++;
        }

        //check if player is a grudger
        i = 0;
        bool betrayed = false;
        while (i < totalRounds - 1 && grudger)
        {
            if (!betrayed)
            {
                if (playerDecisions[i + 1] && !opponentDecisions[i])
                {
                    betrayed = true;
                }
            }
            else if (playerDecisions[i + 1])
            {
                grudger = false;
            }
            i++;
        }

        //check if player is a detective
        //i = 0;
        //while (i < 9 && detective)
        //{
        //    if (i < 5)
        //    {
        //        if (playerDecisions[i] != Detective[i])
        //        {
        //            detective = false;
        //        }
        //    }
        //    else // i <= 5
        //    {
        //        while (i < 9 && copycat)
        //        {
        //            if (playerDecisions[i + 1] != opponentDecisions[i])
        //            {
        //                copycat = false;
        //            }
        //            i++;
        //        }
        //    }
        //    i++;
        //}

        if (playerDecisions.SequenceEqual(Cooperator))
        {
            Debug.Log("Player is a Cooperator");
        }
        else if (playerDecisions.SequenceEqual(Cheater))
        {
            Debug.Log("Player is a Cheater");
        }
        else if (copycat)
        {
            Debug.Log("Player is a Copycat");
        }
        else if (grudger)
        {
            Debug.Log("Player is a Grudger");
        }
        else Debug.Log("Player is Random");
    }

    public void DisablePlayerButtons()
    {
        plCooperate.enabled = false;
        plBetray.enabled = false;

        if (player_coop)
        {
            plBetray.GetComponent<Image>().color = Color.grey;
        } else
            plCooperate.GetComponent<Image>().color = Color.grey;

        if (gameCounter == numberOfOpp)
        {
            plBetray.GetComponent<Image>().color = Color.grey;
            plCooperate.GetComponent<Image>().color = Color.grey;
        }
    }

    public void EnablePlayerButtons()
    {
        plCooperate.enabled = true;
        plBetray.enabled = true;
        plCooperate.GetComponent<Image>().color = Color.white;
        plBetray.GetComponent<Image>().color = Color.white;
    }

    //public void SaveTimestamp()
    //{
    //    string TimestampFileName = string.Format(@"Timestamps_{0}.txt", userID); ;
    //    string fullPath = Path.Combine(@pathString, TimestampFileName);
    //    //save timestamp to text file
    //    string timeStamp = (Time.time - startTime).ToString();
    //    File.AppendAllText(@fullPath, timeStamp + Environment.NewLine);
    //}

    public void NewVideoSesion()
    {
        // if video is recording, stop it
        if (CaptureCamera.GetComponent<VideoCaptureCtrl>().status == VideoCaptureCtrlBase.StatusType.STARTED)
        {
            VideoCaptureCtrl.instance.StopCapture(); //end video session
            Debug.Log("Video capture ended!");
        }

        // if video is finished processing, start new video
        if (CaptureCamera.GetComponent<VideoCaptureCtrl>().status == VideoCaptureCtrlBase.StatusType.FINISH)
        {
            Debug.Log("Video capture " + videoNumber + " processing finished.");

            if (gameCounter < numberOfOpp)
            {
                videoNumber++;
                frames = 0;
                VideoCaptureCtrl.instance.StartCapture(); //start new video session
                Debug.Log("New video capture " + videoNumber + " started!");

                GetComponent<LogHandler>().startTime = Time.time;
            }
            
            videoFinished = true;
        }

        // if video is stopped and is being processed, wait until it is finished
        if (CaptureCamera.GetComponent<VideoCaptureCtrl>().status == VideoCaptureCtrlBase.StatusType.STOPPED)
        {
            frames = 14400;
            videoFinished = false;
            //Debug.Log("Waiting to process video...");
        }
    }
    #endregion

}
