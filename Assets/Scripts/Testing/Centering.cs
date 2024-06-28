using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Centering : MonoBehaviour
{
    void Start()
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(gameObject.transform);
        obj.transform.localPosition = gameObject.GetComponentInChildren<Renderer>().bounds.center;
        obj.transform.RotateAround(transform.position, transform.up, 180f);
        var XRComponent = gameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        XRComponent.attachTransform = obj.transform;
    }
}
