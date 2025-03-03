using TestTask._Scripts.InventorySystem;

namespace TestTask._Scripts.DataPersistenceSystem
{
    public class GameSaveData
    {
        public int maxInventory;
        public int availableInventory;
        public Slot[] slots;

        public void SetBasicValue()
        {
            maxInventory = 30;
            availableInventory = 15;
            slots = new Slot[maxInventory];
        }
        
    }
}