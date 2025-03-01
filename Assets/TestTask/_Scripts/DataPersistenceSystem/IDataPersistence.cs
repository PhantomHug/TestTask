namespace TestTask._Scripts.DataPersistenceSystem
{
    public interface IDataPersistence
    {
        public void Load(GameSaveData persistence);
        public void Save(ref GameSaveData persistence);
    }
}