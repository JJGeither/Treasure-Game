using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public PlayerDrones playerDrones { get; private set; }
    public InputHandler inputHandler { get; private set; }
    public PlayerMovement playerMovement { get; private set; }
    public PlayerStatistics playerStatistics { get; private set; }
    public Interactor interactor { get; private set; }

    public GameObject gameReticle; 
    public ReticleController reticleController { get; private set; }

    void Awake() => instance = this;

    void Start()
    {
        // Assuming the other scripts are on the same GameObject
        playerDrones = GetComponent<PlayerDrones>();
        inputHandler = GetComponent<InputHandler>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatistics = GetComponent<PlayerStatistics>();
        interactor = GetComponent<Interactor>();
        reticleController = gameReticle.GetComponent<ReticleController>();
    }
}
