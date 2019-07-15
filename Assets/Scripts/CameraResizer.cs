using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class CameraResizer : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float targetWidth;
    [SerializeField] private bool alignWidth;
    private void OnValidate()
    {
        if (!targetCamera)
            targetCamera = GetComponent<Camera>();
        
        if (targetCamera)
            targetCamera.orthographic = true;
    }

    private void LateUpdate()
    {
        if (alignWidth)
            targetCamera.orthographicSize = (targetWidth * targetCamera.pixelHeight) / (2 * targetCamera.pixelWidth);
        else
            targetCamera.orthographicSize = targetWidth / 2;

    }
}
