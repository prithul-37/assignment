using DG.Tweening;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public enum BattleState
{
    Start, ActionSelection, MoveSelection, RunningTurn, PerformMove, Busy, PartyScreen, BattleOver,
    AboutToUse, UseItem
}
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    //[SerializeField] BattleHUD playerHud;
    [SerializeField] BattleUnit enemyUnit;
    //[SerializeField] BattleHUD enemyHud;
    [SerializeField] BattleDialogueBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;


    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;
    bool aboutToUseChoice = true;

    // These 3 are used for running. Even after speed drops the formula uses the original speed.
    int originalPlayerSpeed;
    int originalEnemySpeed;
    int runAttempts;
    MoveBase moveToLearn;

    public event Action<bool> OnBattleOver = delegate { };

    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;

    bool isTrainerBattle = false;

    PlayerMovement player;
    TrainerController trainer;

    int escapeAttempts;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerMovement>();

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerMovement>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();
        if (!isTrainerBattle)
        {
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            //playerHud.SetData(playerUnit.Pokemon);
            enemyUnit.Setup(wildPokemon);
            //enemyHud.SetData(enemyUnit.Pokemon);
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

        }
        else
        {
            //trainer Battle


            //show trainer and playersprite
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);

            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle");

            //send out first pokemon of trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyPokemon.Base.Name}");

            //send out first pokemon of player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);



        }
        partyScreen.Init();
        //dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        escapeAttempts = 0;
        partyScreen.Init();
        ActionSelection();
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }


    void MoveSelection()

    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }


    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newPokemon.Base.Name}." +
            $"Do you want to change pokemon?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }




    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }

        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMove)
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.name}");
        //Animation
        sourceUnit.PlayAttactAnimation();
        yield return new WaitForSeconds(1f);

        //Animaton damage
        targetUnit.PlayHitAnimation();


        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateData();
        yield return ShowDamageDdetails(damageDetails);
        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} is Fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
    }

    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);

            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                {
                    // Send Out next pokemon
                    StartCoroutine(AboutToUse(nextPokemon));

                }
                else
                {
                    BattleOver(true);
                    isTrainerBattle = false;
                }
            }
        }
    }


    IEnumerator ShowDamageDdetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical < 1f)
            yield return dialogBox.TypeDialog("A critical Hit");
        if (damageDetails.TypeEffectiveness > 1)
            yield return dialogBox.TypeDialog("It's super Effective");
        else if (damageDetails.TypeEffectiveness < 1)
            yield return dialogBox.TypeDialog("It's not very Effective");
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMove)
        {
            ActionSelection();
        }
    }

    void HandleActionSelection()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                MoveSelection();
            }

            else if (currentAction == 1)
            {
                //Bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                //Pokemon
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                //run
                StartCoroutine(RunTurns(BattleAction.Run));

            }
        }
    }

    IEnumerator RunTurns(BattleAction battleAction)
    {

        state = BattleState.RunningTurn;
        //BattleUnit fastestUnit;
        //BattleUnit slowestUnit;

        if (battleAction == BattleAction.Move)
        {

            ////Choose Move Phase
            //playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            //enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            //int playerMovePriority = playerUnit.Pokemon.CurrentMove.Base.Priority;
            //int enemyMovePriority = enemyUnit.Pokemon.CurrentMove.Base.Priority;

            ////Check who attacks first
            //bool playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            //if (playerMovePriority > enemyMovePriority)
            //    playerGoesFirst = true;
            //else if (playerMovePriority < enemyMovePriority)
            //    playerGoesFirst = false;

            //fastestUnit = playerGoesFirst ? playerUnit : enemyUnit;
            //slowestUnit = playerGoesFirst ? enemyUnit : playerUnit;

            //var slowestPokemon = slowestUnit.Pokemon;

            ////Execute Move Phase
            //yield return RunMove(fastestUnit, slowestUnit, fastestUnit.Pokemon.CurrentMove);

            //if (state == BattleState.BattleOver)
            //    yield break;

            //// Update the current fastest unit (even if not faster) cause the fastest one can die with a VolatileStatus and we don't want to attack it
            //if (playerGoesFirst && fastestUnit.Pokemon.HP <= 0)
            //    fastestUnit = playerUnit;
            //else if (!playerGoesFirst && fastestUnit.Pokemon.HP <= 0)
            //    fastestUnit = enemyUnit;

            //if (slowestPokemon.HP > 0)
            //{
            //    yield return RunMove(slowestUnit, fastestUnit, slowestUnit.Pokemon.CurrentMove);

            //    if (state == BattleState.BattleOver)
            //        yield break;
            //}
        }
        else
        {
            // Every action here required state to be busy because it's doing something (maybe not)
            //state = BattleState.Busy;

            //if (battleAction == BattleAction.SwitchPokemon)
            //{
            //    dialogBox.EnableActionSelector(false);
            //    var selectedPokemon = partyScreen.SelectedPokemon;
            //    yield return SwitchPokemon(selectedPokemon);

            //}
            //else if (battleAction == BattleAction.Run)
            //{
            //    dialogBox.EnableActionSelector(false);
            //    yield return TryToEscape();
            //}
            if (battleAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                yield return ThrowPokeball();
            }
            if (battleAction == BattleAction.Run)
            {
                
                yield return TryToEscape();
                dialogBox.EnableActionSelector(true);
            }
            //if (state == BattleState.BattleOver)
            //    yield break;


            if (isTrainerBattle && battleAction == BattleAction.Run)
            {
                ActionSelection();
                yield break;
            }
            // Enemy turn as this runs every time the player does something that isn't attacking
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            //yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            state = BattleState.PerformMove;

        }
    }

        void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }
    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
                return;
            }
            partyScreen.gameObject.SetActive(false);

            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));
            state = BattleState.AboutToUse;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("You have to choose a pokemon to continue");
                return;
            }


            partyScreen.gameObject.SetActive(false);
            if (prevState == BattleState.AboutToUse)
                state = BattleState.AboutToUse;

            else
                ActionSelection();
        }
    }

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice == true)
            {
                //yes option
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                //no option
                StartCoroutine(SendNextTrainerPokemon());

            }

        }
        //else if (Input.GetKeyDown(KeyCode.X))
        //{
        //    dialogBox.EnableChoiceBox(false);
        //    StartCoroutine(SendNextTrainerPokemon());

        //}





    }



    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        //playerHud.SetData(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

        if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }
        else
        {
            ActionSelection();  // Allow the player to select actions after switching
        }
    }


    IEnumerator SendNextTrainerPokemon()
    {
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextPokemon.Base.Name}!");

        StartCoroutine(EnemyMove());

    }

    //Use T for throwing pokeball
    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal trainer pokemon");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used POKEBALL!");

        // position of thwe ball
        var pokeballobj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballobj.GetComponent<SpriteRenderer>();

        // Animations oh trowiing ball
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        //Capture animation
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);

        for (int i = 0; i < Mathf.Min(shakeCount,3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if(shakeCount  == 4)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.name} was cought");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.name}has added to your party");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0,0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.name} broke free");

            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }


    }

    int TryToCatchPokemon(Pokemon pokemon)
    {
            //float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * ConditionsDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHP);
         float a = pokemon.HP/pokemon.MaxHP;

         if (a < .15f) return 4;
         else return UnityEngine.Random.Range(5,10000);

    }
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
            state = BattleState.PerformMove;
            yield break;
        }
        ++escapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;
        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox. TypeDialog($"Can't escape!");
                state = BattleState.PerformMove;
            }

        }

    }




}