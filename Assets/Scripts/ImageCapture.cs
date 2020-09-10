using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.WSA;
//using UnityEngine.XR.WSA.WebCam;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

public class ImageCapture : MonoBehaviour
{
    public GameObject setupCanvas;
    public string userID;
    public string pathString;

    //public string pathString = @"C:\Users\Jason\Documents\Prisoner's Dilemma\Captures";


    public string textFileName = "TimeStamps.txt";
    string textFilePath;

    //public Button plCooperate;
    //public Button plBetray;

    public RawImage rawimage;  //Image for rendering what the camera sees.
    WebCamTexture webcamTexture_obj = null;

    public int frames = 0;

    void Start()
    {
        //plCooperate.onClick.AddListener(SaveImage);
        //plBetray.onClick.AddListener(SaveImage);

        //userID = setupCanvas.GetComponent<SetupGame>().userID;
        //pathString = setupCanvas.GetComponent<SetupGame>().folderPath;

        //textFilePath = Path.Combine(pathString, textFileName);

        //if (File.Exists(@textFilePath))
        //{
        //    try
        //    {
        //        File.Delete(@textFilePath);
        //    }
        //    catch (IOException e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return;
        //    }
        //}

        //Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
        //float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();

        //Save get the camera devices, in case you have more than 1 camera.
        WebCamDevice[] camDevices = WebCamTexture.devices;

        //Get the used camera name for the WebCamTexture initialization.
        string camName = camDevices[0].name;

        //Debug.Log("Camera name: " + camName + Environment.NewLine);
        //Debug.Log("Camera Resolution: " + cameraResolution);
        //Debug.Log("Camera framerate: " + cameraFramerate);

        webcamTexture_obj = new WebCamTexture(camName, 1920, 1080, 30);

        //webcamTexture_obj.requestedWidth = 1008;

        Debug.Log(camName);
        //webcamTexture_obj.requestedWidth = 1920;
        //webcamTexture_obj.requestedHeight = 1080;

        //webcamTexture_obj.width = 1920;
        //webcamTexture_obj.height = 1080;

        //Debug.Log("Requested texture width: " + webcamTexture_obj.requestedWidth);
        //Debug.Log("Requested texture height: " + webcamTexture_obj.requestedHeight);

        //webcamTexture_obj.filterMode = FilterMode.Trilinear;

        //Debug.Log("Texture width: " + webcamTexture_obj.width);
        //Debug.Log("Texture height: " + webcamTexture_obj.height);


        //Render the image in the screen.
        rawimage.texture = webcamTexture_obj;
        //rawimage.material.mainTexture = webcamTexture_obj;
        webcamTexture_obj.Play();
    }

    void Update()
    {
        //This is to take the picture, save it and stop capturing the camera image.
        //if (webcamTexture_obj.width < 50)
        //{
        //    Debug.Log("Still waiting another frame for correct info...");
        //    return;
        //}
        //else
        //    webcamTexture_obj = new WebCamTexture(WebCamTexture.devices[0].name);

        //    rawimage.texture = webcamTexture;
        //    rawimage.material.mainTexture = webcamTexture;
        //    webcamTexture.Play();

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    SaveImage();
        //    webcamTexture_obj.Stop();
        //}

        frames++;
        if (frames % 60 == 0)
        {
            //Debug.Log("Texture width: " + webcamTexture_obj.width);
            //Debug.Log("Texture height: " + webcamTexture_obj.height);
        }
    }

    //public void getUser()
    //{
    //    userID = userInput.text;
    //    Debug.Log("Got userID: " + userID + Environment.NewLine);
    //    //userInput.enabled = false;
    //    //userInput.GetComponent<Image>().color = Color.grey;
    //}


    public Texture2D SaveTexture()
    {
        Texture2D texture = new Texture2D(rawimage.texture.width, rawimage.texture.height, TextureFormat.ARGB32, false);

        //Save the image to the Texture2D
        texture.SetPixels(webcamTexture_obj.GetPixels());
        texture.Apply();

        ////Encode it as a PNG.
        //byte[] bytes = texture.EncodeToPNG();

        //save texture in a file
        //string captureName = string.Format(@"User_{0}_{1}.png", userID, Time.time); //set capture name with timestamp
        //string filePath = Path.Combine(pathString, captureName); //set capture location
        //File.WriteAllBytes(filePath + ".png", bytes); //write bytes to file

        return texture;
    }
    //public void SaveImage()
    //{

    //    //Debug.Log("Final image width: " + rawimage.texture.width);
    //    //Debug.Log("Final image height: " + rawimage.texture.height);
    //    //Create a Texture2D with the size of the rendered image on the screen.
    //    Texture2D texture = new Texture2D(rawimage.texture.width, rawimage.texture.height, TextureFormat.ARGB32, false);

    //    //Save the image to the Texture2D
    //    texture.SetPixels(webcamTexture_obj.GetPixels());
    //    texture.Apply();

    //    //Encode it as a PNG.
    //    byte[] bytes = texture.EncodeToPNG();

    //    //save texture in a file
    //    string captureName = string.Format(@"User_{0}_{1}.png", userID, Time.time); //set capture name with timestamp
    //    string filePath = Path.Combine(pathString, captureName); //set capture location
    //    File.WriteAllBytes(filePath + ".png", bytes); //write bytes to file
    //    //Debug.Log("Photo saved at " + filePath);

    ////save timestamp to text file
    //string timeStamp = Time.time.ToString();
    //File.AppendAllText(@textFilePath, timeStamp + Environment.NewLine);
    //}

}
