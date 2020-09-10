using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Display.displays[0] is the primary, default display and is always ON.
        // Check if additional displays are available and activate each.
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Display.displays[1].SetParams(1920, 1080, 0, 0);
        }
        if (Display.displays.Length > 2)
        {
            Display.displays[2].Activate();
            Display.displays[2].SetParams(1920, 1080, 1920, 0);
        }
        Debug.Log("displays connected: " + Display.displays.Length);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
