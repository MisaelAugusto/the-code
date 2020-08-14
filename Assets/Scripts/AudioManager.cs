using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public static AudioManager instance;

  private Button sound, effects;
  private bool soundON, effectsON;
  [SerializeField] private AudioSource mainSound;
  [SerializeField] private Sprite[] images = new Sprite[4];

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
    if (SceneManager.GetActiveScene().buildIndex == 0)
    {
      this.mainSound = GameObject.Find("AudioManager").GetComponent<AudioSource>();

      this.sound = GameObject.Find("Sound").GetComponent<Button>();
      this.effects = GameObject.Find("Effects").GetComponent<Button>();

      this.sound.onClick.AddListener(this.Sound);
      this.effects.onClick.AddListener(this.Effects);

      this.SetSoundON(PlayerManager.instance.player.GetSoundON());
      this.SetEffectsON(PlayerManager.instance.player.GetEffectsON());
    }
  }

  public void PlayEffectSound(string audioSourceName)
  {
    if (this.effectsON)
    {
      GameObject.Find(audioSourceName).GetComponent<AudioSource>().Play();
    }
  }

  public void SetSoundON(bool soundON)
  {
    this.soundON = soundON;
    if (this.soundON)
    {
      this.mainSound.Play();
      this.sound.GetComponent<Image>().sprite = this.images[0];

    }
    else
    {
      this.mainSound.Stop();
      this.sound.GetComponent<Image>().sprite = this.images[1];
    }

  }

  public void SetEffectsON(bool effectsON)
  {
    this.effectsON = effectsON;
    this.effects.GetComponent<Image>().sprite = (this.effectsON) ? this.images[2] : this.images[3];
  }

  public void Sound()
  {
    this.PlayEffectSound("Sound");

    this.soundON = !this.soundON;
    if (this.soundON)
    {
      this.mainSound.Play();
      this.sound.GetComponent<Image>().sprite = this.images[0];

    }
    else
    {
      this.mainSound.Stop();
      this.sound.GetComponent<Image>().sprite = this.images[1];
    }

    PlayerManager.instance.player.SetSoundON(this.soundON);
  }

  public void Effects()
  {
    this.PlayEffectSound("Effects");
    this.effectsON = !this.effectsON;
    this.effects.GetComponent<Image>().sprite = (this.effectsON) ? this.images[2] : this.images[3];

    PlayerManager.instance.player.SetEffectsON(this.effectsON);
  }
}
