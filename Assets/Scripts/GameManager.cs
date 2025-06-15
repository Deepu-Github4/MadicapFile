using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameHome;

    public GameObject cardPrefab;
    public RectTransform cardGrid;
    public GridLayoutGroup gridLayout;
    public Sprite[] cardImages;

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI matchText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public GameObject winPopup;
    public TextMeshProUGUI winLevelText;
    public Image[] stars;
    public GameObject gameComplete;
    public GameObject nextButton;

    public Vector2 spacing = new Vector2(10f, 10f);
    public Vector2 padding = new Vector2(50f, 50f);

    private int rows;
    private int columns;
    private int currentLevel = 1;
    private const int maxLevel = 8;

    private List<Card> flippedCards = new List<Card>();
    private List<Card> allCards = new List<Card>();
    private bool isCheckingMatch = false;

    private int turnCount = 0;
    private int matchCount = 0;
    private int totalPairs = 0;

    public int score = 0;
    private int comboCount = 0;

    void Start()
    {
       
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        PlayerPrefs.SetInt("CurrentLevel", level);

        foreach (Transform child in cardGrid)
            Destroy(child.gameObject);

        allCards.Clear();
        flippedCards.Clear();
        turnCount = 0;
        matchCount = 0;
        score = 0;
        comboCount = 0;

        SetLevelLayout(level);
        SetupGrid();
        SpawnCards();

        if (levelText != null)
            levelText.text = "Level " + currentLevel;

        if (winPopup != null)
            winPopup.SetActive(false);

        UpdateScoreUI();
    }

    void SetLevelLayout(int level)
    {
        switch (level)
        {
            case 1: rows = 2; columns = 2; break;
            case 2: rows = 2; columns = 3; break;
            case 3: rows = 3; columns = 3; break;
            case 4: rows = 4; columns = 2; break;
            case 5: rows = 4; columns = 3; break;
            case 6: rows = 3; columns = 4; break;
            case 7: rows = 4; columns = 4; break;
            case 8: rows = 5; columns = 6; break;
            default: rows = 5; columns = 6; break;
        }
    }

    void SetupGrid()
    {
        float width = cardGrid.rect.width - padding.x * 2;
        float height = cardGrid.rect.height - padding.y * 2;

        float cellWidth = (width - (columns - 1) * spacing.x) / columns;
        float cellHeight = (height - (rows - 1) * spacing.y) / rows;
        float squareSize = Mathf.Min(cellWidth, cellHeight);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.spacing = spacing;
        gridLayout.cellSize = new Vector2(squareSize, squareSize);
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
    }

    void SpawnCards()
    {
        int totalCards = rows * columns;
        totalPairs = totalCards / 2;

        if (totalPairs > cardImages.Length)
        {
            Debug.LogError("Not enough cards");
            return;
        }

        List<Sprite> shuffled = new List<Sprite>(cardImages);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            Sprite temp = shuffled[i];
            shuffled[i] = shuffled[rand];
            shuffled[rand] = temp;
        }

        List<Sprite> spriteList = new List<Sprite>();
        for (int i = 0; i < totalPairs; i++)
        {
            spriteList.Add(shuffled[i]);
            spriteList.Add(shuffled[i]);
        }

        for (int i = 0; i < spriteList.Count; i++)
        {
            int rand = Random.Range(i, spriteList.Count);
            Sprite temp = spriteList[i];
            spriteList[i] = spriteList[rand];
            spriteList[rand] = temp;
        }

        foreach (Sprite sprite in spriteList)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardGrid);
            Card card = cardObj.GetComponent<Card>();
            card.SetCardImage(sprite);
            card.manager = this;
            card.ResetCardVisual();
            card.Flip();
            allCards.Add(card);
        }

    }

    IEnumerator InitialPreview()
    {
        float previewTime = currentLevel <= 5 ? 2f : 3f;
        yield return new WaitForSeconds(previewTime);

        foreach (Card card in allCards)
        {
            card.FlipBack();
        }
    }

    public void OnCardClicked(Card card)
    {
        if (card.isMatched || flippedCards.Contains(card)) return;

        AudioManager.instance.PlayFlip();

        card.Flip();
        flippedCards.Add(card);

        if (flippedCards.Count >= 2 && !isCheckingMatch)
        {
            StartCoroutine(CheckMatches());
        }
    }

    IEnumerator CheckMatches()
    {
        isCheckingMatch = true;

        while (flippedCards.Count >= 2)
        {
            Card card1 = flippedCards[0];
            Card card2 = flippedCards[1];

            yield return new WaitForSeconds(1f);

            turnCount++;

            if (card1.GetCardImage() == card2.GetCardImage())
            {
                card1.isMatched = true;
                card2.isMatched = true;

                HideCard(card1);
                HideCard(card2);

                matchCount++;
                comboCount++;

                int comboBonus = (comboCount - 1) * 50;
                score += 100 + comboBonus;

                AudioManager.instance.PlayMatch();

                if (matchCount >= totalPairs)
                {
                    yield return new WaitForSeconds(0.5f);
                    ShowWinPopup();
                }
            }
            else
            {
                card1.FlipBack();
                card2.FlipBack();
                comboCount = 0;

                AudioManager.instance.PlayWrong();
            }

            flippedCards.Remove(card1);
            flippedCards.Remove(card2);
            UpdateScoreUI();
        }

        isCheckingMatch = false;
    }

    void HideCard(Card card)
    {
        var cg = card.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    void UpdateScoreUI()
    {
        if (turnText != null) turnText.text = turnCount.ToString();
        if (matchText != null) matchText.text =  matchCount + "/" + totalPairs;
        if (scoreText != null) scoreText.text = score.ToString();
        if (comboText != null) comboText.text = comboCount > 1 ? $"Combo x{comboCount}!" : "";
    }

    void ShowWinPopup()
    {
        if (winPopup != null)
            winPopup.SetActive(true);

        if (winLevelText != null)
            winLevelText.text = currentLevel.ToString();

        AudioManager.instance.PlayWin();

        int perfectTurns = totalPairs;
        int difference = turnCount - perfectTurns;
        int starCount = 3;

        if (difference >= 3) starCount = 1;
        else if (difference >= 1) starCount = 2;

        for (int i = 0; i < stars.Length; i++)
            stars[i].enabled = i < starCount;

        if(currentLevel >= maxLevel)
        {
            nextButton.SetActive(false);
            gameComplete.SetActive(true);
        }
    }

    public void OnNextLevelButton()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            LoadLevel(currentLevel);
            StartCoroutine(InitialPreview());
        }
    }

    public void OnResetButton()
    {
        AudioManager.instance.ClickSound();
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
    }

        public void PlayButton()
    {
        AudioManager.instance.ClickSound();
        gameHome.SetActive(false);
        int savedLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        LoadLevel(savedLevel);
        StartCoroutine(InitialPreview());
    }

    public void HomeButton()
    {
        AudioManager.instance.ClickSound();
        gameHome.SetActive(true);
    }

    public void ExitButton()
    {
        AudioManager.instance.ClickSound();
        Application.Quit();
    }
}
