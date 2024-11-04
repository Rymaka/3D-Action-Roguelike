using UnityEngine;

[CreateAssetMenu(menuName = "ItemHolder")]
public class ItemHolderSO : ScriptableObject
{
    public bool isAirBlowerActive = false;
    public float coins = 0f;
    public float exp = 0f;
    public int activatedBuffs = 0;
}