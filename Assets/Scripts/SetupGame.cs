using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;
using System.IO;

public class SetupGame : MonoBehaviour {

    #region Game Parameters
    [Header("Game Parameters")]

    public int reward = 2;
    public int tempation = 3;
    public int sucker = -1;
    public int punishment = 0;
    public int numberOfOpp = 5;
    public int roundsPerOpp = 25;
    public float oppDelay;
    public float newRoundDelay;
    public int maxOppDelay = 5;
    public int maxNewRoundDelay = 5;

    public int oppType;
    public static string userID;
    public static string parentFolderPath;
    public static string folderPath;

    public int[] OpponentOrder;

    #endregion

    #region UI Elements

    [Header("UI Elements")]

    public InputField pathInput;
    public InputField userInput;
    public Text userMessage;

    public GameObject oppSelect;
    public Text playerOrder;

    public Text oppDelaySliderText;
    public Text newRoundDelaySliderText;

    public Text camName;

    public Toggle customToggle;
    public GameObject CustomOrder;
    public Dropdown opp1;
    public Dropdown opp2;
    public Dropdown opp3;
    public Dropdown opp4;
    public Dropdown opp5;
    //public Dropdown opp6;
    //public Dropdown opp7;

    public Slider oppDelaySlider;
    public Slider newRoundDelaySlider;

    public GameObject StartButton;

    #endregion

    #region Canvases

    [Header("Canvases")]
    public GameObject mainCanvas;
    public GameObject cameraCanvas;
    public GameObject captureCamera;

    #endregion

    #region Main Methods

    void Start() {

        OpponentOrder = new int[] { opp1.value, opp2.value, opp3.value, opp4.value, opp5.value }; //, opp6.value, opp7.value };
        CustomOrder.SetActive(false);
        userMessage.enabled = false;
        mainCanvas.SetActive(false);
        captureCamera.SetActive(false);
        //cameraCanvas.SetActive(false);
        oppSelect.SetActive(false);
        StartButton.SetActive(false);
        //oppSelect.SetActive(true);
        //StartButton.SetActive(true);
    }

    void Update() {

        WebCamDevice[] camDevices = WebCamTexture.devices;
        camName.text = "WebCam: " + camDevices[0].name;

        playerOrder.text = "Opponent Order will be: " + Environment.NewLine +
            "1." + OppType(OpponentOrder[0]).ToString() + Environment.NewLine +
            "2." + OppType(OpponentOrder[1]).ToString() + Environment.NewLine +
            "3." + OppType(OpponentOrder[2]).ToString() + Environment.NewLine +
            "4." + OppType(OpponentOrder[3]).ToString() + Environment.NewLine +
            "5." + OppType(OpponentOrder[4]).ToString() + Environment.NewLine //+
            //"6." + OppType(OpponentOrder[5]).ToString() + Environment.NewLine +
            //"7." + OppType(OpponentOrder[6]).ToString()
            ;
    }
    
    public void StartGame()
    {
        mainCanvas.SetActive(true);
        cameraCanvas.SetActive(true);
        captureCamera.SetActive(true);

    }

    public void ExitApp()
    {
        Debug.Log("Exiting application.");
        Application.Quit();
    }

    #endregion

    #region Secondary Methods

    #region User Input

    //public void SelectFolder()
    //{
    //    string path = EditorUtility.OpenFolderPanel("Load png Textures", "", "");
    //    string[] files = Directory.GetFiles(path);

    //    foreach (string file in files)
    //        if (file.EndsWith(".png"))
    //            File.Copy(file, EditorApplication.currentScene);

    //    parentFolderPath = path;
    //}

    public void GetPath()
    {
        parentFolderPath = Application.dataPath + "/Users/";
        if (!Directory.Exists(parentFolderPath))
        {
            Directory.CreateDirectory(parentFolderPath);
        }
        //parentFolderPath = Application.dataPath; /*pathInput.text;*/
    }

    public void GetUser()
    {
        userID = userInput.text;
        folderPath = Path.Combine(parentFolderPath, userID);

        userMessage.enabled = false;

        if (folderPath != parentFolderPath)
        {
            userMessage.enabled = true;
            if (Directory.Exists(@folderPath))
            {
                userMessage.text = "This userID already exists! Please enter a different userID.";
                Debug.Log("UserID already exists!");
                return;
            }
            else
            {
                userMessage.text = "Saving game info at: " + folderPath;
            }
        }
    }

    #endregion

    public void SetOppDelay()
    {
        oppDelay = oppDelaySlider.value * maxOppDelay;
        oppDelaySliderText.text = "Opponent decision delay: " + oppDelay + " seconds".ToString();
    }

    public void SetNewRoundDelay()
    {
        newRoundDelay = newRoundDelaySlider.value * maxNewRoundDelay;
        newRoundDelaySliderText.text = "Next round delay: " + newRoundDelay + " seconds".ToString();
    }

    public void SaveUser()
    {
        if (Directory.Exists(@folderPath))
        {
            return;
        }
        else
        {
            userID = userInput.text.ToString();
            Directory.CreateDirectory(folderPath);

            Debug.Log("Got userID: " + userID + Environment.NewLine);
            userInput.enabled = false;
            userInput.GetComponent<Image>().color = Color.grey;
            oppSelect.SetActive(true);

            StartButton.SetActive(true);

        }
    }


    #region Opponent Types

    public string OppType(int option)
    {
        string opponent;
        if (option != 7)
        {
            if (option == 0)
            {
                opponent = "Always Cooperate";
                return opponent;
            }
            else if (option == 1)
            {
                opponent = "Always Betray";
                return opponent;
            }
            else if (option == 2)
            {
                opponent = "Tit-For-Tat";
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
            //    opponent = "Pavlov";
            //    return opponent;
            //}
            //else if (option == 6)
            //{
            //    opponent = "Random Pavlov";
            //    return opponent;
            //}
            else return "Select Opponent order";
        }
        else
        {
            return "Select Opponent order";
        }
    }

    public void Opponent1()
    {
        OpponentOrder[0] = opp1.value;
    }

    public void Opponent2()
    {
        OpponentOrder[1] = opp2.value;
    }

    public void Opponent3()
    {
        OpponentOrder[2] = opp3.value;
    }

    public void Opponent4()
    {
        OpponentOrder[3] = opp4.value;
    }

    public void Opponent5()
    {
        OpponentOrder[4] = opp5.value;
    }

    //public void Opponent6()
    //{
    //    OpponentOrder[5] = opp6.value;
    //}

    //public void Opponent7()
    //{
    //    OpponentOrder[6] = opp7.value;
    //}

    #endregion

    #region Opponent Order

    public void DefaultOrder()
    {
        OpponentOrder = new int[] { 0 , 1, 2, 3, 4 }; //, 4, 0 };
    }

    public void ToggleCustomOrder()
    {
        CustomOrder.SetActive(customToggle.isOn);
        if (customToggle.isOn)
        {
            OpponentOrder[0] = opp1.value;
            OpponentOrder[1] = opp2.value;
            OpponentOrder[2] = opp3.value;
            OpponentOrder[3] = opp4.value;
            OpponentOrder[4] = opp5.value;
            //OpponentOrder[5] = opp6.value;
            //OpponentOrder[6] = opp7.value;
        }
        else
        {
            DefaultOrder();
        }
    }

    public void RandomizeOppponentOrder()
    {
        //System.Random rnd = new System.Random();
        //OpponentOrder = OpponentOrder.OrderBy(x => rnd.Next()).ToArray();
        //for (int i = 0; i < numberOfOpp; i++)
        //{
        //    Debug.Log(OpponentOrder[i]);
        //}

        System.Random rnd = new System.Random();
        int randomIndex = rnd.Next(0, 4);

        if (randomIndex == 1)
        {
            OpponentOrder = new int[] { 1, 2, 3, 4, 0 };
        }
        else if (randomIndex == 2)
        {
            OpponentOrder = new int[] { 2, 3, 4, 0, 1 };
        }
        else if (randomIndex == 3)
        {
            OpponentOrder = new int[] { 3, 4, 0, 1, 2 };
        }
        else if (randomIndex == 4)
        {
            OpponentOrder = new int[] { 4, 0, 1, 2, 3 };
        }
        else
        {
            return;
        }
    }

    #endregion

    #endregion

}
