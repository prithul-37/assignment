using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] Character character;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;


    NPCState state;  
    float idleTimer = 0f;
    int currentPattern = 0;

    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);

            StartCoroutine(DialogManager.Instance.ShowDialog(dialog,() =>
            {
                idleTimer = 0f;
                state =NPCState.Idle;
            }));
            //StartCoroutine(character.Move(new Vector2(0, 2)));
        } 
    }

    private void Update()
    {
        //if (DialogManager.Instance.IsShowing) 
        //{
        //    return;
        //}

        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if(movementPattern.Count > 0)
                    StartCoroutine(Walk());

            }
        }


        character.HandleUpdate();
    }

    IEnumerator Walk()
    {
        state = NPCState.walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);
        if (transform.position != oldPos) {
            currentPattern = (currentPattern + 1) % movementPattern.Count;
        }

        state = NPCState.Idle;
    }

}
public enum NPCState { Idle, walking, Dialog}


