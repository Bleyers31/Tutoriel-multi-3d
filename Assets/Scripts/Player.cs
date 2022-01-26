using UnityEngine;
using Mirror;
using System.Collections;

public class Player : NetworkBehaviour {
    
    [SerializeField]
    private float maxhealth = 100f;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabledOnStart;

    [SyncVar]
    //cette variable est toujours actualisée sur toutes les instances
    private float currentHealth;

    private bool _isDead = false;

    //getter et setter
    public bool isDead{
        get { return _isDead;}
        protected set { _isDead = value;}
    }

    //Initialisation des paramètres du joueur
    public void Setup() {

        wasEnabledOnStart = new bool[disableOnDeath.Length];
        for (var i = 0; i < disableOnDeath.Length; i++){
            wasEnabledOnStart[i] = disableOnDeath[i].enabled;
        }

        SetDefaults();
    }

    //Initilise le joueur avec ses paramètres par défaut
    //Tous les paramètres désactivés lors de sa mort sont réactivés
    public void SetDefaults(){
        isDead = false;
        currentHealth = maxhealth;

        for (var i = 0; i < disableOnDeath.Length; i++){
            disableOnDeath[i].enabled = wasEnabledOnStart[i];
        }

        //On ne peut pas mettre de collider dans une liste de Behaviour. On traite donc à part
        Collider col = GetComponent<Collider>();
        if(col != null){
            col.enabled = true;
        }

    }

    [ClientRpc]
    //ClientRpc : Méthode lue chez tous les clients
    //Ici, tous les clients savent que le player qui possède ce script subbit des dégats
    public void RpcTakeDamage(float amount){
        //Si le joueur n'est pas mort, on réduit ses points de vie
        if(!isDead){
            currentHealth -= amount;
            Debug.Log(transform.name + " a maintenant " + currentHealth + " points de vie");

            if(currentHealth <= 0){
                Die();
            }
        }

    }

    private void Die(){
        isDead = true;

        for (var i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //On ne peut pas mettre de collider dans une liste de Behaviour. On traite donc à part
        Collider col = GetComponent<Collider>();
        if(col != null){
            col.enabled = false;
        }

        Debug.Log(transform.name + " est mort");

        //On lance le respawn en tache de fond
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn(){
        //yield return indique que la suite de la fonction sera lue une fois la valeur retournée
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
        SetDefaults();

        //On fait respawn le joueur sur un point de spawn prédéfini
        //Singleton permet d'accéder à l'instance de NetworkManager présente dans la scène
        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;
    }

    private void Update() {
        if(isLocalPlayer){
            if(Input.GetKeyDown(KeyCode.K)){
                RpcTakeDamage(50);
            }
        }
    }
}
