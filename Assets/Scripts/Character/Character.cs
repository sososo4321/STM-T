using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    public float speed = 1f; // Speed of movement
    private Animator animator;
    private StateManager stateManager;

    void Start()
    {
        // Set the initial rotation to face the camera
        Vector3 cameraPosition = Camera.main.transform.position;
        transform.LookAt(new Vector3(cameraPosition.x, transform.position.y, cameraPosition.z));

        animator = GetComponent<Animator>();

        // Start the coroutine to set the 'isIdle' trigger every ten seconds
        StartCoroutine(SetIdleTrigger());
    }

    IEnumerator SetIdleTrigger()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            animator.SetTrigger("isIdle");
        }
    }
}
