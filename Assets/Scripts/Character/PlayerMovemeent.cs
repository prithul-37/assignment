using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;
    [SerializeField]
    //public float movespeed;
    public Light2D PlayerLight;

    //public CharacterAnimator anim;
    public Character character;
    private bool moving;
    //private float x, y;


    float nextEncounter = 0;
    public float EncounterGap = 5f;
    private Vector2 input;

    public event Action OnEncountered = delegate { };
    public event Action<Collider2D> OnEnterTrainersView = delegate { };

    private void Start()
    {
        
        nextEncounter = Time.time;

        if (PlayerLight == null)
            Debug.LogError("PlayerLight is not assigned in the Inspector.");
    }

    //private void Awake()
    //{
    //    character = GetComponent<Character>();
    //}

    public void HandleUpdate()
    {
        if (WeatherSystem.isDay)
        {
            PlayerLight.intensity = 0;
        }
        else
        {
            PlayerLight.intensity = 0.7f;
        }

        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // remove diagonal movement
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
                
            }            
        }
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();
    }

    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    //IEnumerator Move(Vector3 targetPos)
    //{
    //    moving = true;

    //    while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
    //    {
    //        transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);
    //        yield return null;
    //    }

    //    transform.position = targetPos;
    //    moving = false;

    //    CheckForEncounter();
    //}


    //public bool IsWalkable(Vector3 targetPos)
    //{
    //    if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjLayer | interactableLayer) != null)
    //    {
    //        return false;
    //    }
    //    return true;
    //}

    void OnMoveOver()
    {
        CheckForEncounter();
        CheckIfInTrainerView();
    }

    void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.05f, GameLayers.i.GrassLayer) != null)
        {
            Debug.Log("In");
            if (UnityEngine.Random.Range(1, 101) <= 20 && Time.time > nextEncounter)
            {
                character.Animator.IsMoving = false;
                OnEncountered?.Invoke();
            }
        }
    }

    void CheckIfInTrainerView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if (collider != null)
        {
            //Debug.Log("In Trainers View");
            character.Animator.IsMoving = false;
            OnEnterTrainersView?.Invoke(collider);
        }
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
