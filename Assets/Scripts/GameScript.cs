using UnityEngine;

public class GameScript : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    UIManager.instance.UpdateCoinsText();
    UIManager.instance.UpdateHeartsText();
  }

  private void OnApplicationFocus()
  {
    PlayerManager.instance.Save();
  }
}

