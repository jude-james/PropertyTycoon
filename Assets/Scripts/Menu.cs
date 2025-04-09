using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

// hide Menu from doc generation b/c all methods are private
/// @cond
public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Sprite humanImage;
    [SerializeField] private Sprite botImage;
    [SerializeField] private Sprite empty;
    [SerializeField] private Button[] playerDisplay;

    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Button start, exit, startGame, addHuman, addBot;
    [SerializeField] private TMP_InputField inputName;

    [SerializeField] private GameObject tokenChoicesObject;
    private TMP_Dropdown tokenChoices;
    private bool error;

    private static List<MenuPlayer> initPlayers;

    private void Start()
    {
        DontDestroyOnLoad(this);

        initPlayers = new List<MenuPlayer>(6);

        tokenChoices = tokenChoicesObject.GetComponent<TMP_Dropdown>();
        start.onClick.AddListener(ChangeMenu);
        exit.onClick.AddListener(ExitGame);
        startGame.onClick.AddListener(StartGame);
        addHuman.onClick.AddListener(() => Add(false));
        addBot.onClick.AddListener(() => Add(true));
        //for (int i = 0; i < 6; i++) { playerDisplay[i].onClick.AddListener(() => Remove(i)); }
        playerDisplay[0].onClick.AddListener(() => Remove(0));
        playerDisplay[1].onClick.AddListener(() => Remove(1));
        playerDisplay[2].onClick.AddListener(() => Remove(2));
        playerDisplay[3].onClick.AddListener(() => Remove(3));
        playerDisplay[4].onClick.AddListener(() => Remove(4));
        playerDisplay[5].onClick.AddListener(() => Remove(5));

        message.text = "Add players and then press start";
    }

    private void DisplayPlayers()
    {
        // Iterates through initPlayers - Changes the corresponding display images and makes them clickable
        int i = 0;
        while (i < initPlayers.Count)
        {
            playerDisplay[i].GetComponent<Button>().interactable = true;
            if (initPlayers[i].isBot)
            {
                playerDisplay[i].GetComponent<Image>().sprite = botImage;
            }
            else
            {
                playerDisplay[i].GetComponent<Image>().sprite = humanImage;
            }
            i++;
        }
        // Remaining spaces are set to blank and made unclickable
        while (i < playerDisplay.Length)
        {
            playerDisplay[i].GetComponent<Button>().interactable = false;
            playerDisplay[i].GetComponent<Image>().sprite = empty;
            i++;
        }
    }

    private void Add(bool isBot)
    {
        error = false;

        // Checks if name field is empty
        if (string.IsNullOrWhiteSpace(inputName.text))
        {
            message.text = "No name entered";
            error = true;
        }
        else
        {
            // Checks if name or token have already been used
            for (int i = 0; i < initPlayers.Count; i++)
            {
                if (initPlayers[i].name == inputName.text)
                {
                    message.text = "Name already taken";
                    error = true;
                    break;
                }
                if (initPlayers[i].token == tokenChoices.value)
                {
                    message.text = "Token already taken";
                    error = true;
                    break;
                }
            }
        }
        // If there are no errors, it adds the new Player then updates the display
        if (!error)
        {
            message.text = inputName.text + " added - Click player to remove";
            initPlayers.Add(new MenuPlayer(inputName.text, tokenChoices.value, isBot));
            DisplayPlayers();
        }
    }

    private void Remove(int index)
    {
        // When clicked, players are removed and then the display is updated
        initPlayers.RemoveAt(index);
        DisplayPlayers();
    }

    private void ChangeMenu()
    {
        playerMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    private void StartGame()
    {
        error = false;

        // Makes sure there are at least two players
        if (initPlayers.Count > 1)
        {
            // Runs a check to make sure that they are not all bots or all humans
            int count = 0;
            for (int i = 0; i < initPlayers.Count; i++)
            {
                if (initPlayers[i].isBot)
                {
                    count++;
                }
                else
                {
                    count--;
                }
            }
            if (count * Mathf.Sign(count) == initPlayers.Count)
            {
                error = true;
                message.text = "There must be at least one human and one bot";
            }
        }
        else
        {
            error = true;
            message.text = "Not enough players";
        }

        if (!error)
        {
            SceneManager.LoadScene(1);
            // Scene will be loaded, then the board will retrieve the player data to create players
        }
    }

    public static List<MenuPlayer> GetMenuPlayers()
    {
        return initPlayers;
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
/// @endcond