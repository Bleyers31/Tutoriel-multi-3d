using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "Blaster";
    public float damage = 5f; 
    public float range = 100f;

    public float fireRate = 0f;

    public GameObject graphics;
}
