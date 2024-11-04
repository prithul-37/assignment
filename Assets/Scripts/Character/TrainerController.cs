using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    public Character character;

    //state
    bool battleLost = false;


    private void Start()
    {
        SetFovRotation((character.anim.DefaultDirection));
    }
    public void Interact(Transform initiator)
    {
        //throw new System.NotImplementedException();
        character.LookTowards(initiator.position);

        //show dialog
        if (!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                //Debug.Log("Starting Trainer BAttle");
                GameController.Instance.StartTrainerBattle(this);

            }));
        }
        else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
        }

    }

    public IEnumerator TriggerTrainerBattle(PlayerMovement player)
    {
        //show exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        //walk toward the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        //show dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog,() =>
        {
            //Debug.Log("Starting Trainer BAttle");
            GameController.Instance.StartTrainerBattle(this);

        }));


    }

    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }


    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    

    public string Name
    {
        get => name;
    }
public Sprite Sprite
    {
        get => sprite; 
    }

}
