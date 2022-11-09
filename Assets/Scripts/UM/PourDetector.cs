using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * PourDetector class for watering can
 * Author: Henry Kim
 * Based on online tutorial https://www.youtube.com/watch?v=hyiyjUEReYg
 */
public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 25;
    public Transform origin = null;
    public GameObject particleSystem = null;

    private bool isPouring = false;
    private Stream currentStream = null;

    // Update is called once per frame
    private void Update()
    {
        bool pourCheck = CalculatePourAngle() < pourThreshold;
        if (isPouring != pourCheck) {
            isPouring = pourCheck;
            particleSystem.SetActive(!isPouring);
        }
    }

    private float CalculatePourAngle() {
        float zAngle = 180 - Mathf.Abs(180 - transform.rotation.eulerAngles.z);
        float xAngle = 180 - Mathf.Abs(180 - transform.rotation.eulerAngles.x);
        return Mathf.Max(zAngle, xAngle);
    }

}
