using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VRTooltipManager))]
public class IntroSequence : MonoBehaviour
{
    [SerializeField] private string[] pages;
    [SerializeField] private float nextLetterInterval = .05f;
    [SerializeField] private float nextPageInterval = 3f;
    private VRTooltipManager vrtm;

    // Start is called before the first frame update
    void Start()
    {
        vrtm = GetComponent<VRTooltipManager>();
        StartCoroutine(Go(0, 0));
    }

    private IEnumerator Go(int pageIndex, int letterIndex)
    {
        bool isPageEnd = letterIndex == pages[pageIndex].Length - 1;
        bool isSequenceEnd = isPageEnd && pageIndex == pages.Length - 1;
        float waitTime = isPageEnd ? nextPageInterval : nextLetterInterval;
        vrtm.ShowTooltip(pages[pageIndex].Substring(0, letterIndex + 1), 2*waitTime);
        yield return new WaitForSeconds(waitTime);
        if (!isSequenceEnd)
            StartCoroutine(Go(isPageEnd ? pageIndex + 1 : pageIndex, isPageEnd ? 0 : letterIndex + 1));
    }
}
