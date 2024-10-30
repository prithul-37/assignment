using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    private bool moving;
    private float x, y;

    public float movespeed;
    public LayerMask solidObjLayer;
    public LayerMask interactableLayer;
    public LayerMask LongGrass;

    float nextEncounter = 0;
    public float EncounterGap = 5f;
    public Light2D PlayerLight;

    public event Action OnEncountered = delegate { };


    private void Start()
    {
        nextEncounter = Time.time;
    }

    public void HandleUpdate()
    {

        if (WeatherSystem.isDay)
        {
            PlayerLight.intensity = 0;
        }
        else if (!WeatherSystem.isDay)
        {
            PlayerLight.intensity = .7f;
        }

        x = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");

        if (x != 0) y = 0;

        if (x != 0 || y != 0)
        {
            if (!moving)
            {
                moving = true;
                anim.SetBool("Moving", moving);
            }

            Move();
        }

        else
        {
            if (moving)
            {
                moving = false;
                anim.SetBool("Moving", moving);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();

    }

    void Interact()
    {
        var facingDir = new Vector3(anim.GetFloat("X"), anim.GetFloat("Y"));
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(interactPos, transform.position,Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }


    void Move()
    {
        anim.SetFloat("X", x);
        anim.SetFloat("Y", y);

        Vector2 targetPos = new Vector2(x * Time.deltaTime * movespeed,
                                        y * Time.deltaTime * movespeed);
        targetPos += new Vector2(transform.position.x, transform.position.y);

        if (Physics2D.OverlapCircle(targetPos, .2f, solidObjLayer | interactableLayer) != null)
        {
            return;
        }
        transform.Translate(x * Time.deltaTime * movespeed,
                            y * Time.deltaTime * movespeed,
                            0);

        //gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(x * movespeed,   y* movespeed);

        CheckForEncounter();

    }


    void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, .05f, LongGrass) != null)
        {
            Debug.Log("In");
            if (UnityEngine.Random.Range(1, 101) <= 20 && Time.time > nextEncounter)
            {
                nextEncounter = Time.time + EncounterGap;
                //Emplement Pokemon Attack
                Debug.Log("A wild pokemon has appeared");
                OnEncountered?.Invoke();
            }
        }
    }
}
