using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestTask._Scripts.DataPersistenceSystem
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("Data persistence config")]
        [Space(5)]
        [SerializeField] private string _path;
        [SerializeField] private string _fileName;
        
        private static DataPersistenceManager _instance;
        private List<IDataPersistence> _persistenceList;
        private GameSaveData _gameSaveData;
        private FileDataHandler _fileDataHandler;
        
        private void Awake()
        {
            if (_instance != null)
            {
                var managers = FindObjectsOfType<DataPersistenceManager>();
                
                var managersName = new StringBuilder();
                foreach (var manager in managers)
                    managersName.Append(manager.gameObject.name + ", ");
                Debug.LogError($"Only one instance of DataPersistenceManager is allowed. Names of managers: {managersName}");
            }

            _instance = this;
            
            if(string.IsNullOrEmpty(_path)) _path = Application.persistentDataPath;
        }

        private void Start()
        {
            _fileDataHandler = new FileDataHandler(_path, _fileName);
            _persistenceList = FindAllDataPersistenceObjects();
            Load();
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
        }

        private void NewGame()
        {
            _gameSaveData = new GameSaveData();
            _gameSaveData.SetBasicValue();
        }

        private void Load()
        {
            _gameSaveData = _fileDataHandler.LoadGameSaveData();
            if(_gameSaveData == null)
                NewGame();
            foreach (var persistence in _persistenceList)
            {
                persistence.Load(_gameSaveData);
            } 
        }

        private void Save()
        {
            foreach (var persistence in _persistenceList)
            {
                persistence.Save(ref _gameSaveData);
            }
            _fileDataHandler.SaveGameSaveData(_gameSaveData);
        }

        private void OnApplicationQuit()
        {
            Save();
        }
    }
}