using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class NPC : MonoBehaviour
{
    public GameObject destination;
    public GameObject beginning;
    public NavMeshAgent agent;
    public GameObject moneyBag;

    private GameObject currentDestination;
    public string idleAnim;
    public string walkingAnim;

    private void Start()
    {
        initDestination();
    }

    public void initDestination()
    {
        currentDestination = destination;
    }

    private void Update()
    {
        agent.SetDestination(currentDestination.transform.position);
    }
    public void GiveBike()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable bike = socket.GetOldestInteractableSelected();
        Destroy(bike.transform.gameObject);
        currentDestination = beginning;
        GetComponentInChildren<Animator>().Play(walkingAnim);

        Instantiate(moneyBag, transform.position, moneyBag.transform.localRotation);
    }
}