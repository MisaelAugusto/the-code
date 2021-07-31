using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ADSUnity : MonoBehaviour
{
  public static ADSUnity instance;

  private bool unlocked;
  private string gameID;
  private Text coinsADSText, coinsTipText, coinsHeartText;
  private int coinsADS, coinsTip, coinsHeart, doubleCoins;

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
    gameID = "3566525"; // ANDROID ID

    if (Advertisement.isSupported && !Advertisement.isInitialized) {
      Advertisement.Initialize(gameID, false);
    }

    this.unlocked = true;
    switch (SceneManager.GetActiveScene().buildIndex)
    {
      case 0:
        this.coinsADSText = GameObject.Find("ADS Text").GetComponent<Text>();
        this.coinsADS = PlayerManager.instance.player.GetCoinsADS();
        this.coinsADSText.text = "Do you want to watch an ads to earn <color=#F0F050>" + this.coinsADS.ToString() + "</color> coins ?";
        break;

      case 1:
        this.coinsTipText = GameObject.Find("Tip Text").GetComponent<Text>();
        this.SetCoinsTip();

        this.coinsHeartText = GameObject.Find("Heart Text").GetComponent<Text>();
        this.SetCoinsHeart();
        break;
    }
  }

  public void ShowADS()
  {
    if (Advertisement.IsReady("video"))
    {
      Advertisement.Show("video");
    }
  }

  public void ShowTryAgainADS()
  {
    if (Advertisement.IsReady("rewardedVideo") && this.unlocked)
    {
      this.unlocked = false;
      Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = TryAgainAdsAnalyze });
    }
  }

  public void TryAgainAdsAnalyze(ShowResult result)
  {
    if (result == ShowResult.Finished)
    {
      PlayerManager.instance.player.GetLevel().SetTryAgain(true);
      GameObject.Find("Watch 2").GetComponent<Button>().interactable = false;
      GameObject.Find("Again").GetComponent<RectTransform>().DOScale(Vector3.one, 0.0f);
      GameObject.Find("Previous").GetComponent<RectTransform>().DOScale(Vector3.zero, 0.0f);
      GameObject.Find("Again").GetComponent<Button>().onClick.AddListener(KeyboardManager.instance.PlayAgain);
      AudioManager.instance.PlayEffectSound("Watch 2");
    }
  }

  public void ShowDoubleCoinADS()
  {
    this.doubleCoins = KeyboardManager.instance.GetCoinsEarned();

    if (Advertisement.IsReady("rewardedVideo") && this.unlocked)
    {
      this.unlocked = false;
      Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = DoubleCoinAdsAnalyze });
    }
  }

  public void DoubleCoinAdsAnalyze(ShowResult result)
  {
    if (result == ShowResult.Finished)
    {
      PlayerManager.instance.player.GetLevel().SetDoubleCoins(true);

      GameObject.Find("Coins Earned").GetComponent<Text>().text = (this.doubleCoins * 2).ToString();

      KeyboardManager.instance.CloseX2AdsPanel();
      GameObject.Find("Watch").GetComponent<Button>().interactable = false;
      AudioManager.instance.PlayEffectSound("Watch");
    }
  }

  public void ShowRewardedADS()
  {
    if (Advertisement.IsReady("rewardedVideo") && this.unlocked)
    {
      this.unlocked = false;
      Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = AdsAnalyze });
    }
  }

  public void AdsAnalyze(ShowResult result)
  {
    if (result == ShowResult.Finished)
    {
      this.unlocked = true;
      PlayerManager.instance.player.AddCoins(this.coinsADS);
      UIManager.instance.UpdateCoinsText();

      this.SetCoinsADS();
      PlayerManager.instance.player.SetCoinsADS(this.coinsADS);
      AudioManager.instance.PlayEffectSound("Watch");
    }
  }

  public int GetCoinsTip()
  {
    return this.coinsTip;
  }

  public int GetCoinsHeart()
  {
    return this.coinsHeart;
  }

  public void SetCoinsADS()
  {
    int number = UnityEngine.Random.Range(0, 10);

    if (number < 4)
    {
      this.coinsADS = 25;
    }
    else if (number < 7)
    {
      this.coinsADS = 50;
    }
    else if (number < 9)
    {
      this.coinsADS = 75;
    }
    else
    {
      this.coinsADS = 100;
    }

    this.coinsADSText.text = this.coinsADSText.text = "Do you want to watch an ads to earn <color=#F0F050>" + this.coinsADS.ToString() + "</color> coins ?";

    PlayerManager.instance.player.SetCoinsADS(this.coinsADS);
  }

  public void SetCoinsTip()
  {
    if (PlayerManager.instance.player.GetLevel().HasTip())
    {
      int[] prices = { 200, 800, 3200, 12800, 51600};

      this.coinsTip = prices[PlayerManager.instance.player.GetLevel().GetTips()];
      this.coinsTipText.text = "Do you want to spend <color=#F0F050>" + this.coinsTip.ToString() + "</color> coins to buy a code number ?";
    }
    else
    {
      this.coinsTipText.text = "You have already purchased the maximum number of tips for that level!";
    }
  }

  public void SetCoinsHeart()
  {
    if (PlayerManager.instance.player.GetLevel().GetExtraHearts() < 5)
    {
      int[] prices = { 50, 500, 1000, 2500, 5000 };
      this.coinsHeart = prices[PlayerManager.instance.player.GetLevel().GetExtraHearts()];
      this.coinsHeartText.text = "Do you want to spend <color=#F0F050>" + this.coinsHeart + "</color> coins to buy a heart ?";
    }
    else
    {
      this.coinsHeartText.text = "You have already purchased the maximum number of hearts for that level!";
    }
  }
}
