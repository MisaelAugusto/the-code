[System.Serializable]
public class Player
{
  public Level level;
  public bool soundON, effectsON;
  public int coins, coinsADS, gameState, showADS;

  public Player(int coins, int coinsADS, Level level, bool soundON, bool effectsON)
  {
    this.gameState = 0;
    this.coins = coins;
    this.level = level;
    this.soundON = soundON;
    this.coinsADS = coinsADS;
    this.effectsON = effectsON;
    this.showADS = UnityEngine.Random.Range(2, 5);
  }

  public int GetGameState()
  {
    return this.gameState;
  }

  public int GetCoins()
  {
    return this.coins;
  }

  public int GetCoinsADS()
  {
    return this.coinsADS;
  }

  public Level GetLevel()
  {
    return this.level;
  }

  public bool GetSoundON()
  {
    return this.soundON;
  }

  public bool GetEffectsON()
  {
    return this.effectsON;
  }

  public void SetGameState(int state)
  {
    this.gameState = state;
  }

  public void AddCoins(int coins)
  {
    this.coins += coins;
  }

  public void SetCoinsADS(int coinsADS)
  {
    this.coinsADS = coinsADS;
  }

  public void SetLevel(Level level)
  {
    this.level = level;
  }

  public void SetSoundON(bool soundON)
  {
    this.soundON = soundON;
  }

  public void SetEffectsON(bool effectsON)
  {
    this.effectsON = effectsON;
  }

  public void resetShowADS()
  {
    this.showADS = UnityEngine.Random.Range(2, 5);
  }

  public void DecrementShowADS()
  {
    this.showADS--;
  }

  public int GetShowADS()
  {
    return this.showADS;
  }
}
