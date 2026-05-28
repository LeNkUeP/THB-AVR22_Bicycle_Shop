using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BikeStation : MonoBehaviour
{
    GameObject bike;
    Vector3 startPosition;
    AudioSource sound;
    bool isPlaying = false;
    bool movingDown = false;
    bool movingUp = false;
    float audioTime = 1;

    public float speed = 0.5f;

    void Start()
    {
        sound = GetComponent<AudioSource>();
        startPosition = transform.localPosition;
    }

    public bool IsEmpty(){
        if (transform.childCount == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartAnimation(GameObject assembledBike)
    {
        if (!isPlaying && assembledBike != null)
        {
            bike = assembledBike;
            isPlaying = true;
            movingDown = true;
            sound.time = audioTime;
            sound.Play();
        }
    }

    void Update()
    {
        if (isPlaying)
        {
            if (movingDown)
            {
                if (transform.localPosition.y > 2)
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime);
                }
                else
                {
                    movingDown = false;
                    movingUp = true;
                    bike.transform.parent = transform;
                }
            }

            if (movingUp)
            {
                if (transform.localPosition.y < startPosition.y)
                {
                    transform.Translate(Vector3.up * speed * Time.deltaTime);
                }
                else
                {
                    movingUp = false;
                    isPlaying = false;
                    sound.Stop();
                    bike.AddComponent<Rigidbody>().isKinematic = false;
                    bike.GetComponent<Rigidbody>().useGravity = true;
                    bike.AddComponent<BoxCollider>().size = new Vector3(1.5f,1,0.5f);
                    bike.GetComponent<BoxCollider>().center = new Vector3(-3f, -0.4f, 5f);
                    GameObject attach = new GameObject("AttachTransform");
                    attach.transform.parent = bike.transform;
                    attach.transform.localPosition = new Vector3(-3f, -0.35f, 5f);
                    attach.transform.localRotation = Quaternion.Euler(0,0,0);
                    bike.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().attachTransform = attach.transform;
                    bike.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().interactionLayers = InteractionLayerMask.GetMask("FullBike");
                    bike.layer = LayerMask.NameToLayer("FullBike");
                    bike.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().selectExited.AddListener(delegate { DetachBike(); });
                }
            }
        }
    }

    void DetachBike()
    {
        // grabinteractable tries to reparent, so disable and enable to prevent
        bike.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().enabled = false;
        bike.transform.parent = null;
        bike.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().enabled = true;
    }
}
