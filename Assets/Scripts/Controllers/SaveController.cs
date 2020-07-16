using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private BinaryFormatter formatter;
    private string path;

    public static SaveController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        path = Application.persistentDataPath + "/gamedata.bin";
        formatter = new BinaryFormatter();
    }

    public void SaveGame(GameData data)
    {
        if (data == null)
        {
            Debug.Log("Data was null");
            return;
        }
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        Debug.Log("game saved locally");
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public GameData LoadGame()
    {
        Debug.Log("loading local file");
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            Debug.Log("game loaded locally");
            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return gameData;
        }
        else
        {
            Debug.Log("No data found so it's a new game");
            return null;
        }
    }
}