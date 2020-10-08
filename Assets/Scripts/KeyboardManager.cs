using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class KeyboardManager : MonoBehaviour
{
  public static KeyboardManager instance;

  private Code code;
  private Level level;
  private float scrollPosition;
  private int[] numbers, attempt;
  private Button buyTip, buyHeart;
  private int inputIndex, gameState, coins;
  private Image[] codeBlocks, attemptBlocks;
  [SerializeField] private Sprite codeTipBlock;
  [SerializeField] private Sprite[] ballSprites;
  [SerializeField] private Sprite[] numberSprites;
  private RectTransform[] attemptUp, attemptDown, ballsUp, ballsDown;
  private RectTransform codePanelUp, codePanelDown, attemptsContent, background;

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(this.gameObject);
    }
    else
    {
      Destroy(this.gameObject);
    }

    SceneManager.sceneLoaded += this.Load;
  }

  private void Load(Scene scene, LoadSceneMode mode)
  {
    if (SceneManager.GetActiveScene().buildIndex == 1)
    {
      this.gameState = PlayerManager.instance.player.GetGameState();

      this.level = PlayerManager.instance.player.GetLevel();

      this.code = this.level.GetCode();

      this.numbers = this.code.GetNumbers();
      this.attempt = new int[this.numbers.Length];
      this.codeBlocks = new Image[this.numbers.Length];
      this.attemptBlocks = new Image[this.level.GetMaxTries() + 5];
      this.ballsUp = new RectTransform[this.level.GetMaxTries() + 5];
      this.attemptUp = new RectTransform[this.level.GetMaxTries() + 5];
      this.ballsDown = new RectTransform[this.level.GetMaxTries() + 5];
      this.attemptDown = new RectTransform[this.level.GetMaxTries() + 5];
      this.background = GameObject.Find("Background 2").GetComponent<RectTransform>();
      this.attemptsContent = GameObject.Find("Content").GetComponent<RectTransform>();

      this.SetInputIndex();

      this.buyTip = GameObject.Find("Buy Tip").GetComponent<Button>();
      this.buyHeart = GameObject.Find("Buy Heart").GetComponent<Button>();

      this.buyTip.onClick.AddListener(this.BuyTip);
      this.buyHeart.onClick.AddListener(this.BuyHeart);

      this.SetBuyTip();
      this.SetBuyHeart();

      GameObject.Find("0").GetComponent<Button>().onClick.AddListener(() => this.Input(0));
      GameObject.Find("1").GetComponent<Button>().onClick.AddListener(() => this.Input(1));
      GameObject.Find("2").GetComponent<Button>().onClick.AddListener(() => this.Input(2));
      GameObject.Find("3").GetComponent<Button>().onClick.AddListener(() => this.Input(3));
      GameObject.Find("4").GetComponent<Button>().onClick.AddListener(() => this.Input(4));
      GameObject.Find("5").GetComponent<Button>().onClick.AddListener(() => this.Input(5));
      GameObject.Find("6").GetComponent<Button>().onClick.AddListener(() => this.Input(6));
      GameObject.Find("7").GetComponent<Button>().onClick.AddListener(() => this.Input(7));
      GameObject.Find("8").GetComponent<Button>().onClick.AddListener(() => this.Input(8));
      GameObject.Find("9").GetComponent<Button>().onClick.AddListener(() => this.Input(9));

      GameObject.Find("Send").GetComponent<Button>().onClick.AddListener(this.Send);
      GameObject.Find("Backspace").GetComponent<Button>().onClick.AddListener(this.Backspace);

      // Code Layout
      this.codePanelUp = GameObject.Find("Code Panel Up").GetComponent<RectTransform>();
      this.codePanelDown = GameObject.Find("Code Panel Down").GetComponent<RectTransform>();

      if (this.codeBlocks.Length > 5)
      {
        this.codePanelUp.DOAnchorPosY(65.0f, 0.0f);
      }

      for (int i = 0; i < this.codeBlocks.Length; i++)
      {
        this.codeBlocks[i] = GameObject.Find("Code " + i.ToString()).GetComponent<Image>();
        RectTransform panel = (i < 5) ? this.codePanelUp : this.codePanelDown;
        GameObject.Find("Code " + i.ToString()).GetComponent<RectTransform>().SetParent(panel);

        if (this.code.GetTipIndexes().Contains(i))
        {
          this.attempt[i] = this.numbers[i];
          this.codeBlocks[i].sprite = this.codeTipBlock;
          GameObject.Find(this.numbers[i].ToString()).GetComponent<Button>().interactable = false;
          this.codeBlocks[i].GetComponentInChildren<Text>().text = this.numbers[i].ToString();
        }

        this.codeBlocks[i].GetComponent<RectTransform>().DOScale(Vector3.one, 0.25f);
      }

      for (int i = 0; i < 10; i++)
      {
        if (i < this.code.GetMin() || i > this.code.GetMax())
        {
          GameObject.Find(i.ToString()).GetComponent<Button>().interactable = false;
        }
      }

      // Attempts Layout
      int height = (this.numbers.Length <= 5) ? 60 : 120;
      for (int i = 0; i < this.attemptBlocks.Length; i++)
      {
        this.attemptBlocks[i] = GameObject.Find("Attempt " + i.ToString()).GetComponent<Image>();
        this.attemptBlocks[i].GetComponent<RectTransform>().sizeDelta = new Vector2(510.0f, height);

        this.attemptUp[i] = this.attemptBlocks[i].transform.Find("Numbers Panel Up").GetComponent<RectTransform>();
        this.ballsUp[i] = this.attemptBlocks[i].transform.Find("Balls Panel Up").GetComponent<RectTransform>();

        for (int j = Mathf.Min(this.numbers.Length, 5); j < 5; j++)
        {
          Destroy(this.attemptUp[i].Find("Number " + (j).ToString()).gameObject);
        }

        if (this.numbers.Length > 5)
        {
          this.attemptUp[i].anchoredPosition = new Vector2(0.0f, 25.0f);
          this.ballsUp[i].anchoredPosition = new Vector2(0.0f, 25.0f);

          this.attemptDown[i] = this.attemptBlocks[i].transform.Find("Numbers Panel Down").GetComponent<RectTransform>();
          this.ballsDown[i] = this.attemptBlocks[i].transform.Find("Balls Panel Down").GetComponent<RectTransform>();

          for (int j = this.numbers.Length; j < 10; j++)
          {
            Destroy(this.attemptDown[i].Find("Number " + (j).ToString()).gameObject);
          }

        }
      }

      this.scrollPosition = 0.0f;
      this.attemptsContent.sizeDelta = new Vector2(0.0f, 0.0f);

      List<int[]> attempts = this.level.GetAttempts();
      for (int i = 0; i < this.level.GetTries(); i++)
      {
        this.ShowAttempt(attempts[i], i);
      }

      if (this.gameState == 1)
      {
        this.Win();
      }
      else if (this.gameState == 2)
      {
        this.Lose();
      }
    }
  }

  private void ShowAttempt(int[] attempt, int index)
  {
    int[] temp = (this.numbers.Length <= 5) ? new int[2] {70, 4} : new int[2] {125, 2};

    if (temp[0] == 125 && index == 2)
    {
      this.attemptsContent.sizeDelta += new Vector2(0.0f, 60.0f);
      this.scrollPosition += 60.0f;
    }

    if (index > temp[1])
    {
      this.attemptsContent.sizeDelta += new Vector2(0.0f, temp[0]);
      this.scrollPosition += temp[0];
    }

    if (index == this.level.GetTries() - 1)
    {
      this.attemptsContent.DOAnchorPos(new Vector2(0.0f, this.scrollPosition), 0.5f);
    }

    for (int i = 0; i < attempt.Length; i++)
    {
      Transform panel = (i < 5) ? this.attemptUp[index] : this.attemptDown[index];
      panel.transform.Find("Number " + i.ToString()).GetComponent<Image>().sprite = this.numberSprites[attempt[i]];
      panel.transform.Find("Number " + i.ToString()).GetComponent<RectTransform>().DOScale(Vector3.one, 0.0f);
    }

    int[] balls = this.GetBalls(attempt);

    for (int k = 0; k < balls.Sum(); k++)
    {
      Sprite ball = (k < balls[0]) ? this.ballSprites[0] : this.ballSprites[1];
      Transform panel = (k < 5) ? this.ballsUp[index] : this.ballsDown[index];
      panel.transform.Find("Ball " + k.ToString()).GetComponent<Image>().sprite = ball;
      panel.transform.Find("Ball " + k.ToString()).GetComponent<RectTransform>().DOScale(Vector3.one, 0.0f);
    }

    this.attemptBlocks[index].GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f);
  }

  public void BuyTip()
  {
    if (PlayerManager.instance.player.GetCoins() >= ADSUnity.instance.GetCoinsTip() && this.level.HasTip())
    {
      this.CleanCodeBlocks();

      List<int> indexes = new List<int>();

      for (int i = 0; i < this.numbers.Length; i++)
      {
        if (!this.code.GetTipIndexes().Contains(i))
        {
          indexes.Add(i);
        }
      }

      int index = UnityEngine.Random.Range(0, indexes.Count);
      this.level.IncrementTips();
      this.code.AddTipIndex(indexes[index]);
      this.SetInputIndex();
      this.codeBlocks[indexes[index]].sprite = this.codeTipBlock;
      PlayerManager.instance.player.AddCoins(ADSUnity.instance.GetCoinsTip() * -1);
      this.codeBlocks[indexes[index]].GetComponentInChildren<Text>().text = this.numbers[indexes[index]].ToString();

      ADSUnity.instance.SetCoinsTip();
      UIManager.instance.UpdateCoinsText();
      GameObject.Find(this.numbers[indexes[index]].ToString()).GetComponent<Button>().interactable = false;
    }

    AudioManager.instance.PlayEffectSound("Buy Tip");
    StartCoroutine(this.SetBuyTipIteractable());
  }

  public void BuyHeart()
  {
    if (PlayerManager.instance.player.GetCoins() >= ADSUnity.instance.GetCoinsHeart()
        && this.level.GetExtraHearts() < 5)
    {
      this.level.IncrementExtraHearts();
      PlayerManager.instance.player.AddCoins(ADSUnity.instance.GetCoinsHeart() * -1);

      ADSUnity.instance.SetCoinsHeart();

      UIManager.instance.UpdateCoinsText();
      UIManager.instance.UpdateHeartsText();
    }

    AudioManager.instance.PlayEffectSound("Buy Heart");
    StartCoroutine(this.SetBuyHeartIteractable());
  }

  private void SetBuyTip()
  {
    this.buyTip.interactable = false;

    bool enoughCoins = PlayerManager.instance.player.GetCoins() >= ADSUnity.instance.GetCoinsTip();
    bool canBuy = PlayerManager.instance.player.GetLevel().HasTip();

    this.buyTip.interactable = (enoughCoins && canBuy);
  }

  private IEnumerator SetBuyTipIteractable()
  {
    this.buyTip.interactable = false;

    yield return new WaitForSeconds(0.2f);

    bool enoughCoins = PlayerManager.instance.player.GetCoins() >= ADSUnity.instance.GetCoinsTip();
    bool canBuy = PlayerManager.instance.player.GetLevel().HasTip();

    this.buyTip.interactable = (enoughCoins && canBuy);
  }

  private void SetBuyHeart()
  {
    this.buyHeart.interactable = false;

    bool enoughCoins = PlayerManager.instance.player.GetCoins() >= ADSUnity.instance.GetCoinsHeart();
    bool canBuy = PlayerManager.instance.player.GetLevel().GetExtraHearts() < 5;

    this.buyHeart.interactable = (enoughCoins && canBuy);
  }

  private IEnumerator SetBuyHeartIteractable()
  {
    this.buyHeart.interactable = false;

    yield return new WaitForSeconds(0.2f);

    bool enoughCoins = PlayerManager.instance.player.GetCoins() >= ADSUnity.instance.GetCoinsHeart();
    bool canBuy = PlayerManager.instance.player.GetLevel().GetExtraHearts() < 5;

    this.buyHeart.interactable = (enoughCoins && canBuy);
  }

  private void CleanCodeBlocks()
  {
    int temp = this.inputIndex;
    for (int i = 0; i < temp; i++)
    {
      this.Backspace();
    }
  }

  private void SetInputIndex()
  {
    for (int i = 0; i < this.numbers.Length; i++)
    {
      if (!this.code.GetTipIndexes().Contains(i))
      {
        this.inputIndex = i;
        break;
      }
      else
      {
        this.attempt[i] = this.numbers[i];
      }
    }
  }

  public void Input(int number)
  {
    if (this.inputIndex < this.numbers.Length)
    {
      while (this.code.GetTipIndexes().Contains(this.inputIndex))
      {
        this.attempt[this.inputIndex] = this.numbers[this.inputIndex];
        this.inputIndex++;
      }

      this.attempt[this.inputIndex] = number;
      GameObject.Find(number.ToString()).GetComponent<Button>().interactable = false;
      this.codeBlocks[this.inputIndex++].GetComponentInChildren<Text>().text = number.ToString();

      while (this.code.GetTipIndexes().Contains(this.inputIndex))
      {
        this.attempt[this.inputIndex] = this.numbers[this.inputIndex];
        this.inputIndex++;
      }
    }

    AudioManager.instance.PlayEffectSound(number.ToString());
  }

  public void Backspace()
  {
    if (this.inputIndex > 0)
    {
      this.inputIndex--;
      while (this.code.GetTipIndexes().Contains(this.inputIndex))
      {
        if (this.inputIndex > 0)
        {
          this.inputIndex--;
        }
        else
        {
          return;
        }
      }

      int number = int.Parse(this.codeBlocks[this.inputIndex].GetComponentInChildren<Text>().text);

      this.codeBlocks[this.inputIndex].GetComponentInChildren<Text>().text = "";
      GameObject.Find(number.ToString()).GetComponent<Button>().interactable = true;
    }

    AudioManager.instance.PlayEffectSound("Backspace");
  }

  private int[] GetBalls(int[] attempt)
  {
    int[] balls = new int[2];

    for (int i = 0; i < attempt.Length; i++)
    {
      if (this.numbers[i] == attempt[i])
      {
        balls[0]++;
      }
      else
      {
        if (this.numbers.Contains(attempt[i]))
        {
          balls[1]++;
        }
      }
    }

    return balls;
  }

  public void Send()
  {
    if (this.inputIndex == this.numbers.Length)
    {
      if (this.IsCorrectCode())
      {
        this.Win();
      }
      else
      {
        this.level.UpdateAttempts(this.attempt);

        if (this.level.GetHearts() == 0)
        {
          this.Lose();
        }

        this.CleanCodeBlocks();
        this.ShowAttempt(this.attempt, this.level.GetTries() - 1);
        this.attempt = new int[this.numbers.Length];
      }

      UIManager.instance.UpdateHeartsText();
    }

    AudioManager.instance.PlayEffectSound("Send");
  }

  private void Win()
  {
    this.gameState = 1;
    PlayerManager.instance.player.SetGameState(1);

    this.coins = (this.coins == 0) ? this.GetCoinsEarned() : this.coins;

    if (this.level.GetDoubleCoins())
    {
      this.coins *= 2;
    }

    this.background.DOScale(new Vector3(7.0f, 14.0f, 1.0f), 0.0f);
    AudioManager.instance.PlayEffectSound("Win Panel");
    RectTransform panel = GameObject.Find("Win Panel").GetComponent<RectTransform>();

    UIManager.instance.SetLevelImage("Win Level ");
    GameObject.Find("Code").GetComponent<Text>().text = this.NumbersToString(true);
    GameObject.Find("ADS").GetComponent<Button>().onClick.AddListener(this.OpenX2AdsPanel);
    GameObject.Find("Close").GetComponent<Button>().onClick.AddListener(this.CloseX2AdsPanel);
    GameObject.Find("Coins Earned").GetComponent<Text>().text = this.coins.ToString();
    GameObject.Find("Hearts Left").GetComponent<Text>().text = this.level.GetHearts().ToString();
    GameObject.Find("Home Blue").GetComponent<Button>().onClick.AddListener(this.HomeBlue);
    GameObject.Find("Next").GetComponent<Button>().onClick.AddListener(() => this.GenerateNextLevel(true));
    GameObject.Find("Watch").GetComponent<Button>().onClick.AddListener(ADSUnity.instance.ShowDoubleCoinADS);

    if (this.level.GetDoubleCoins())
    {
      GameObject.Find("Watch").GetComponent<Button>().interactable = false;
    }

    RectTransform adsPanel = GameObject.Find("ADS x2 Panel").GetComponent<RectTransform>();
    adsPanel.DOLocalMove(new Vector3(0.0f, -320.0f, 0.0f), 0.0f);

    panel.DOScale(Vector3.one, 0.5f);
  }

  private void Lose()
  {
    this.gameState = 2;
    PlayerManager.instance.player.SetGameState(2);

    this.background.DOScale(new Vector3(7.0f, 14.0f, 1.0f), 0.0f);
    AudioManager.instance.PlayEffectSound("Lose Panel");
    RectTransform panel = GameObject.Find("Lose Panel").GetComponent<RectTransform>();

    UIManager.instance.SetLevelImage("Lose Level ");
    GameObject.Find("Code *").GetComponent<Text>().text = this.NumbersToString(false);
    GameObject.Find("ADS2").GetComponent<Button>().onClick.AddListener(this.OpenADS2Panel);
    GameObject.Find("Close 2").GetComponent<Button>().onClick.AddListener(this.CloseADS2Panel);
    GameObject.Find("Home Red").GetComponent<Button>().onClick.AddListener(UIManager.instance.Home);
    GameObject.Find("Watch 2").GetComponent<Button>().onClick.AddListener(ADSUnity.instance.ShowTryAgainADS);
    GameObject.Find("Previous").GetComponent<Button>().onClick.AddListener(() => this.GenerateNextLevel(false));

    Text warning = GameObject.Find("Warning Text").GetComponent<Text>();
    warning.text = "Clicking on the left arrows button you return to the level           <color=#FA3C3C><size=30>" + this.GetPreviousLevel().ToString() + "</size></color>";

    if (this.level.GetTryAgain())
    {
      GameObject.Find("Watch 2").GetComponent<Button>().interactable = false;
      GameObject.Find("Again").GetComponent<RectTransform>().DOScale(Vector3.one, 0.0f);
      GameObject.Find("Previous").GetComponent<RectTransform>().DOScale(Vector3.zero, 0.0f);
      GameObject.Find("Again").GetComponent<Button>().onClick.AddListener(KeyboardManager.instance.PlayAgain);
    }

    RectTransform adsPanel = GameObject.Find("ADS Try Panel").GetComponent<RectTransform>();
    adsPanel.DOLocalMove(new Vector3(0.0f, -340.0f, 0.0f), 0.5f);

    panel.DOScale(Vector3.one, 0.5f);
  }

  public void PlayAgain()
  {
    this.gameState = 0;
    PlayerManager.instance.player.SetGameState(0);

    this.level.SetTryAgain(false);

    int[] tips = this.GenerateTipArray((int)Mathf.Floor(this.numbers.Length / 2));
    Code code = new Code(this.numbers.Length, this.code.GetMin(), this.code.GetMax(), tips, this.code.GetTimes(), this.code.GetMaxTimes());
    Level level = new Level(this.level.GetNumberLevel(), 0, code);

    PlayerManager.instance.player.SetLevel(level);

    SceneManager.LoadScene("Game");
  }

  private string NumbersToString(bool win)
  {
    string result = "";
    foreach (int number in this.numbers)
    {
      result += ((win) ? number.ToString() : "*") + " ";
    }
    return result.Trim();
  }

  private int GetPreviousLevel()
  {
    int previousLevel, level = this.level.GetNumberLevel();

    previousLevel = (level == 1) ? 1 : (level - ((int)Mathf.Floor(level / 200) + 1));

    return previousLevel;
  }

  public int GetCoinsEarned()
  {
    int tipsNotUsed = this.code.GetTipIndexes().Length - this.level.GetTips();
    int triesNotUsed = this.level.GetMaxTries() - this.level.GetTries();

    return (tipsNotUsed * 50) + (triesNotUsed * 10) + (this.level.GetNumberLevel() * 5);
  }

  private void HomeBlue()
  {
    this.coins = int.Parse(GameObject.Find("Coins Earned").GetComponent<Text>().text);
    SceneManager.LoadScene("Start");
  }
  public void OpenX2AdsPanel()
  {
    AudioManager.instance.PlayEffectSound("ADS");
    this.Pause("Mini Background");
    Text adsX2Text = GameObject.Find("ADS x2 Text").GetComponent<Text>();
    adsX2Text.text = "Do you want to watch an ads to earn more <color=#F0F050>" +
                      this.GetCoinsEarned().ToString() + "</color> coins ?";
    RectTransform adsPanel = GameObject.Find("ADS x2 Panel").GetComponent<RectTransform>();
    adsPanel.DOLocalMove(Vector3.zero, 0.5f);
    adsPanel.DOScale(Vector3.one, 0.5f);
  }

  public void CloseX2AdsPanel()
  {
    AudioManager.instance.PlayEffectSound("Close");
    this.Resume("Mini Background");
    RectTransform adsPanel = GameObject.Find("ADS x2 Panel").GetComponent<RectTransform>();
    adsPanel.DOLocalMove(new Vector3(0.0f, -320.0f, 0.0f), 0.5f);
    adsPanel.DOScale(Vector3.zero, 0.5f);
  }

  public void OpenADS2Panel()
  {
    AudioManager.instance.PlayEffectSound("ADS2");
    this.Pause("Mini Background 2");
    RectTransform adsPanel = GameObject.Find("ADS Try Panel").GetComponent<RectTransform>();
    adsPanel.DOLocalMove(Vector3.zero, 0.5f);
    adsPanel.DOScale(Vector3.one, 0.5f);
  }

  public void CloseADS2Panel()
  {
    AudioManager.instance.PlayEffectSound("Close 2");
    this.Resume("Mini Background 2");
    RectTransform adsPanel = GameObject.Find("ADS Try Panel").GetComponent<RectTransform>();
    adsPanel.DOLocalMove(new Vector3(0.0f, -340.0f, 0.0f), 0.5f);
    adsPanel.DOScale(Vector3.zero, 0.5f);
  }

  private void GenerateNextLevel(bool win)
  {
    PlayerManager.instance.player.DecrementShowADS();

    if (PlayerManager.instance.player.GetShowADS() == 0)
    {
      ADSUnity.instance.ShowADS();
      PlayerManager.instance.player.resetShowADS();
    }

    this.gameState = 0;
    this.level.SetDoubleCoins(false);
    PlayerManager.instance.player.SetGameState(0);

    this.background.DOScale(Vector3.zero, 0.0f);
    if (win)
    {
      if (this.level.GetNumberLevel() == 1000)
      {
        PlayerManager.instance.player.SetGameState(3);
        SceneManager.LoadScene("End");
        return;
      }

      Level level = new Level(this.level.GetNumberLevel() + 1, 0, this.code.GetNext(this.level.GetNumberLevel()));
      PlayerManager.instance.player.SetLevel(level);

      int coinsEarned = int.Parse(GameObject.Find("Coins Earned").GetComponent<Text>().text);
      PlayerManager.instance.player.AddCoins(coinsEarned);
      UIManager.instance.UpdateCoinsText();

      this.coins = 0;

      SceneManager.LoadScene("Game");
    }
    else
    {
      int previousLevel = this.GetPreviousLevel();
      int diff = this.level.GetNumberLevel() - previousLevel;

      if (diff == 0)
      {
        this.PlayAgain();
      }
      else
      {
        Code code = this.code.GetPrevious();

        for (int i = 0; i < (this.level.GetNumberLevel() - this.GetPreviousLevel() - 1); i++)
        {
          code = code.GetPrevious();
        }

        Level level = new Level(previousLevel, 0, code);
        PlayerManager.instance.player.SetLevel(level);

        SceneManager.LoadScene("Game");
      }
    }
  }

  private int[] GenerateTipArray(int length)
  {
    int[] tips = new int[length];
    for (int i = 0; i < length; i++)
    {
      tips[i] = -1;
    }
    return tips;
  }

  private bool IsCorrectCode()
  {
    bool result = true;

    for (int i = 0; i < this.numbers.Length; i++)
    {
      if (this.numbers[i] != this.attempt[i])
      {
        result = false;
        break;
      }
    }

    return result;
  }

  private void Pause(string gameObjectName)
  {
    GameObject.Find(gameObjectName).GetComponent<RectTransform>().DOScale(Vector3.one, 0.0f);
  }

  private void Resume(string gameObjectName)
  {
    GameObject.Find(gameObjectName).GetComponent<RectTransform>().DOScale(Vector3.zero, 0.0f);
  }
}
