using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public string weaponName;
    [SerializeField] protected float baseBodyDamage;
    [SerializeField] protected float baseHeadDamage;
    
    public float CurrentBodyDamage { get; protected set; }
    public float CurrentHeadDamage { get; protected set; }

    protected virtual void Awake()
    {
        CurrentBodyDamage = baseBodyDamage;
        CurrentHeadDamage = baseHeadDamage;
    }

    public virtual void ApplyDamageMultiplier(float multiplier)
    {
        CurrentBodyDamage = baseBodyDamage * multiplier;
        CurrentHeadDamage = baseHeadDamage * multiplier;
        Debug.Log($"{weaponName} damage boosted! Body: {CurrentBodyDamage}, Head: {CurrentHeadDamage}");
    }

    public abstract void ProcessHit(RaycastHit hit, bool isHeavyAttack = false);
}