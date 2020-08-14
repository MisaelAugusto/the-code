using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Level
{
  public Code code;
  public bool tryAgain, doubleCoins;
  public List<int[]> attempts;
  public int tips, level, tries, maxTries, extraHearts;

  public Level(int level, int tips, Code code)
  {
    this.tries = 0;
    this.tips = tips;
    this.code = code;
    this.level = level;
    this.SetMaxTries();
    this.SetAttempts();
    this.extraHearts = 0;
    this.tryAgain = false;
    this.doubleCoins = false;
  }

  public int GetTips()
  {
    return this.tips;
  }

  public int GetNumberLevel()
  {
    return this.level;
  }

  public Code GetCode()
  {
    return this.code;
  }

  public int GetTries()
  {
    return this.tries;
  }

  public int GetMaxTries()
  {
    return this.maxTries;
  }

  public int GetHearts()
  {
    return (this.maxTries - this.tries) + this.extraHearts;
  }

  public int GetExtraHearts()
  {
    return this.extraHearts;
  }

  public bool GetTryAgain()
  {
    return this.tryAgain;
  }

  public bool GetDoubleCoins()
  {
    return this.doubleCoins;
  }

  public void SetTryAgain(bool tryAgain)
  {
    this.tryAgain = tryAgain;
  }

  public void SetDoubleCoins(bool doubleCoins)
  {
    this.doubleCoins = doubleCoins;
  }

  private void SetMaxTries()
  {
    int length = this.code.GetNumbers().Length;
    int numbers = this.code.GetMax() - this.code.GetMin() + 1;
    int diff = numbers - length;

    this.maxTries = (length < 5) ? 5 : ((length < 8) ? 10 : 15);

    bool diffCondition = (2 <= diff) && (diff <= length);
    if (diffCondition || length == 10)
    {
      this.maxTries += 5;
    }
  }

  private void SetAttempts()
  {
    List<int[]> attempts = new List<int[]>();

    for (int i = 0; i < this.maxTries; i++)
    {
      attempts.Add(new int[this.code.GetNumbers().Length]);
    }

    this.attempts = attempts;
  }

  public List<int[]> GetAttempts()
  {
    return this.attempts;
  }

  public bool HasTip()
  {
    return this.tips < this.code.GetTipIndexes().Length;
  }

  public void IncrementTips()
  {
    this.tips++;
  }

  public void IncrementNumberLevel()
  {
    this.level++;
  }

  public void UpdateAttempts(int[] attempt)
  {
    this.attempts.Insert(this.tries, attempt);
    this.tries++;
  }

  public void IncrementExtraHearts()
  {
    this.extraHearts++;
  }
}
