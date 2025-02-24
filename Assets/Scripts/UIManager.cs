using System.Collections;
using Tiles;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Dice")]
    [SerializeField] private Animator dice1Animator;
    [SerializeField] private Animator dice2Animator;
    [SerializeField] private SpriteRenderer dice1;
    [SerializeField] private SpriteRenderer dice2;
    [SerializeField] private Sprite[] landedDiceFaces;
    
    [Header("Player Info")]
    [SerializeField] private GameObject[] playerInfoPanels;

    [Header("Game Prompts")]
    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] private GameObject endTurnPanel;
    [SerializeField] private GameObject forSalePanel;
    [SerializeField] private GameObject inJailPanel;
    
    [Header("Game Popups")]
    [SerializeField] private GameObject goToJailPanel;
    
    [Header("Buttons")]
    public Button rollDiceButton;
    public Button endTurnButton;
    
    public Button buyButton;
    public Button auctionButton;
    
    public Button postBailButton;
    public Button getOutOfJailFreeButton;
    public Button remainInJailButton;
    
    private TMP_Text _rollDicePanelNameText;
    private Image _rollDicePanelImage;
    
    private TMP_Text _endTurnPanelNameText;
    private Image _endTurnPanelImage;

    private TMP_Text _forSalePanelCostText;
    private GameObject _forSalePanelCard;
    private Transform _forSalePanelCardPlaceholder;

    private TMP_Text _goToJailPanelNameText;

    private TMP_Text _inJailPanelNameText;
    private Image _inJailPanelImage;
    
    private readonly WaitForSeconds _diceRollTime = new(1.5f);

    private int _nextInfoPanel;
    
    private void Awake()
    {
        _rollDicePanelNameText = rollDicePanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _rollDicePanelImage = rollDicePanel.transform.GetChild(2).GetComponent<Image>();
        
        _endTurnPanelNameText = endTurnPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _endTurnPanelImage = endTurnPanel.transform.GetChild(2).GetComponent<Image>();

        _forSalePanelCostText = forSalePanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _forSalePanelCardPlaceholder = forSalePanel.transform.GetChild(4).transform;
        
        _goToJailPanelNameText = goToJailPanel.transform.GetChild(0).GetComponent<TMP_Text>();

        _inJailPanelNameText = inJailPanel.transform.GetChild(1).GetComponent<TMP_Text>();
        _inJailPanelImage = inJailPanel.transform.GetChild(2).GetComponent<Image>();
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
    
    public GameObject GetInfoPanel()
    {
        var infoPanel = playerInfoPanels[_nextInfoPanel];
        infoPanel.SetActive(true);
        _nextInfoPanel++;
        return infoPanel;
    }
    
    public void SetActivePlayerInfo(string name, Sprite sprite)
    {
        _rollDicePanelNameText.SetText(name);
        _rollDicePanelImage.sprite = sprite;
        _endTurnPanelNameText.SetText(name);
        _endTurnPanelImage.sprite = sprite;

        _goToJailPanelNameText.SetText(name);
        
        _inJailPanelNameText.SetText(name);
        _inJailPanelImage.sprite = sprite;
    }

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
}
