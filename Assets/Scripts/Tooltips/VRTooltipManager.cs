using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRTooltipManager : MonoBehaviour
{
    public static VRTooltipManager Instance { get; private set; }
    public float camDistance = .5f;
    public float camHeight = -.1f;
    public float smoothTime = .3f;
    public float tooltipSeconds = 1f;
    public bool doLockVertical = true;
    public TextMeshProUGUI text;
    public GameObject canvas;
    private Transform camTransform;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        camTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas.activeSelf)
        {
            UpdatePositionSmooth();
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        return camTransform.position + new Vector3(0, camHeight, 0) + (doLockVertical ? Vector3.ProjectOnPlane(camTransform.rotation * Vector3.forward, Vector3.up) : camTransform.rotation * Vector3.forward).normalized * camDistance;
    }

    private void UpdatePositionSmooth()
    {
        Vector3 targetPosition = CalculateTargetPosition();
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        transform.LookAt(camTransform.position);
        transform.Rotate(Vector3.up * 180f, Space.Self);
    }

    public void ShowTooltip(string tooltip, float time=-1)
    {
        StopAllCoroutines();
        text.text = tooltip;
        if (!canvas.activeSelf)
        {
            Vector3 targetPosition = CalculateTargetPosition();
            transform.position = targetPosition;
            UpdatePositionSmooth();
            canvas.SetActive(true);
        }
        StartCoroutine(HideTooltipDelay(time));
    }

    private IEnumerator HideTooltipDelay(float time)
    {
        yield return new WaitForSeconds(time > 0 ? time : tooltipSeconds);
        canvas.SetActive(false);
    }

    public void HideTooltip()
    {
        StopAllCoroutines();
        canvas.SetActive(false);
    }
}
