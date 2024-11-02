using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    public Character character;


    private void Start()
    {
        SetFovRotation((character.anim.DefaultDirection));
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
