using UnityEngine;
using UnityEngine.UI;

public class SleepButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private AudioClip hoverClip;

    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        button.onClick.AddListener(DayManager.Instance.EndDay);
    }

    private void PlaySound(AudioClip c)
    {
        source.clip = c;
        source.Play();
    }

    public void HoverButton(Image i)
    {
        i.color = new Color(i.color.r * .7f, i.color.g * .7f, i.color.b * .7f, 255 * .8f);
        PlaySound(hoverClip);
    }

    public void UnhoverButton(Image i)
    {
        i.color = Color.white;
    }
}
