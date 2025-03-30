using UnityEngine;

[CreateAssetMenu(fileName = "NewAIBehavior", menuName = "AI/Behavior Data")]
public class AIBehaviorData : ScriptableObject
{
    [Header("General")]
    public float movementSpeed = 3.5f;

    [Header("Search Settings")]
    public float searchDuration = 3f;
    public float searchRotationSpeed = 60f;

    [Header("Combat Settings")]
    public bool isRanged = true;               // ✅ Toggle: ranged or melee
    public float attackRange = 10f;
    public float fireRate = 1f;
    public float damage = 10f;

    [Header("Burst Fire Settings")]
    public bool useBurstFire = false;          // ✅ Toggle burst shooting
    public int burstCount = 3;                 // How many shots per burst
    public float burstDelay = 0.2f;            // Delay between burst shots

    [Header("Vision Settings")]
    public float viewRadius = 15f;
    public float viewAngle = 120f;
    
    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip footstepSound;
}