using UnityEngine;

public class StartScript : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {
    UIManager.instance.UpdateCoinsText();
  }

  private void OnApplicationFocus()
  {
    PlayerManager.instance.Save();
  }

  private void OnApplicationQuit()
  {
    PlayerManager.instance.Save();
  }
}
