using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class ItemData : ScriptableObject
{
    public string id;
    public GameObject prefab;

    public bool isStackable = true;
    
    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!string.IsNullOrEmpty(id)) return;
        id = name; // Assign the asset name as the default value
        UnityEditor.EditorUtility.SetDirty(this); // Make sure Unity saves it
#endif
    }
}