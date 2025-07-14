using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioObjectController : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
        
        if (Application.platform != RuntimePlatform.Android)
        {
            Debug.LogError($"Aspect Ratio: {_camera.aspect}" + Application.platform);
            SetCameraFieldView(_camera.aspect.ToString());
        }
    }

    private void SetCameraFieldView(string aspectRatio)
    {
        switch (aspectRatio)
        {
            case "1.777778":
                _camera.fieldOfView = 38;
                break;
            case "1.185185":
                _camera.fieldOfView = 49;
                break;
            case "1.778646":
                _camera.fieldOfView = 38f;
                break;
            case "1.6":
                _camera.fieldOfView = 38;
                break;
            case "1.555556":
                _camera.fieldOfView = 39;
                break;
            case "1.333333":
                _camera.fieldOfView = 45;
                break;
            case "1.25":
                _camera.fieldOfView = 48;
                break;
            case "1.666667":
                _camera.fieldOfView = 37;
                break;
            case "1.692708":
                _camera.fieldOfView = 39.5f;

                break;


        }
    }


}
