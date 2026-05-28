using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class OkIPullUp : MonoBehaviour
{
    public void PlaySound()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable currentItem = socket.GetOldestInteractableSelected();
        Destroy(currentItem.transform.gameObject);
        GetComponent<AudioSource>().Play();
    }
}
