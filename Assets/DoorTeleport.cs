using UnityEngine;
using System.Collections;

public class DoorTeleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [SerializeField] private Transform pointA; // Starting point (A)
    [SerializeField] private Transform pointB; // Destination point (B)
    [SerializeField] private GameObject[] exclamations;
    [SerializeField] private CameraTransition cameraTransition;

    private bool playerInRange = false; // Tracks if the player is near the door
    private GameObject player; // Reference to the player object

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(StartFading());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure only the player triggers the teleport
        {
            playerInRange = true;
            player = other.gameObject.transform.parent.gameObject;
            foreach(GameObject e in exclamations) 
                e.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
            Debug.Log("Player left the door's range.");
            foreach (GameObject e in exclamations)
                e.SetActive(false);
        }
    }

    private Transform GetNearestPoint(Transform playerTransform)
    {
        float distanceToA = Vector2.Distance(playerTransform.position, pointA.position);
        float distanceToB = Vector2.Distance(playerTransform.position, pointB.position);

        return (distanceToA < distanceToB) ? pointA : pointB;
    }

    private void Teleport(GameObject player, Vector2 destination)
    {   
        player.SetActive(false);
        player.transform.position = new Vector3(destination.x, destination.y, player.transform.position.z);
        player.SetActive(true);
        foreach (GameObject e in exclamations)
            e.SetActive(false);
        Debug.Log($"Teleported to {destination}");
    }

    IEnumerator StartFading()
    {   
        cameraTransition.FadeToBlack();
        yield return new WaitForSeconds(2f);
        Transform nearestPoint = GetNearestPoint(player.transform);
        Transform farthestPoint = (nearestPoint == pointA) ? pointB : pointA;
        yield return new WaitForSeconds(2f);
        if (farthestPoint != null)
        {
            Teleport(player, farthestPoint.position);
        }

        cameraTransition.FadeFromBlack();
    }

}
