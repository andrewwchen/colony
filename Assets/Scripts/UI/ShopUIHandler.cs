using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUIHandler : MonoBehaviour
{

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inventoryMenu;
    [SerializeField] private TextMeshProUGUI[] cash;
    [SerializeField] private Image[] slotImgs;
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemThumbnail;
    [SerializeField] private TextMeshProUGUI itemCost;
    [SerializeField] private TextMeshProUGUI itemTotal;
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private Button itemLeft;
    [SerializeField] private Button itemRight;
    [SerializeField] private Button itemBuy;
    [SerializeField] private Animator baldorAnim;

    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip cashClip;

    private InventoryManager im;
    private Transform cam;
    private int page = 0;
    private int quantity = 1;
    private Item currentItem;
    private AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        im = InventoryManager.Instance;

        source = GetComponent<AudioSource>();

        leftButton.onClick.AddListener(LastPage);
        rightButton.onClick.AddListener(NextPage);
        im.OnMoneyChange.AddListener(ResetCashText);
        itemLeft.onClick.AddListener(SubtractItem);
        itemRight.onClick.AddListener(AddItem);
        itemBuy.onClick.AddListener(Buy);

        ResetPage();
        ResetCashText();

        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
        gameObject.SetActive(false);
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

    private void ResetCashText()
    {
        foreach (TextMeshProUGUI c in cash)
        {
            c.text = im.GetBalance();
        }
        baldorAnim.SetTrigger("Happy");
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

    private void ResetPage()
    {
        leftButton.gameObject.SetActive(page > 0);
        rightButton.gameObject.SetActive(page <= Mathf.CeilToInt(im.buyables.Count / slotImgs.Length));

        for (int i = 0; i < slotImgs.Length; i++)
        {
            slotButtons[i].onClick.RemoveAllListeners();
            int buyableIndex = page * slotImgs.Length + i;
            if (buyableIndex < im.buyables.Count)
            {
                Item buyable = im.buyables[buyableIndex];
                slotImgs[i].sprite = buyable.thumbnail;
                slotImgs[i].gameObject.SetActive(true);
                slotButtons[i].onClick.AddListener(delegate { SelectItem(buyable); });
            } else
            {
                slotImgs[i].gameObject.SetActive(false);
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

    private void SelectItem(Item i)
    {
        itemName.text = i.displayName;
        itemDescription.text = i.description;
        itemCost.text = Utils.MoneyToString(i.buyPrice);
        itemThumbnail.sprite = i.thumbnail;
        quantity = 1;
        currentItem = i;
        ResetQuantity();

        showInventoryMenu();
    }

    private void ResetQuantity()
    {
        itemLeft.gameObject.SetActive(quantity != 1);
        itemQuantity.text = quantity.ToString();
        itemTotal.text = Utils.MoneyToString(currentItem.buyPrice * quantity);
    }

    private void Buy()
    {
        if (im.money >= currentItem.buyPrice * quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                im.BuyItem(currentItem);
            }
            quantity = 1;
            PlaySound(cashClip);
            ResetQuantity();
        }
    }

    void LateUpdate()
    {
        transform.LookAt(/*gameObject.transform.position + */ cam.position);
        transform.Rotate(0f, 180f, 0f);
    }

    public void toggleDisplay()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void showMainMenu()
    {
        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
    }

    public void showInventoryMenu()
    {
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(true);
    }
}
