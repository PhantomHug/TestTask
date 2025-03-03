using System;
using System.IO;
using UnityEngine;

namespace TestTask._Scripts.DataPersistenceSystem
{
    public class FileDataHandler
    {
        private readonly string _dirPath;
        private readonly string _fileName;
        
        private string path => Path.Combine(_dirPath, _fileName);
        
        public FileDataHandler(string dirPath, string fileName)
        {
            _dirPath = dirPath;
            _fileName = fileName;
        }

        public GameSaveData LoadGameSaveData()
        {
            GameSaveData gameSaveData = null;
            if (File.Exists(path))
            {
                try
                {
                    string json = File.ReadAllText(path);
                    gameSaveData = JsonUtility.FromJson<GameSaveData>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Debug.Log("Stop playing and start again. IT WILL REFRESH SAVED DATA TO BASIC");
                }
            }
            return gameSaveData;
        }

        public void SaveGameSaveData(GameSaveData gameSaveData)
        {
            try
            {
                string jsonData = JsonUtility.ToJson(gameSaveData);
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}