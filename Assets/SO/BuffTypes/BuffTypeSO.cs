using UnityEngine;

[CreateAssetMenu(fileName = "BuffTypesSO", menuName = "ScriptableObjects/BuffTypesSO")]
public class BuffTypeSO : ScriptableObject
{
    public BuffTypes buffType;
    //public int buffLength;
    //public int currentLength;
    public float buffPercentage;

    public Sprite buffSprite;

    public Sprite GetBuffSprite()
    {
        return buffSprite;
    }

    public BuffTypes GetBuffType()
    {
        return buffType;
    }

    public enum BuffTypes
    {
        Healing,
        Damaging,
        Boost,
        None
    }
}
