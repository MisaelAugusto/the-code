using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
  public static UIManager instance;

  private Color32 color;
  private Text coinsText, heartsText;
  private bool unlockedSett, unlockedArrow;
  [SerializeField] private Sprite[] levelNumbers;
  private Button play, settings, adsButton, watch, cancelAds;
  private Button downArrow, home, historic, tip, recovery, cancelTip, cancelRecovery;
  private RectTransform buttonsSett, buttonsArrow, adsPanel, tipPanel, recoveryPanel, attemptsPanel;

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
    switch (SceneManager.GetActiveScene().buildIndex)
    {
      case 0:
        this.SetLevelImage("Level ");
        this.coinsText = GameObject.Find("Coins").GetComponent<Text>();

        this.play = GameObject.Find("Play").GetComponent<Button>();
        this.watch = GameObject.Find("Watch").GetComponent<Button>();
        this.adsButton = GameObject.Find("ADS").GetComponent<Button>();
        this.cancelAds = GameObject.Find("Cancel").GetComponent<Button>();
        this.settings = GameObject.Find("Settings").GetComponent<Button>();

        this.unlockedSett = true;
        this.buttonsSett = GameObject.Find("ButtonsSett").GetComponent<RectTransform>();

        this.adsPanel = GameObject.Find("ADS Panel").GetComponent<RectTransform>();
        this.adsPanel.DOAnchorPosX(270.0f, 0.0f);
        this.adsPanel.DOAnchorPosY(-540.0f, 0.0f);
        this.adsPanel.DOScale(Vector3.zero, 0.0f);

        // Set on click
        this.play.onClick.AddListener(this.Play);
        this.settings.onClick.AddListener(this.Settings);
        this.adsButton.onClick.AddListener(this.OpenADSPanel);
        this.cancelAds.onClick.AddListener(this.CloseADSPanel);
        this.watch.onClick.AddListener(ADSUnity.instance.ShowRewardedADS);

        GameObject.Find("Play Text").GetComponent<Text>().color = this.color;
        break;

      case 1:
        this.SetLevelImage("Level ");
        this.coinsText = GameObject.Find("Coins").GetComponent<Text>();
        this.heartsText = GameObject.Find("Hearts").GetComponent<Text>();

        this.unlockedArrow = true;
        this.tip = GameObject.Find("Tip").GetComponent<Button>();
        this.home = GameObject.Find("Home").GetComponent<Button>();
        this.recovery = GameObject.Find("Recovery").GetComponent<Button>();
        this.cancelTip = GameObject.Find("Cancel 2").GetComponent<Button>();
        this.downArrow = GameObject.Find("Down Arrow").GetComponent<Button>();
        this.cancelRecovery = GameObject.Find("Cancel").GetComponent<Button>();
        this.attemptsPanel = GameObject.Find("Attempts").GetComponent<RectTransform>();
        this.buttonsArrow = GameObject.Find("ButtonsArrow").GetComponent<RectTransform>();

        this.tipPanel = GameObject.Find("Tip Panel").GetComponent<RectTransform>();
        this.tipPanel.DOAnchorPosX(300.0f, 0.0f);
        this.tipPanel.DOAnchorPosY(353.0f, 0.0f);
        this.tipPanel.DOScale(Vector3.zero, 0.0f);

        this.recoveryPanel = GameObject.Find("Recovery Panel").GetComponent<RectTransform>();
        this.recoveryPanel.DOAnchorPosX(300.0f, 0.0f);
        this.recoveryPanel.DOAnchorPosY(456.0f, 0.0f);
        this.recoveryPanel.DOScale(Vector3.zero, 0.0f);

        // Set on click
        this.home.onClick.AddListener(this.Home);
        this.tip.onClick.AddListener(this.OpenTipPanel);
        this.downArrow.onClick.AddListener(this.DownArrow);
        this.cancelTip.onClick.AddListener(this.CloseTipPanel);
        this.recovery.onClick.AddListener(this.OpenRecoveryPanel);
        this.cancelRecovery.onClick.AddListener(this.CloseRecoveryPanel);

        break;
    }
  }

  public void SetLevelImage(string name)
  {
    Color32[] colors = new Color32[10] {new Color32(222, 184, 135, 255), new Color32(108, 104, 112, 255),
                                            new Color32(205, 127, 50, 255), new Color32(192, 192, 192, 255),
                                            new Color32(255, 215, 60, 255), new Color32(80, 200, 120, 255),
                                            new Color32(15, 82, 186, 255), new Color32(155, 17, 30, 255),
                                            new Color32(135, 206, 235, 255), new Color32(255, 203, 219, 255)};

    int level = PlayerManager.instance.player.GetLevel().GetNumberLevel();
    string levelText = level.ToString();
    this.color = (level == 1000) ? colors[9] : colors[(int)Mathf.Floor(level / 100)];

    float begin = 0.0f;
    for (int i = 1; i < levelText.Length; i++)
    {
      begin -= (int.Parse(levelText[i].ToString()) == 1) ? 30.0f : 35.0f;
    }

    for (int i = 0; i < levelText.Length; i++)
    {
      Image image = GameObject.Find(name + i.ToString()).GetComponent<Image>();
      image.sprite = this.levelNumbers[int.Parse(levelText[i].ToString())];
      image.color = color;
      image.GetComponent<RectTransform>().DOAnchorPosX(begin, 0.0f);
      image.GetComponent<RectTransform>().DOScale(Vector3.one, 0.0f);
      begin += (int.Parse(levelText[i].ToString()) == 1) ? 60.0f : 70.0f;
    }
  }

  public void Play()
  {
    if (PlayerManager.instance.player.GetGameState() == 3)
    {
      SceneManager.LoadScene("End");
    }
    else
    {
      SceneManager.LoadScene("Game");
    }
  }

  public void Settings()
  {
    if (this.unlockedSett)
    {
      this.unlockedSett = false;
      this.buttonsSett.DOAnchorPosY(0.0f, 1.0f);
      this.settings.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0.0f, 0.0f, -179.0f), 1.0f);
    }
    else
    {
      this.unlockedSett = true;
      this.buttonsSett.DOAnchorPosY(-180.0f, 1.0f);
      this.settings.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
    }

    AudioManager.instance.PlayEffectSound("Settings");
  }

  public void DownArrow()
  {
    if (this.unlockedArrow)
    {
      this.unlockedArrow = false;
      this.buttonsArrow.DOAnchorPosY(0.0f, 1.0f);
      this.downArrow.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0.0f, 0.0f, 0.0f), 1.0f);
      this.attemptsPanel.DOAnchorPos(new Vector2(-70.0f, 0.0f), 1.0f);
    }
    else
    {
      this.unlockedArrow = true;
      this.buttonsArrow.DOAnchorPosY(270.0f, 1.0f);
      this.downArrow.GetComponent<RectTransform>().DOLocalRotate(new Vector3(0.0f, 0.0f, 180.0f), 1.0f);
      this.attemptsPanel.DOAnchorPos(new Vector2(0.0f, 0.0f), 1.0f);
    }

    AudioManager.instance.PlayEffectSound("Down Arrow");
  }

  public void Home()
  {
    SceneManager.LoadScene("Start");
  }

  public void OpenADSPanel()
  {
    AudioManager.instance.PlayEffectSound("ADS");
    this.Pause();
    this.adsPanel.DOAnchorPosX(0.0f, 0.5f);
    this.adsPanel.DOAnchorPosY(0.0f, 0.5f);
    this.adsPanel.DOScale(Vector3.one, 0.5f);
  }

  public void CloseADSPanel()
  {
    AudioManager.instance.PlayEffectSound("Cancel");
    this.Resume();
    this.adsPanel.DOAnchorPosX(270.0f, 0.5f);
    this.adsPanel.DOAnchorPosY(-540.0f, 0.5f);
    this.adsPanel.DOScale(Vector3.zero, 0.5f);
  }

  public void OpenTipPanel()
  {
    AudioManager.instance.PlayEffectSound("Tip");
    this.Pause2();
    this.tipPanel.DOAnchorPosX(0.0f, 0.5f);
    this.tipPanel.DOAnchorPosY(0.0f, 0.5f);
    this.tipPanel.DOScale(Vector3.one, 0.5f);
  }

  public void CloseTipPanel()
  {
    AudioManager.instance.PlayEffectSound("Cancel 2");
    this.Resume2();
    this.tipPanel.DOAnchorPosX(300.0f, 0.5f);
    this.tipPanel.DOAnchorPosY(353.0f, 0.5f);
    this.tipPanel.DOScale(Vector3.zero, 0.5f);
  }

  public void OpenRecoveryPanel()
  {
    AudioManager.instance.PlayEffectSound("Recovery");
    this.Pause2();
    this.recoveryPanel.DOAnchorPosX(0.0f, 0.5f);
    this.recoveryPanel.DOAnchorPosY(0.0f, 0.5f);
    this.recoveryPanel.DOScale(Vector3.one, 0.5f);
  }

  public void CloseRecoveryPanel()
  {
    AudioManager.instance.PlayEffectSound("Cancel");
    this.Resume2();
    this.recoveryPanel.DOAnchorPosX(300.0f, 0.5f);
    this.recoveryPanel.DOAnchorPosY(456.0f, 0.5f);
    this.recoveryPanel.DOScale(Vector3.zero, 0.5f);
  }

  public void UpdateCoinsText()
  {
    this.coinsText.text = PlayerManager.instance.player.GetCoins().ToString();
  }

  public void UpdateHeartsText()
  {
    this.heartsText.text = PlayerManager.instance.player.GetLevel().GetHearts().ToString();
  }

  public void Pause()
  {
    GameObject.Find("Background").GetComponent<RectTransform>().DOScale(new Vector3(7.0f, 14.0f, 1.0f), 0.0f);
  }

  public void Resume()
  {
    GameObject.Find("Background").GetComponent<RectTransform>().DOScale(Vector3.zero, 0.0f);
  }

  public void Pause2()
  {
    GameObject.Find("Background 2").GetComponent<RectTransform>().DOScale(new Vector3(7.0f, 14.0f, 1.0f), 0.0f);
  }

  public void Resume2()
  {
    GameObject.Find("Background 2").GetComponent<RectTransform>().DOScale(Vector3.zero, 0.0f);
  }
}
