using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class DayManager : MonoBehaviour
{
    // singleton instance
    public static DayManager Instance;

    [SerializeField] private LightingSet[] lightingSets;

    [HideInInspector] public int day;
    [HideInInspector] public UnityEvent OnEndDay;
    [HideInInspector] public UnityEvent OnStartDay;
    [HideInInspector] public UnityEvent OnTimeChange;
    [HideInInspector] public int time = 0; // time in in-game minutes since the day began

    private static float realSecondsPerGameMinute = 0.5f;

    private static int wakeHour = 6;
    private static int duskHour = 18;
    private static int sleepHour = 22;
    private static float realSecondsPerLightSwap = 1f;

    private DataManager dm;
    private InventoryManager im;
    private StructureManager sm;
    private UniversalManipulator um;
    private FadeController fc;
    private Dictionary<string, LightingSet> idToLightingSet = new Dictionary<string, LightingSet>();
    private Dictionary<string, LightmapData[]> idToLightmap = new Dictionary<string, LightmapData[]>();
    private float dayStartTime;
    private string currentLightingSetId = "night";
    private bool dayEnded = false;
    private Light sun;
    private Material skybox;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        dm = DataManager.Instance;
        im = InventoryManager.Instance;
        sm = StructureManager.Instance;
        um = UniversalManipulator.Instance;
        day = dm.gameData.day;
        fc = FadeController.Instance;
        sun = RenderSettings.sun;
        skybox = RenderSettings.skybox;

        for (int i = 0; i < lightingSets.Length; i++)
        {
            LightingSet lightingSet = lightingSets[i];
            string id = lightingSet.id;
            idToLightingSet[id] = lightingSet;
            LightmapData ld = new LightmapData();
            ld.lightmapColor = lightingSet.lightmapColor;
            ld.lightmapDir = lightingSet.lightmapDir;
            idToLightmap[id] = new LightmapData[] { ld };
        }
        UseLightingSet("day");
        dayStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        SetTime();
        if (currentLightingSetId == "day" && time > (duskHour - wakeHour) * 60)
        {
            UseLightingSet("night");
        } else if (time >= (sleepHour - wakeHour) * 60) {
            EndDay();
        }
    }

    private void SetTime()
    {
        float realSeconds = Time.time - dayStartTime;
        float gameMinutes = realSeconds / realSecondsPerGameMinute;
        int newTime = Mathf.Clamp(Mathf.FloorToInt(gameMinutes), 0, (sleepHour - wakeHour) * 60);
        if (dayEnded)
        {
            newTime = (sleepHour - wakeHour) * 60;
        }
        if (newTime != time)
        {
            time = newTime;
            OnTimeChange?.Invoke();
        }
    }

    public string GetDisplayTime()
    {
        int gameMinutes = wakeHour * 60 + time;
        int gameHours = Mathf.FloorToInt(gameMinutes / 60f);
        int gameMinutesRemainder = Mathf.FloorToInt(gameMinutes % 60f);
        return string.Format("{0:D2}:{1:D2}", gameHours, gameMinutesRemainder);
    }

    private void UseLightingSet(string id)
    {
        if (currentLightingSetId != null)
        {
            if (currentLightingSetId == id)
                return;
        }

        StopAllCoroutines();
        StartCoroutine(SwapLightingSet(currentLightingSetId, id));
        currentLightingSetId = id;
    }

    private IEnumerator SwapLightingSet(string id1, string id2)
    {
        float startTime = Time.time;
        float elapsedTime;
        while ((elapsedTime = Time.time - startTime) < realSecondsPerLightSwap)
        {
            LerpLightingSet(idToLightingSet[id1], idToLightingSet[id2], elapsedTime/realSecondsPerLightSwap);
            yield return new WaitForFixedUpdate();
        }
    }

    private void LerpLightingSet(LightingSet set1, LightingSet set2, float t)
    {
        // Lerp sun intensity, angle, and color
        sun.intensity = Mathf.Lerp(set1.light.intensity, set2.light.intensity, t);
        sun.transform.rotation = Quaternion.Lerp(set1.light.transform.rotation, set2.light.transform.rotation, t);
        sun.color = Color.Lerp(set1.light.color, set2.light.color, t);

        // Lerp skybox shader values
        skybox.Lerp(set1.skyboxMat, set2.skyboxMat, t);

        // Swap lightmaps after reaching threshold
        if (t < 0.5f)
        {
            LightmapSettings.lightmaps = idToLightmap[set1.id];
        } else
        {
            LightmapSettings.lightmaps = idToLightmap[set2.id];
        }
    }

    public void EndDay()
    {
        if (!dayEnded)
        {
            dayEnded = true;
            SetTime();
            fc.OnFadeEnd += OnFadeEnd;
            fc.StartFade();
        }
    }

    private void OnFadeEnd()
    {
        fc.OnFadeEnd -= OnFadeEnd;
        OnEndDay.Invoke();
        day += 1;
        um.resetEnergy();
        int money = im.money;
        ItemData[] id = im.Serialize();
        StructureData[] sd = sm.Serialize();
        dm.UpdateData(day, money, id, sd);
        dm.Save();
        StartCoroutine(WhileFaded());
    }

    private IEnumerator WhileFaded()
    {
        OnStartDay?.Invoke();
        yield return new WaitForSeconds(2f);
        dayStartTime = Time.time;
        UseLightingSet("day");
        dayEnded = false;
        fc.OnUnfadeEnd += OnUnfadeEnd;
        fc.StartUnfade();
    }

    private void OnUnfadeEnd()
    {
        fc.OnUnfadeEnd -= OnUnfadeEnd;
    }
}
 