using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene}
public class GameController : MonoBehaviour
{
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Camera WorldCamera;

    GameState state;

    public static GameController Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
        //ConditionsDB.Init();
    }

    private void Start()
    {
        playerMovement.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        playerMovement.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerMovement));
            }
        };

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };

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

    TrainerController trainer;

    public void StartTrainerBattle(TrainerController trainer)
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        WorldCamera.gameObject.SetActive(false);


        this.trainer = trainer; 
        var playerParty = playerMovement.GetComponent<PokemonParty>();
        //var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();
        var trainerParty = trainer.GetComponent<PokemonParty>();


        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    void EndBattle(bool won)
    {
        if (trainer != null && won == true)
        {
            trainer.BattleLost();
            trainer=null;
        }

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        WorldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerMovement.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
