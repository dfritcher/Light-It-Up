using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField]
    private GameSaveInfo _gameSaveInfo = null;

    public delegate void SaveGameCallback();
    public event SaveGameCallback SaveGameComplete;

    public delegate void LoadGameCallback();
    public event LoadGameCallback LoadGameComplete;


    public void SaveGame(SaveGameCallback callback)
    {
        StartCoroutine(SaveGameCoroutine(callback));
    }

    public void LoadGame(LoadGameCallback callback)
    {
        StartCoroutine(LoadGameCouroutine(callback));        
    }

    private SaveData CreateSaveGameData()
    {
        return new SaveData
        {
            HighestLevelUnlocked = 1,
            MusicOn = true,
            SoundEffectsOn = true
        };
    }

    private IEnumerator SaveGameCoroutine(SaveGameCallback callback)
    {
        //Debug.Log("Save Game Coroutine Started.");
        var save = CreateSaveGameData();
        yield return null;

        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/gamesave.save");
        yield return null;
        
        bf.Serialize(file, save);
        yield return null;
        
        file.Close();
        yield return null;

        callback?.Invoke();
    }

    private IEnumerator LoadGameCouroutine(LoadGameCallback callback)
    {
        //Debug.Log("Load Game Coroutine Started");
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            //Debug.Log("Load File Found.");
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);

            var save = (SaveData)bf.Deserialize(file);

            file.Close();
            yield return null;
            LoadGameSaveInfo(save);
            yield return null;

            callback?.Invoke();
        }
        else
        {
            //Debug.Log("NO Load File Found.");
        }
    }

    private void LoadGameSaveInfo(SaveData save)
    {
        _gameSaveInfo.HighestLevelUnlocked = save.HighestLevelUnlocked;
        _gameSaveInfo.IsMusicOn = save.MusicOn;
        _gameSaveInfo.IsSfxOn = save.SoundEffectsOn;
    }
}
