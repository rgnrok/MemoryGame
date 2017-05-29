using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    [SerializeField]
    private GameObject cardObject;

    [SerializeField]
    private Sprite[] cardImages;

    [SerializeField]
    private Sprite emptySprite;

    public int rows = 5;
    public int cols = 5;
    public float timeOnPair = 2f;

    public GameObject winText;
    public GameObject gameOverText;
    public GameObject carPanel;
    public Text timerText;

    private int emptySpriteId = -1;
    private int visibleCardsCount;

    private Card firstRevealedCard;
    private Card secondRevealedCard;

    private bool playOnTime;
    private int timer;
    private bool gameOver;

    private Card[] cards;

    // Use this for initialization
    void Start() {

        playOnTime = PlayerPrefs.GetInt("playOnTime") != 0;
        carPanel.GetComponent<GridLayoutGroup>().constraintCount = cols;
        cards = new Card[cols * rows];

        if (playOnTime) {
            InitTimer();
        }

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                int index = i * rows + j;
                cards[index] = Instantiate(cardObject, carPanel.transform).GetComponent<Card>();
                cards[index].OnCardClick += OnCardClick;
            }
        }
        GenerateCardImages();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Menu");
        }
    }

    protected void InitTimer() {
        timer = Mathf.CeilToInt(timeOnPair * cols * rows);
        timerText.text = timer.ToString();
        timerText.gameObject.SetActive(true);
        StartCoroutine(CheckTimerGameOver());
    }

    protected void GenerateCardImages() {
        int[] imageIds = GenerateImageIds();

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                int index = i * rows + j;
                int imageId = imageIds[index];

                cards[index].SetImage(imageId, (imageId == emptySpriteId ? emptySprite : cardImages[imageId]));
            }
        }
    }


    protected int[] GenerateImageIds() {
        int cardCount = rows * cols;
        visibleCardsCount = cardCount;

        bool withEmptyCard = cardCount % 2 == 1;
        int pairs = cardCount / 2;

        int[] imageIds = new int[cardCount];
        if (withEmptyCard) {
            imageIds[cardCount - 1] = emptySpriteId;
        }

        int availableImageCount = cardImages.Length;
        for (int i = 0, j = 0; i < pairs; i++, j = j + 2) {
            imageIds[j] = availableImageCount > i ? i : i % availableImageCount;
            imageIds[j + 1] = imageIds[j];
        }
        ShuffleArray(ref imageIds);
        return imageIds;
    }

    protected void ShuffleArray(ref int[] array) {
        for (int i = 0; i < array.Length; i++) {
            int tmp = array[i];
            int key = UnityEngine.Random.Range(i, array.Length);
            array[i] = array[key];
            array[key] = tmp;
        }
    }

    protected bool CanReveal() {
        return secondRevealedCard == null && !gameOver;
    }

    protected void CardRevealed(Card card) {
        if (firstRevealedCard == null) {
            firstRevealedCard = card;
        }
        else {
            secondRevealedCard = card;
            StartCoroutine(CheckCardMatch());
        }
    }

    protected void OnCardClick(object sender, EventArgs e) {
        Card card = sender as Card;
        if (CanReveal()) {
            card.Reveal();
            CardRevealed(card);
        }
    }

    protected IEnumerator CheckCardMatch() {

        if (firstRevealedCard.Id == secondRevealedCard.Id) {
            yield return new WaitForSeconds(1f);
            StartCoroutine(firstRevealedCard.Fade());
            StartCoroutine(secondRevealedCard.Fade());
            visibleCardsCount -= 2;

            CheckGameOver();
        }
        else {
            firstRevealedCard.Shake();
            secondRevealedCard.Shake();
            yield return new WaitForSeconds(1f);
            firstRevealedCard.UnReveal();
            secondRevealedCard.UnReveal();
        }
        firstRevealedCard = null;
        secondRevealedCard = null;
    }

    protected void CheckGameOver() {
        if (visibleCardsCount <= 1) {
            StartCoroutine(Restart(true));
        }
    }

    protected IEnumerator CheckTimerGameOver() {
        while (timer > 0 && !gameOver) {
            timer--;
            timerText.text = timer.ToString();
            yield return new WaitForSecondsRealtime(1);
        }
        if (!gameOver) {
            StartCoroutine(Restart(false));
        }
    }

    protected IEnumerator Restart(bool isWin) {
        gameOver = true;

        if (isWin) {
            carPanel.SetActive(false);
            winText.SetActive(true);
        }
        else {
            gameOverText.SetActive(true);
        }

        yield return new WaitForSeconds(3);

        winText.SetActive(false);
        gameOverText.SetActive(false);
        carPanel.SetActive(true);
        GenerateCardImages();
        gameOver = false;

        if (playOnTime) {
            InitTimer();
        }
    }

}


