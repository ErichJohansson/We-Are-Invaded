using GooglePlayGames;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    public GameData gameData;
    public Action<SavedGameRequestStatus> OnSave;
    public Action<SavedGameRequestStatus> OnLoad;
    private BinaryFormatter formatter;

    public static SaveController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        formatter = new BinaryFormatter();
    }

    public void SaveGame(GameData data)
    {
        string path = Application.persistentDataPath + "/gamedata.bin";
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        Debug.Log("game saved locally");
        formatter.Serialize(stream, data);
        stream.Close();
        gameData = data;
        SaveToCloud();
    }

    public void LoadGame()
    {
        if (!GPSController.Instance.platform.IsAuthenticated())
        {
            string path = Application.persistentDataPath + "/gamedata.bin";
            if (File.Exists(path))
            {
                FileStream stream = new FileStream(path, FileMode.Open);
                Debug.Log("game loaded locally");
                gameData = formatter.Deserialize(stream) as GameData;
                stream.Close();
            }
            else
            {
                Debug.Log("No data found so it's a new game");
                gameData = null;
            }
        }
        else
            LoadFromCloud();       
    }

    public byte[] Serialize()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            formatter.Serialize(ms, gameData);
            return ms.GetBuffer();
        }
    }

    public GameData Deserialize(byte[] byteData)
    {
        using (MemoryStream ms = new MemoryStream(byteData))
        {
            if (byteData.Length == 0)
                return null;
            return (GameData)formatter.Deserialize(ms);
        }
    }

    public void SaveToCloud()
    {
        GPSController.Instance.OpenCloudSave(OnSaveResponse);
    }

    private void OnSaveResponse(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            var data = Serialize();
            SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription(DateTime.Now.ToString()).Build();
            GPSController.Instance.platform.SavedGame.CommitUpdate(meta, update, data, SaveCallback);
        }
        else
            OnSave?.Invoke(status);
    }

    private void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        OnSave?.Invoke(status);
        if (status == SavedGameRequestStatus.Success) Debug.Log("game saved to cloud");
    }

    public void LoadFromCloud()
    {
        Debug.Log("Cloud load started");
        GPSController.Instance.OpenCloudSave(OnLoadResponse);
    }

    private void OnLoadResponse(SavedGameRequestStatus status, ISavedGameMetadata meta)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Recieved some data from cloud");
            GPSController.Instance.platform.SavedGame.ReadBinaryData(meta, LoadCallback);
        }
        else
            OnLoad?.Invoke(status);
    }

    private void LoadCallback(SavedGameRequestStatus status, byte[] data)
    {
        gameData = Deserialize(data);
        Debug.Log("Cloud data deserialized");
        OnLoad?.Invoke(status);
    }
}
