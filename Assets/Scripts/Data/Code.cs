using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Code
{
  public int[] numbers;
  public int[] tipIndexes;
  public int min, max, index, times, maxTimes;

  public Code(int length, int min, int max, int[] tipIndexes, int times, int maxTimes)
  {
    this.index = 0;
    this.min = min;
    this.max = max;
    this.times = times;
    this.maxTimes = maxTimes;
    this.tipIndexes = tipIndexes;
    this.numbers = this.GenerateCode(length, min, max);
  }

  private int[] GenerateCode(int length, int min, int max)
  {
    int[] code = new int[length];
    List<int> numbers = new List<int>();

    for (int i = min; i <= max; i++)
    {
      numbers.Add(i);
    }

    int index;
    for (int i = 0; i < length; i++)
    {
      index = UnityEngine.Random.Range(0, numbers.Count);
      code[i] = numbers[index];
      numbers.RemoveAt(index);
    }

    return code;
  }

  public int[] GetNumbers()
  {
    return this.numbers;
  }

  public int GetMin()
  {
    return this.min;
  }

  public int GetMax()
  {
    return this.max;
  }

  public int[] GetTipIndexes()
  {
    return this.tipIndexes;
  }

  public int GetTimes()
  {
    return this.times;
  }

  public int GetMaxTimes()
  {
    return this.maxTimes;
  }

  public void AddTipIndex(int tipIndex)
  {
    this.tipIndexes[this.index++] = tipIndex;
  }

  public Code GetNext(int level)
  {
    int[] tips, range;
    int length, times, maxTimes;

    if (this.times == this.maxTimes)
    {
      times = 1;
      maxTimes = (level == 953) ? 47 : Mathf.Min(this.maxTimes + 2, 37);
      length = ((this.max - this.min) == 9) ? this.numbers.Length + 1 : this.numbers.Length;
    }
    else
    {
      times = this.times + 1;
      maxTimes = this.maxTimes;
      length = this.numbers.Length;
    }

    range = this.GetRange(length, times, maxTimes);
    tips = this.GenerateTipArray((int)Mathf.Floor(length / 2));

    return new Code(length, range[0], range[1], tips, times, maxTimes);
  }

  private int[] GetRange(int length, int times, int maxTimes)
  {
    int[] range = new int[2];

    int diff = (this.max - this.min);
    if (1 < times && times <= maxTimes)
    {
      range[0] = UnityEngine.Random.Range(0, 10 - diff);
      range[1] = range[0] + diff;
    }
    else
    {
      range[0] = 0;
      range[1] = (diff == 9) ? length - 1 : diff + 1;
    }

    return range;
  }

  public Code GetPrevious()
  {
    int[] tips, range;
    int length, times, maxTimes;

    if (1 < this.times && this.times <= this.maxTimes)
    {
      times = this.times - 1;
      maxTimes = this.maxTimes;
      length = this.numbers.Length;
    }
    else
    {
      maxTimes = (this.numbers.Length <= 5 && (this.max - this.min + 1) <= 8) ? this.maxTimes - 2 : 37;
      times = maxTimes;
      length = ((this.max - this.min + 1) == this.numbers.Length) ? this.numbers.Length - 1 : this.numbers.Length;
    }

    range = this.GetRangePrevious(length, times, maxTimes);
    tips = this.GenerateTipArray((int)Mathf.Floor(length / 2));

    return new Code(length, range[0], range[1], tips, times, maxTimes);
  }

  private int[] GetRangePrevious(int length, int times, int maxTimes)
  {
    int[] range = new int[2];

    int diff = (this.max - this.min);
    if (times == maxTimes)
    {
      if (this.numbers.Length == (this.max - this.min + 1))
      {
        range[0] = 0;
        range[1] = 9;
      }
      else
      {
        range[0] = UnityEngine.Random.Range(0, 10 - diff + 1);
        range[1] = range[0] + diff - 1;
      }
    }
    else if (1 < times && times < maxTimes)
    {
      range[0] = UnityEngine.Random.Range(0, 10 - diff);
      range[1] = range[0] + diff;
    }
    else
    {
      range[0] = 0;
      range[1] = diff;
    }

    return range;
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
}
