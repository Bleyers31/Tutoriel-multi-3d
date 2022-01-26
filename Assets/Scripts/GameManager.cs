using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string playerIdPrefix = "Player";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public MatchSettings matchSettings;

    public static GameManager instance;

    private void Awake() {
        if(instance == null){
            instance = this;
            return;
        }

        Debug.LogError("PLUS D'UNE INSTANCE DE GAMEMANAGER!!");
    }

    //A chaque fois qu'un joueur arrive sur le serveur, on l'enregistre dans le dictionnaire avec pour nom [Player + netId]
    public static void RegisterPlayer(string netId, Player player){
        string playerId = playerIdPrefix + netId;
        players.Add(playerId, player);
        player.transform.name = playerId;
    }

    //A chaque fois qu'un joueur se déconnecte du serveur, on le supprime du dictionnaire
    public static void UnregisterPlayer(string playerId){
        players.Remove(playerId);
    }

    public static Player GetPlayer(string playerId){
        return players[playerId];
    }
}
