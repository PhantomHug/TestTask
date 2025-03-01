using TestTask._Scripts.InventorySystem;
using UnityEditor;

namespace TestTask.Editor
{
    [CustomEditor(typeof(Inventory))]
    public class Refresher : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Repaint();
        }
    }
}