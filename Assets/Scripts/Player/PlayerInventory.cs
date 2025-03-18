using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private HashSet<KeyPassType> collectedKeys = new HashSet<KeyPassType>();

    public void AddKey(KeyPassType keyType)
    {
        if (!collectedKeys.Contains(keyType))
        {
            collectedKeys.Add(keyType);
            Debug.Log($"✅ Key {keyType} collected!");
        }
    }

    public bool HasKey(KeyPassType keyType)
    {
        return collectedKeys.Contains(keyType);
    }
}
