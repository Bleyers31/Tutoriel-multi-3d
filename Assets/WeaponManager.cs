using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon; //arme par défaut au lancement

    private PlayerWeapon currentWeapon; //arme actuelle

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;


    void Start()
    {
        EquipWeapon(primaryWeapon);
    }

    void EquipWeapon(PlayerWeapon _weapon){
        //On définit l'arme comme actuelle
        currentWeapon = _weapon;
        //On initialise les graphiques de l'arme sur le joueur
        GameObject weaponIns = Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        if(isLocalPlayer){
            //On applique le layer des armes
            weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
            SetlayerRecursively(weaponIns, weaponIns.layer);
        }
    }

    public PlayerWeapon GetCurrentWeapon(){
        return currentWeapon;
    }

    //La méthode fait appel à elle même pour chaque enfant de chaque objet, ainsi tout est fait avec une seule boucle
    private void SetlayerRecursively(GameObject obj, int newLayer){
        //On attribue le layer souhaité
        obj.layer = newLayer;

        //Si le GameObject possède des enfants, on leur applique également récursivement
        foreach (Transform child in obj.transform){
           SetlayerRecursively(child.gameObject, newLayer);
        }
    }
}
