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
    [HideInInspector] public int time = 0; // time in minutes

    private static float realSecondsPerGameMinute = 0.1f;
    private static int wakeHour = 6;
    private static int duskHour = 21;
    private static int sleepHour = 22;

    private DataManager dm;
    private InventoryManager im;
    private StructureManager sm;
    private FadeController fc;
    private Dictionary<string, LightingSet> idToLightingSet;
    private Dictionary<string, GameObject> idToLight = new Dictionary<string, GameObject>();
    private GameObject currentLight;
    private Dictionary<string, LightmapData> idToLightmap = new Dictionary<string, LightmapData>();
    private float dayStartTime;

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
    }

    private void SetTime()
    {
        float realSeconds = dayStartTime - Time.time;
        float gameMinutes = realSeconds / realSecondsPerGameMinute;
        int newTime = Mathf.FloorToInt(gameMinutes);
        if (newTime != time)
        {
            time = newTime;
        }
    }

    private void UseLightingSet(string id)
    {
        if (currentLight != null)
            currentLight.SetActive(false);
        currentLight = idToLight[id];
        currentLight.SetActive(true);
        LightingSet lightingSet = idToLightingSet[id];
        RenderSettings.skybox = lightingSet.skyboxMat;
        RenderSettings.sun = currentLight.GetComponent<Light>();
        LightmapSettings.lightmaps = new LightmapData[]{ idToLightmap[id]};
    }

    public void EndDay()
    {
        UseLightingSet("night");
        fc.OnFadeEnd += OnFadeEnd;
        fc.StartFade();
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
        fc.OnUnfadeEnd += OnUnfadeEnd;
        fc.StartUnfade();
        dayStartTime = Time.time;
    }

    private void OnUnfadeEnd()
    {
        fc.OnUnfadeEnd -= OnUnfadeEnd;
    }
}
 