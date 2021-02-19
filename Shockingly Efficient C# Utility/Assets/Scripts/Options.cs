using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Options : MonoBehaviour
{
    public void SetFullscreen(bool isFullscreen)
    {
	    Screen.fullScreen = isFullscreen;
    }

   
    void Start()
    {
        
    }
}
