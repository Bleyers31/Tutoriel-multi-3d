using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    [SerializeField]
    private string dontDrawLayerName = "DontDraw";

    [SerializeField]
    private GameObject playerGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;
    private GameObject playerUIInstance;

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

            //Désactiver la partie graphique du joueur local (recursively = lui et tous ses enfants)
            SetlayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Création du UI du joueur local
            playerUIInstance = Instantiate(playerUIPrefab);
        }

        //Initialisation de notre personnage
        GetComponent<Player>().Setup();
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
        //On supprime l'UI du joueur
        Destroy(playerUIInstance);

        if(sceneCamera != null){
            sceneCamera.gameObject.SetActive(true);
        }

        //on supprime le joueur du dictionnaire des joueurs connectés via son nom qui est la clef du dictionnaire
        GameManager.UnregisterPlayer(transform.name);
    }
}
