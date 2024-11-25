using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTheObjects : MonoBehaviour
{
    public GameObject gameObject;

    public void TouchIt(GameObject oneObject)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = oneObject.transform.position + new Vector3(0.07f, -0.16f, -0.1f);
    }
}