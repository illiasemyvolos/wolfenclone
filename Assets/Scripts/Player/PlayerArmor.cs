using UnityEngine;

public class PlayerArmor : MonoBehaviour
{
    [Header("Armor Settings")]
    public int maxArmor = 50;
    public int currentArmor;

    private void Start()
    {
        currentArmor = maxArmor;
    }

    public void AddArmor(int amount)
    {
        currentArmor += amount;
        currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
    }

    public int AbsorbDamage(int damage)
    {
        if (currentArmor > 0)
        {
            int absorbedDamage = Mathf.Min(currentArmor, damage);
            currentArmor -= absorbedDamage;
            return damage - absorbedDamage; // Remaining damage to health
        }
        return damage; // No armor left to absorb damage
    }
}