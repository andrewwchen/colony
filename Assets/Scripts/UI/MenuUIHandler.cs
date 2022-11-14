using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MenuUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private GameObject tooltipMenu;

    [SerializeField] private TextMeshProUGUI mmDate;
    [SerializeField] private TextMeshProUGUI mmTime;
    [SerializeField] private TextMeshProUGUI mmCash;

    [SerializeField] private Image[] invImgs;
    [SerializeField] private Button[] invButtons;
    [SerializeField] private Button invLeftButton;
    [SerializeField] private Button invRightButton;

    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemThumbnail;

    [SerializeField] private TextMeshProUGUI itemSellQuantity;
    [SerializeField] private TextMeshProUGUI itemSellTotal;
    [SerializeField] private Button itemLeftButton;
    [SerializeField] private Button itemRightButton;
    [SerializeField] private Button itemSellButton;
    [SerializeField] private Button itemSelectButton;

    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip errorClip;

    private InventoryManager im;
    private UniversalManipulator um;
    private DayManager dm;
    private int page = 0;
    private List<Item> heldItems = new List<Item>();
    private int quantity = 1;
    private Item currentItem;
    private AudioSource source;
    private Queue<AudioClip> audioQueue = new Queue<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        im = InventoryManager.Instance;
        um = UniversalManipulator.Instance;
        dm = DayManager.Instance;

        source = GetComponent<AudioSource>();

        invLeftButton.onClick.AddListener(LastPage);
        invRightButton.onClick.AddListener(NextPage);
        im.OnInventoryChange.AddListener(ResetHeldItems);
        im.OnInventoryChange.AddListener(ResetItemCount);
        im.OnMoneyChange.AddListener(ResetCashText);
        itemLeftButton.onClick.AddListener(SubtractItem);
        itemRightButton.onClick.AddListener(AddItem);
        itemSellButton.onClick.AddListener(Sell);
        itemSelectButton.onClick.AddListener(Select);
        dm.OnStartDay.AddListener(ResetDateText);
        dm.OnTimeChange.AddListener(ResetTimeText);

        ResetHeldItems();
        ResetCashText();
        ResetDateText();
        ResetTimeText();

        showMainMenu();
    }

    private void PlaySound(AudioClip c)
    {
        audioQueue.Enqueue(c);
        if (source.clip == null) StartCoroutine(PlaySoundQueue());
    }

    IEnumerator PlaySoundQueue()
    {
        while (audioQueue.Count != 0)
        {
            source.clip = audioQueue.Dequeue();
            source.Play();
            yield return new WaitForSeconds(source.clip.length);
        }
        source.clip = null;
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

    private void NextPage()
    {
        page += 1;
        ResetPage();
    }

    private void LastPage()
    {
        page -= 1;
        ResetPage();
    }

    private void ResetHeldItems()
    {
        heldItems.Clear();

        for (int i = 0; i < im.items.Length; i++)
        {
            Item item = im.items[i];
            if (im.inventory[item] >= 1)
                heldItems.Add(item);
        }

        ResetPage();
    }

    private void ResetPage()
    {
        invLeftButton.gameObject.SetActive(page > 0);
        invRightButton.gameObject.SetActive(page < Mathf.CeilToInt((heldItems.Count - 1) / invImgs.Length));

        for (int i = 0; i < invImgs.Length; i++)
        {
            invButtons[i].onClick.RemoveAllListeners();
            int invIndex = page * invImgs.Length + i;
            if (invIndex < heldItems.Count)
            {
                Item item = heldItems[invIndex];
                invImgs[i].sprite = item.thumbnail;
                invImgs[i].gameObject.SetActive(true);
                invButtons[i].onClick.AddListener(delegate { SelectItem(item); });
            }
            else
            {
                invImgs[i].gameObject.SetActive(false);
            }
        }
    }
    private void AddItem()
    {
        quantity += 1;
        ResetQuantity();
    }

    private void SubtractItem()
    {
        quantity -= 1;
        ResetQuantity();
    }

    private void ResetQuantity()
    {
        quantity = Mathf.Clamp(quantity, 1, im.inventory[currentItem]);
        itemLeftButton.gameObject.SetActive(quantity != 1);
        itemRightButton.gameObject.SetActive(quantity < im.inventory[currentItem]);
        itemSellQuantity.text = quantity.ToString();
        itemSellTotal.text = Utils.MoneyToString(currentItem.sellPrice * quantity);
    }

    private void Sell()
    {
        im.SellItem(currentItem, quantity);
        quantity = 1;
        ResetQuantity();
    }

    private void ResetItemCount()
    {
        if (currentItem != null)
        {
            int itemCount = im.inventory[currentItem];
            itemName.text = currentItem.displayName + " x " + itemCount;
            ResetQuantity();

            if (itemCount == 0)
            {
                showInventoryMenu();
            }
        }
    }

    private void SelectItem(Item i)
    {
        itemDescription.text = i.description;
        itemThumbnail.sprite = i.thumbnail;
        quantity = 1;
        currentItem = i;
        ResetItemCount();

        showTooltipMenu();
    }

    private void ResetCashText()
    {
        mmCash.text = im.GetBalance();
    }

    private void ResetDateText()
    {
        mmDate.text = "Day " + dm.day;
    }

    private void ResetTimeText()
    {
        mmTime.text = dm.GetDisplayTime();
    }


    private void Select()
    {
        switch (currentItem.useType)
        {
            case ItemUseType.Seed:
                um.SetMode(UMMode.Planting, currentItem);
                break;
            case ItemUseType.Structure:
                um.SetMode(UMMode.Building, currentItem);
                break;
            default:
                PlaySound(errorClip);
                break;
        }
    }

    public void toggleDisplay()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void showMainMenu()
    {
        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
        tooltipMenu.SetActive(false);
    }

    public void showInventoryMenu()
    {
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(true);
        tooltipMenu.SetActive(false);
    }

    public void showTooltipMenu()
    {
        ResetQuantity();
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        tooltipMenu.SetActive(true);
    }
}
