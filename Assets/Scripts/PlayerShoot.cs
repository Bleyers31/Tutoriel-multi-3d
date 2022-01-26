using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    public PlayerWeapon playerWeapon;

    private void Start() {
        if(cam == null){
            Debug.LogError("Il manque la camera sur le script PlayerShoot!");
            this.enabled = false;
        }
    }

    private void Update() {
        //Fire 1 = clic gauche
        if(Input.GetButtonDown("Fire1")){
            Shoot();
        }
    }

    //Client : action uniquement sur l'instance locale
    [Client]
    private void Shoot(){
        RaycastHit hit;

        if((Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, playerWeapon.range, mask))){
            //Si un autre joueur a été touché
            if(hit.collider.tag.Equals("Player")){
                //On indique au serveur que le joueur est touché
                CmdPlayerShoot(hit.collider.name, playerWeapon.damage);
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
