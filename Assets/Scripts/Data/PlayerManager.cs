using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  public static PlayerManager instance;
  public Player player;

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

    this.player = this.Load();
  }

  public void Save()
  {
    BinaryFormatter formatter = new BinaryFormatter();

    string path = Path.Combine(Application.persistentDataPath, "player.tcd");
    FileStream stream = new FileStream(path, FileMode.Create);

    formatter.Serialize(stream, this.player);
    stream.Close();
  }

  public Player Load()
  {
    Code code = new Code(3, 0, 2, new int[1] { -1 }, 1, 1);
    Level level = new Level(1, 0, code);
    Player player = new Player(100, 50, level, true, true);

    string path = Path.Combine(Application.persistentDataPath, "player.tcd");
    if (File.Exists(path))
    {
      BinaryFormatter formatter = new BinaryFormatter();
      FileStream stream = new FileStream(path, FileMode.Open);

      player = formatter.Deserialize(stream) as Player;
      stream.Close();
    }

    return player;
  }
}
