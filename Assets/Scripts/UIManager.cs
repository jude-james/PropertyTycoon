using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Animator dice1Animator;
    [SerializeField] private Animator dice2Animator;
    [SerializeField] private SpriteRenderer dice1;
    [SerializeField] private SpriteRenderer dice2;
    [SerializeField] private Sprite[] landedDiceFaces;
    [SerializeField] private GameObject[] playerInfoPanels;

    [SerializeField] private GameObject rollDicePanel;
    [SerializeField] private GameObject endTurnPanel;
    [SerializeField] private GameObject forSalePanel;

    public Button rollDiceButton;
    public Button endTurnButton;
    public Button buyButton;
    public Button auctionButton;
    
    private TMP_Text _rollDicePanelNameText;
    private Image _rollDicePanelImage;
    
    private TMP_Text _endTurnPanelNameText;
    private Image _endTurnPanelImage;

    private TMP_Text _forSalePanelCostText;
    private GameObject _forSalePanelTitleDeed; // TODO set properties card to this
    
    private readonly WaitForSeconds _diceRollTime = new(1.5f);

    private int _nextInfoPanel;
    
    private void Awake()
    {
        _rollDicePanelNameText = rollDicePanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _rollDicePanelImage = rollDicePanel.transform.GetChild(2).GetComponent<Image>();
        
        _endTurnPanelNameText = endTurnPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        _endTurnPanelImage = endTurnPanel.transform.GetChild(2).GetComponent<Image>();

        _forSalePanelCostText = forSalePanel.transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void ShowRollDicePrompt() => rollDicePanel.SetActive(true);
    public void HideRollDicePrompt() => rollDicePanel.SetActive(false);
    
    public void ShowEndTurnPrompt() => endTurnPanel.SetActive(true);
    public void HideEndTurnPrompt() => endTurnPanel.SetActive(false);
    
    public void ShowForSalePrompt(int cost)
    {
        _forSalePanelCostText.SetText("£" + cost);
        forSalePanel.SetActive(true);
    }

    public void HideForSalePrompt() => forSalePanel.SetActive(false);
    
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
    }

    public WaitForSeconds AnimateDiceRoll(int diceRoll1, int diceRoll2)
    {
        StartCoroutine(AnimateDiceRollCoroutine(diceRoll1, diceRoll2));
        return _diceRollTime;
    }

    private IEnumerator AnimateDiceRollCoroutine(int diceRoll1, int diceRoll2)
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
