using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppSetting : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        Application.targetFrameRate = 60;
    }
    
}
