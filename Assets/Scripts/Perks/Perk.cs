using UnityEngine;

public class Perk : ScriptableObject
{
    public string perkName;
    public Sprite icon;
    [TextArea] public string description;

    public virtual void ApplyEffect()
    {
        Debug.Log("Applying base perk effect");
    }
}
