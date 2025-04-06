using System;
using System.Collections;
using Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manages the UI elements and interactions in the game.
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [Header("Dice")]
    [SerializeField] private Animator dice1Animator;
    [SerializeField] private Animator dice2Animator;
    [SerializeField] private SpriteRenderer dice1;
    [SerializeField] private SpriteRenderer dice2;
    [SerializeField] private Sprite[] landedDiceFaces;
    
    [Header("Info UI")]
    [SerializeField] public GameObject[] playerInfoPanels;

    [field: SerializeField] public GameObject BankInfoPanel { get; private set; }
    [field: SerializeField] public GameObject FreeParkingInfoPanel { get; private set; }
    
    [Header("Game Prompts")]
    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] private GameObject endTurnPanel;
    [SerializeField] private GameObject forSalePanel;
    [SerializeField] private GameObject auctionPanel;
    [SerializeField] private GameObject inJailPanel;
    [SerializeField] private GameObject mortgagePanel;
    [SerializeField] private GameObject unmortgagePanel;
    
    [Header("Game Popups")]
    [SerializeField] private GameObject goToJailPanel;
    [SerializeField] private GameObject payRentPanel;
    [SerializeField] private GameObject botDecisionDialogPanel;
    
    [Header("Buttons")]
    public Button rollDiceButton;
    public Button endTurnButton;
    
    public Button buyButton;
    public Button auctionButton;

    public Button bidButton;
    public Button foldButton;
    
    public Button postBailButton;
    public Button getOutOfJailFreeButton;
    public Button remainInJailButton;

    public Button mortgageButton;
    public Button unmortgageButton;

    public Button endMortgageButton;
    public Button endUnmortgageButton;
    
    private TMP_Text _rollDicePanelNameText;
    private Image _rollDicePanelImage;
    
    private TMP_Text _endTurnPanelNameText;
    private Image _endTurnPanelImage;

    private TMP_Text _forSalePanelCostText;
    private GameObject _forSalePanelCard;
    private Transform _forSalePanelCardPlaceholder;

    private TMP_Text _auctionPanelCostText;
    private TMP_Text _auctionPanelNameText;
    private GameObject _auctionPanelCard;
    private Transform _auctionPanelCardPlaceholder;
    private Image _auctionPanelImage;
    private TMP_Text _auctionPanelPriceText;
    
    private TMP_Text _goToJailPanelNameText;

    private TMP_Text _inJailPanelNameText;
    private Image _inJailPanelImage;

    private TMP_Text _payRentPanelRentText;
    private TMP_Text _payRentPanelPlayerNameText;
    private Image _payRentPanelPlayerImage;
    private TMP_Text _payRentPanelOwnedByNameText;
    private Image _payRentPanelOwnedByImage;

    private TMP_Text _mortgagePanelNameText;
    private Image _mortgagePanelImage;
    
    private TMP_Text _unmortgagePanelNameText;
    private Image _unmortgagePanelImage;
    
    private readonly WaitForSeconds _diceRollTime = new(1.5f);
    private const float MoneyChangeDuration = 1.5f;

    private int _nextInfoPanel;
    
    private void Awake()
    {
        _rollDicePanelNameText = rollDicePanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _rollDicePanelImage = rollDicePanel.transform.GetChild(2).GetComponent<Image>();
        
        _endTurnPanelNameText = endTurnPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _endTurnPanelImage = endTurnPanel.transform.GetChild(2).GetComponent<Image>();

        _forSalePanelCostText = forSalePanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _forSalePanelCardPlaceholder = forSalePanel.transform.GetChild(4).transform;
        
        _auctionPanelCostText = auctionPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _auctionPanelNameText = auctionPanel.transform.GetChild(3).GetComponent<TMP_Text>();
        _auctionPanelCardPlaceholder = auctionPanel.transform.GetChild(2).transform;
        _auctionPanelImage = auctionPanel.transform.GetChild(4).GetComponent<Image>();
        _auctionPanelPriceText = auctionPanel.transform.GetChild(6).GetComponent<TMP_Text>();
        
        _goToJailPanelNameText = goToJailPanel.transform.GetChild(0).GetComponent<TMP_Text>();

        _inJailPanelNameText = inJailPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _inJailPanelImage = inJailPanel.transform.GetChild(2).GetComponent<Image>();

        _payRentPanelRentText = payRentPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _payRentPanelPlayerNameText = payRentPanel.transform.GetChild(2).GetComponent<TMP_Text>();
        _payRentPanelPlayerImage = payRentPanel.transform.GetChild(3).GetComponent<Image>();
        _payRentPanelOwnedByNameText = payRentPanel.transform.GetChild(5).GetComponent<TMP_Text>();
        _payRentPanelOwnedByImage = payRentPanel.transform.GetChild(6).GetComponent<Image>();

        _mortgagePanelNameText = mortgagePanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _mortgagePanelImage = mortgagePanel.transform.GetChild(2).GetComponent<Image>();
        
        _unmortgagePanelNameText = unmortgagePanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _unmortgagePanelImage = unmortgagePanel.transform.GetChild(2).GetComponent<Image>();
    }

    public void ShowRollDicePrompt() => rollDicePanel.SetActive(true);
    public void HideRollDicePrompt() => rollDicePanel.SetActive(false);
    
    public void ShowEndTurnPrompt() => endTurnPanel.SetActive(true);
    public void HideEndTurnPrompt() => endTurnPanel.SetActive(false);
    
    public void ShowForSalePrompt(bool buyButtonEnabled, bool auctionButtonEnabled, Property property)
    {
        buyButton.interactable = buyButtonEnabled;
        auctionButton.interactable = auctionButtonEnabled;
        
        _forSalePanelCostText.SetText("£" + property.Cost);
        _forSalePanelCard = Instantiate(property.Card.transform.GetChild(0).gameObject, _forSalePanelCardPlaceholder);
        
        forSalePanel.SetActive(true);
    }
    public void HideForSalePrompt()
    {
        Destroy(_forSalePanelCard);
        forSalePanel.SetActive(false);
    }

    public void ShowAuctionPrompt(Property property)
    {
        _auctionPanelCostText.SetText("£" + property.Cost);
        _auctionPanelCard = Instantiate(property.Card.transform.GetChild(0).gameObject, _auctionPanelCardPlaceholder);
        auctionPanel.SetActive(true);
    }
    
    public void HideAuctionPrompt()
    {
        Destroy(_auctionPanelCard);
        auctionPanel.SetActive(false);
    }
    
    public void UpdateAuctionPrompt(bool bidButtonEnabled, bool foldButtonEnabled, string bidderName, Sprite bidderSprite)
    {
        bidButton.interactable = bidButtonEnabled;
        foldButton.interactable = foldButtonEnabled;
        _auctionPanelNameText.SetText(bidderName);
        _auctionPanelImage.sprite = bidderSprite;
    }
    
    public void DisableAuctionButtons()
    {
        bidButton.interactable = false;
        foldButton.interactable = false;
    }

    public void UpdateAuctionPrice(int newValue)
    {
        _auctionPanelPriceText.SetText("£"+newValue);
    }

    public void UpdateBidButtonAmount(int auctionPrice, int bidAmount)
    {
        // TODO store Text variable
        bidButton.GetComponentInChildren<TMP_Text>().SetText("BID £" + (auctionPrice + bidAmount) + "\n(+£" + bidAmount + ")");
    }
    
    public void ShowGoToJailPopup() => goToJailPanel.SetActive(true);
    public void HideGoToJailPopup() => goToJailPanel.SetActive(false);

    public void ShowInJailPrompt(bool postBailButtonEnabled, bool getOutOfJailFreeButtonEnabled, bool remainInJailButtonEnabled)
    {
        postBailButton.interactable = postBailButtonEnabled;
        getOutOfJailFreeButton.interactable = getOutOfJailFreeButtonEnabled;
        remainInJailButton.interactable = remainInJailButtonEnabled;
        inJailPanel.SetActive(true);
    }
    public void HideInJailPrompt() => inJailPanel.SetActive(false);

    public void ShowPayRentPopup(int rent, string ownedByName, Sprite ownedBySprite)
    {
        _payRentPanelRentText.SetText("£" + rent);
        _payRentPanelOwnedByNameText.SetText(ownedByName);
        _payRentPanelOwnedByImage.sprite = ownedBySprite;
        
        payRentPanel.SetActive(true);
    }
    public void HidePayRentPopup() => payRentPanel.SetActive(false);

    public void ShowMortgagePanel() => mortgagePanel.SetActive(true);
    public void HideMortgagePanel() => mortgagePanel.SetActive(false);
    
    public void ShowUnmortgagePanel() => unmortgagePanel.SetActive(true);
    public void HideUnmortgagePanel() => unmortgagePanel.SetActive(false);
    
    public void EnableMortgageButton() => mortgageButton.interactable = true;
    public void DisableMortgageButton() => mortgageButton.interactable = false;
    
    public void EnableUnmortgageButton() => unmortgageButton.interactable = true;
    public void DisableUnmortgageButton() => unmortgageButton.interactable = false;
    
    public void ShowBotDecisionDialog() => botDecisionDialogPanel.SetActive(true);
    public void HideBotDecisionDialog() => botDecisionDialogPanel.SetActive(false);

    /// <summary>
    /// Returns the next available UI panel and makes it visible
    /// </summary>
    /// <returns> The UI Panel component </returns>
    public GameObject GetInfoPanel()
    {
        var infoPanel = playerInfoPanels[_nextInfoPanel];
        infoPanel.SetActive(true);
        _nextInfoPanel++;
        return infoPanel;
    }
    
    /// <summary>
    /// Sets the active player's information in the UI.
    /// </summary>
    /// <param name="name">The name of the active player.</param>
    /// <param name="sprite">The sprite representing the player.</param>
    public void SetActivePlayerInfo(string name, Sprite sprite)
    {
        _rollDicePanelNameText.SetText(name);
        _rollDicePanelImage.sprite = sprite;
        _endTurnPanelNameText.SetText(name);
        _endTurnPanelImage.sprite = sprite;

        _goToJailPanelNameText.SetText(name);
        
        _inJailPanelNameText.SetText(name);
        _inJailPanelImage.sprite = sprite;
        
        _payRentPanelPlayerNameText.SetText(name);
        _payRentPanelPlayerImage.sprite = sprite;
        
        _mortgagePanelNameText.SetText(name);
        _mortgagePanelImage.sprite = sprite;
        
        _unmortgagePanelNameText.SetText(name);
        _unmortgagePanelImage.sprite = sprite;
    }

    /// <summary>
    /// Animates the dice roll and sets the final sprite to the dice roll values
    /// </summary>
    /// <param name="diceRoll1">The first dice roll result.</param>
    /// <param name="diceRoll2">The second dice roll result.</param>
    public IEnumerator AnimateDiceRoll(int diceRoll1, int diceRoll2)
    {
        dice1Animator.enabled = true;
        dice2Animator.enabled = true;
        yield return _diceRollTime;
        dice1Animator.enabled = false;
        dice2Animator.enabled = false;
        dice1.sprite = landedDiceFaces[diceRoll1-1];
        dice2.sprite = landedDiceFaces[diceRoll2-1];
    }

    /// @cond
    public void AnimateMoney(TMP_Text money, int newValue)
    {
        StartCoroutine(AnimateMoneyCoroutine(money, newValue));
    }
    /// @endcond
    
    /// <summary>
    /// Interpolates the money text value to the newValue, for the duration
    /// </summary>
    /// <param name="money"> The TMP Text component </param>
    /// <param name="newValue"> The integer value to set the TMP Text component to </param>
    /// <returns></returns>
    private IEnumerator AnimateMoneyCoroutine(TMP_Text money, int newValue)
    {
        var currentValue = int.Parse(money.text.Substring(1, money.text.Length - 1));

        float elapsedTime = 0;
        while (elapsedTime < MoneyChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            money.SetText("£" + Math.Round(Mathf.Lerp(currentValue, newValue, elapsedTime / MoneyChangeDuration)));
            yield return null;
        }
    }

    /// <summary>
    /// Loops through the list of title deeds and changes the opacity of the UI
    /// depending on if the player/bank owns the title deed
    /// </summary>
    /// <param name="titleDeeds"> The title deeds the player/bank owns </param>
    /// <param name="infoPanel"> The player/bank info panel to update </param>
    public void UpdateTitleDeedUI(Property[] titleDeeds, GameObject infoPanel)
    {
        const float unownedAlpha = 0.4f;
        const float ownedAlpha = 1f;

        var miniTitleDeeds = infoPanel.transform.GetChild(3).gameObject;

        for (var i = 0; i < titleDeeds.Length; i++)
        {
            var image = miniTitleDeeds.transform.GetChild(i).GetComponent<Image>();

            var alpha = titleDeeds[i] == null ? unownedAlpha : ownedAlpha;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        }
    }

    public void OnMainMenuClick()
    {
        SceneManager.LoadScene(0);
    }
}
