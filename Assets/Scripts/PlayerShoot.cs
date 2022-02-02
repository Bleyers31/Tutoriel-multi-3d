using UnityEngine;
using Mirror;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;


    private void Start() {

        weaponManager = GetComponent<WeaponManager>();

        if(cam == null){
            Debug.LogError("Il manque la camera sur le script PlayerShoot!");
            this.enabled = false;
        }
    }

    private void Update() {

        currentWeapon = weaponManager.GetCurrentWeapon();

        //Fire 1 = clic gauche
        if(currentWeapon.fireRate <= 0f){
            if(Input.GetButtonDown("Fire1")){
                Shoot();
            }
        }else{
             if(Input.GetButtonDown("Fire1")){
                //Appelle une méthode de façon répétée selon l'intervale défini
                InvokeRepeating("Shoot", 0f, 1f/currentWeapon.fireRate);
            }else if(Input.GetButtonUp("Fire1")){
                //Permet d'annuler l'appel répété précédent
                CancelInvoke("Shoot");
            }
        }
       
    }

    //Client : action uniquement sur l'instance locale
    [Client]
    private void Shoot(){
        RaycastHit hit;
        Debug.Log("TIR!");
        if((Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))){
            //Si un autre joueur a été touché
            if(hit.collider.tag.Equals("Player")){
                //On indique au serveur que le joueur est touché
                CmdPlayerShoot(hit.collider.name, currentWeapon.damage);
            }
        }
    }

    //Command : Le serveur traite la demande de son côté
    [Command]
    private void CmdPlayerShoot(string playerId, float damage){
        Debug.Log(playerId + " a été touché!");

        //Récupération du joueur via son id dans le dictionnaire des joueurs connectés au serveur
        Player player = GameManager.GetPlayer(playerId);
        player.RpcTakeDamage(damage);
    }
}
