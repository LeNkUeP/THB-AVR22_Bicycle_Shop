using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class AssemblingStation : MonoBehaviour
{
    Vector3 startPosition;
    AudioSource sound;
    bool isPlaying = false;
    bool movingDown = false;
    bool movingUp = false;
    float audioTime = 1;

    public bool onlyFullBike = true;
    public float speed = 0.5f;
    public float transferDelay = 10f;
    public string[] bikeStations;

    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets;

    private void Start()
    {
        sound = GetComponent<AudioSource>();
        startPosition = transform.localPosition;
    }

    public bool IsBikeCompleted()
    {
        if (onlyFullBike)
        {
            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket in sockets)
            {
                if (!socket.hasSelection)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public string GetAvailableStationInShop()
    {
        foreach (string name in bikeStations)
        {
            GameObject station = GameObject.Find(name);
            if (station != null && station.GetComponentInChildren<BikeStation>().IsEmpty())
            {
                return name;
            }
        }
        return null;
    }

    public GameObject GetBike()
    {
        GameObject fullBike = new GameObject();
        fullBike.name = "Bike";

        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket in sockets)
        {
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable currentItem = socket.GetOldestInteractableSelected();
            if (currentItem != null)
            {
                Destroy(currentItem.transform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>());
                Destroy(currentItem.transform.GetComponent<MeshCollider>());
                Destroy(currentItem.transform.GetComponent<Rigidbody>());
                currentItem.transform.parent = fullBike.transform;
                CleanChildren(currentItem.transform.gameObject);
            }
        }

        return fullBike;
    }

    public void CleanChildren(GameObject obj)
    {
        int nbChildren = obj.transform.childCount;

        for (int i = nbChildren - 1; i >= 0; i--)
        {
            Destroy(obj.transform.GetChild(i).gameObject);
        }
    }

    public bool SendBikeToShop()
    {
        string availableStation = GetAvailableStationInShop();
        if (availableStation != null)
        {
            GameObject station = GameObject.Find(availableStation);
            GameObject bike = GetBike();
            bike.transform.position = station.transform.position;
            bike.transform.rotation = station.transform.rotation;
            StartCoroutine(StartTransferAnimation(transferDelay, bike, station));

            return true;
        }
        return false;
    }

    public IEnumerator StartTransferAnimation(float t, GameObject bike, GameObject station)
    {
        yield return new WaitForSeconds(t);
        station.GetComponentInChildren<BikeStation>().StartAnimation(bike);
    }

    public void startAnimation()
    {
        if (!isPlaying && IsBikeCompleted() && GetAvailableStationInShop() != null)
        {
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
                    SendBikeToShop();
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
                }
            }
        }
    }
}
