﻿using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

public class SaveDataManager : MonoBehaviour
{
    [SerializeField]
    private GameSaveInfo _gameSaveInfo = null;

    public delegate void SaveGameCallback();    
    public delegate void LoadGameSuccessCallback(SaveData saveData);    
    public delegate void LoadGameFailureCallback(SaveData saveData);    

    public void SaveGame(SaveGameCallback callback)
    {
        StartCoroutine(SaveGameCoroutine(callback));
    }

    public void LoadGame(LoadGameSuccessCallback successCallback, LoadGameFailureCallback failureCallback)
    {
        StartCoroutine(LoadGameCouroutine(successCallback, failureCallback));        
    }

    private SaveData CreateSaveGameData()
    {
        return new SaveData
        {
            HighestLevelUnlocked = _gameSaveInfo.HighestLevelUnlocked,
            MusicOn = _gameSaveInfo.IsMusicOn,
            SoundEffectsOn = _gameSaveInfo.IsSfxOn
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

    private IEnumerator LoadGameCouroutine(LoadGameSuccessCallback successCallback, LoadGameFailureCallback failureCallback)
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            var bf = new BinaryFormatter();
            var file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            yield return null;
            var save = (SaveData)bf.Deserialize(file);

            file.Close();
            successCallback?.Invoke(save);
        }
        else
        {
            failureCallback?.Invoke(new SaveData() { HighestLevelUnlocked = 1, MusicOn = true, SkipTutorials = false, SoundEffectsOn =true });
        }
    }    
}
