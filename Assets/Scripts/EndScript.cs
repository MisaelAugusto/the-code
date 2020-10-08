using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class EndScript : MonoBehaviour
{
  private string end;
  private Button back;
  private Text endText, creditsText;

  // Start is called before the first frame update
  void Start()
  {
      GameObject.Find("AudioManager").GetComponent<AudioSource>().Stop();

      this.end = "Congratulations to complete all levels of 'The code', you are a legendary code cracker! I think now you're waiting for something special appears, but if you are here, the only thing special here is you, you are so smart and persistent! You have endurance! These are great qualities that everyone have got to put in practice in their lifes. The most i can do now is to advise you for everything you're doing or everything you want to do in your life: No matter how difficult or how many obstacles, if you put all your efforts into that cause you'll definitely get what you want! Don't wait to start, just do it, and the most important, don't give up!";

      this.back = GameObject.Find("Back").GetComponent<Button>();
      this.endText = GameObject.Find("End Text").GetComponent<Text>();
      this.creditsText = GameObject.Find("Credits").GetComponent<Text>();

      this.back.onClick.AddListener(this.Back);

      StartCoroutine(this.TypeText());
  }

  private void Back()
  {
      StopAllCoroutines();
      SceneManager.LoadScene("Start");
  }

  private IEnumerator TypeText()
  {
      this.creditsText.DOFade(1.0f, 2.0f);

      yield return new WaitForSeconds(1.0f);

      for (int i = 0; i < this.end.Length; i++)
      {
          this.endText.text += this.end[i].ToString();
          yield return new WaitForSeconds(0.07f);
      }

      yield return new WaitForSeconds(1.0f);

      this.back.GetComponent<Image>().DOFade(1.0f, 2.0f);
      this.back.interactable = true;
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
