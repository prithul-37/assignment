using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    Start, PlayerAction, PlayerMove, EnemyMove, Busy
}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHUD playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHUD enemyHud;
    [SerializeField] BattleDialogueBox dialogueBox;

    BattleState state;
    int currentAction;
    int currentMove;

    public event Action<bool> OnBattleOver = delegate { };

    public void StartBattle()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyUnit.Setup();
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogueBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogueBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogueBox.TypeDialog("Choose an action"));
        dialogueBox.EnableActionSelector(true);
    }


    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }

        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }

    }

    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogueBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.name}");

        //Animation player attack
        playerUnit.PlayAttactAnimation();
        yield return new WaitForSeconds(1);

        //Animaton damage
        enemyUnit.PlayHitAnimation();


        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateData();
        yield return ShowDamageDdetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogueBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} is fainted");
            enemyUnit.PlayFaintedAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver?.Invoke(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator ShowDamageDdetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical < 1f)
            yield return dialogueBox.TypeDialog("A critical Hit");
        if (damageDetails.TypeEffectiveness > 1)
            yield return dialogueBox.TypeDialog("It's super Effective");
        else if (damageDetails.TypeEffectiveness < 1)
            yield return dialogueBox.TypeDialog("It's not very Effective");
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogueBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.name}");

        //Animation
        enemyUnit.PlayAttactAnimation();
        yield return new WaitForSeconds(1);

        //Animaton damage
        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return playerHud.UpdateData();
        yield return ShowDamageDdetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogueBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} is fainted");
            playerUnit.PlayFaintedAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver?.Invoke(false);
        }
        else
        {
            PlayerAction();
        }

    }

    void HandleActionSelection()
    {

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
            {
                ++currentAction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
            {
                --currentAction;
            }


        }

        dialogueBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                PlayerMove();
            }

            else if (currentAction == 1)
            {
                //run
            }


        }
    }

    void PlayerMove()

    {
        state = BattleState.PlayerMove;
        dialogueBox.EnableActionSelector(false);
        dialogueBox.EnableDialogText(false);
        dialogueBox.EnableMoveSelector(true);
    }




    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1) ++currentMove;
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2) currentMove = 2;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)

                currentMove -= 2;
        }

        dialogueBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogueBox.EnableMoveSelector(false);
            dialogueBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}