using UnityEngine;

public class Perk : ScriptableObject
{
    public Sprite cardImage;
    public string perkName;
    [TextArea] public string description;

    public virtual void ApplyEffect()
    {
        Debug.Log("Applying base perk effect");
    }
}
