using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    private void Start() {
        if(!isLocalPlayer){
             //On désactive les composants des autres joueurs
            for (var i = 0; i < componentsToDisable.Length; i++){
                componentsToDisable[i].enabled = false;
            }

            //On définit le joueur distant comme touchable par le joueur local
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName);

        }else{
            sceneCamera = Camera.main;

            //on désactive la main camera si elle est présente
            if(sceneCamera != null){
                sceneCamera.gameObject.SetActive(false);
            }
        }

        //Initialisation de notre personnage
        GetComponent<Player>().Setup();
    }

    //Methode appellée automatiquement lorsqu'un joueur arrive sur le serveur
    //On enregistre le joueur dans le dictionnaires des joueurs connectés
    public override void OnStartClient()
    {
        base.OnStartClient();

        string netId = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netId, player);
    }


    //Appellé lors de la deconnexion d'un joueur (objet player désactivé)
    private void OnDisable() {
        if(sceneCamera != null){
            sceneCamera.gameObject.SetActive(true);
        }

        //on supprime le joueur du dictionnaire des joueurs connectés via son nom qui est la clef du dictionnaire
        GameManager.UnregisterPlayer(transform.name);
    }
}
