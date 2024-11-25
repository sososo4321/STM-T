using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmINearSpheres : MonoBehaviour
{
    public Color myColor;
    public GameObject stateManager;

    private Vector3 myOriginalPosition;
    public AudioClip grabSound;
    public AudioClip meetObjectSound;
    private AudioSource objectSource;




    // Start is called before the first frame update
    void Start()
    {
        objectSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        objectSource.PlayOneShot(meetObjectSound);
    }

    public void MyPosition()
    {
        myOriginalPosition = transform.position;
        objectSource.PlayOneShot(grabSound);
    }
    public void GoBack()
    {
        transform.position = myOriginalPosition;
        objectSource.PlayOneShot(grabSound);
    }
}
