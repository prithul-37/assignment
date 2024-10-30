using UnityEngine;

public enum GameState { Freeroam, Battle }
public class GameController : MonoBehaviour
{
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Camera WorldCamera;

    GameState state;


    private void Start()
    {
        playerMovement.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

    }

    private void OnDestroy()
    {
        playerMovement.OnEncountered -= StartBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        WorldCamera.gameObject.SetActive(false);

        var playerParty = playerMovement.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty,wildPokemon); 
    }

    void EndBattle(bool won)
    {
        state = GameState.Freeroam;
        battleSystem.gameObject.SetActive(false);
        WorldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.Freeroam)
        {
            playerMovement.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
    }
}
