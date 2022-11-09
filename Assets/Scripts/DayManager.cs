using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-1)]
public class DayManager : MonoBehaviour
{
    // singleton instance
    public static DayManager Instance;

    [SerializeField] private string[] lightingSetIds;
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

    private DataManager dm;
    private InventoryManager im;
    private StructureManager sm;
    private FadeController fc;
    private Dictionary<string, LightingSet> idToLightingSet;
    private Dictionary<string, GameObject> idToLight = new Dictionary<string, GameObject>();
    private Dictionary<string, LightmapData> idToLightmap = new Dictionary<string, LightmapData>();
    private float dayStartTime;
    private string currentLightingSetId;
    private bool dayEnded = false;

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
        day = dm.gameData.day;
        fc = FadeController.Instance;
        idToLightingSet = Utils.Zip(lightingSetIds, lightingSets);
        for (int i = 0; i < lightingSetIds.Length; i++)
        {
            string id = lightingSetIds[i];
            LightingSet lightingSet = idToLightingSet[id];
            GameObject go = Instantiate(lightingSet.mainLight);
            go.SetActive(false);
            idToLight.Add(id, go);
            LightmapData ld = new LightmapData();
            ld.lightmapColor = lightingSet.lightmapColor;
            ld.lightmapDir = lightingSet.lightmapDir;
            idToLightmap.Add(id, ld);
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
            idToLight[currentLightingSetId].SetActive(false);

        currentLightingSetId = id;
        GameObject currentLight = idToLight[id];
        currentLight.SetActive(true);
        LightingSet lightingSet = idToLightingSet[id];
        RenderSettings.skybox = lightingSet.skyboxMat;
        RenderSettings.sun = currentLight.GetComponent<Light>();
        LightmapSettings.lightmaps = new LightmapData[]{ idToLightmap[id]};
    }

    public void EndDay()
    {
        if (!dayEnded)
        {
            dayEnded = true;
            fc.OnFadeEnd += OnFadeEnd;
            fc.StartFade();
        }
    }

    private void OnFadeEnd()
    {
        fc.OnFadeEnd -= OnFadeEnd;
        OnEndDay.Invoke();
        day += 1;
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
 