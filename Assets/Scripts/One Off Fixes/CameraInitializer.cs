using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraInitializer : MonoBehaviour
{
    private void OnEnable()
    {
        // Invalidate the confiner cache to ensure it's up to date
        var confiner = GetComponent<CinemachineConfiner2D>();
        if (confiner != null)
        {
            StartCoroutine(InvalidateConfinerCache(confiner));
        }
    }

    private IEnumerator InvalidateConfinerCache(CinemachineConfiner2D confiner)
    {
        yield return new WaitForEndOfFrame();
        confiner.InvalidateBoundingShapeCache();
    }
}