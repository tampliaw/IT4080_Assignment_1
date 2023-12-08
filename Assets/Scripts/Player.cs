using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public NetworkVariable<Color> playerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> ScoreNetVar = new NetworkVariable<int>(0);
    public BulletSpawner bulletSpawner;
    public NetworkVariable<int> playerHP = new NetworkVariable<int>();

    public float movementSpeed = 25f;
    public float rotationSpeed = 100f;
    private Camera playerCamera;
    private GameObject playerBody;

    private void NetworkInit()
    {
        playerBody = transform.Find("PlayerBody").gameObject;

        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;

        ApplyColor();
        playerColor.OnValueChanged += OnPlayerColorChanged;

        if (IsClient)
        {
            ScoreNetVar.OnValueChanged += ClientOnScoreValueChanged;
        }

    }

    private void Awake()
    {
        NetworkHelper.Log(this, "Awake");
    }

    private void Start()
    {
        NetworkHelper.Log(this, "Start");
    }

    private void Update()
    {
        if (IsOwner)
        {
            OwnerHandleInput();
            if (Input.GetButtonDown("Fire1"))
            {
                NetworkHelper.Log("Requesting Fire");
                bulletSpawner.FireServerRpc();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkInit();
        base.OnNetworkSpawn();
        playerHP.Value = 100;
    }

    private void ClientOnScoreValueChanged(int old, int current)
    {
        if (IsOwner)
        {
            NetworkHelper.Log(this, $"My score is {ScoreNetVar.Value}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            ServerHandleCollision(collision);
        }
    }


    private void ServerHandleCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            NetworkHelper.Log(this,
                $"Hit by {collision.gameObject.name} " +
                $"owned by {ownerId}");
            Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
            Destroy(collision.gameObject);
            other.ScoreNetVar.Value += 1;
            playerHP.Value -= 50;
        }
        if (collision.gameObject.CompareTag("bullet"))
        {
            ulong ownerId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
            NetworkHelper.Log(this,
                $"Hit by {collision.gameObject.name} " +
                $"owned by {ownerId}");
            Player other = NetworkManager.Singleton.ConnectedClients[ownerId].PlayerObject.GetComponent<Player>();
            Destroy(collision.gameObject);
            other.ScoreNetVar.Value += 1;
            playerHP.Value -= 25;

            if (playerHP.Value <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void OnPlayerColorChanged(Color previous, Color current)
    {
        ApplyColor();
    }

    private void OwnerHandleInput()
    {
        Vector3 movement = CalcMovement();
        Vector3 rotation = CalcRotation();
        if (movement != Vector3.zero || rotation != Vector3.zero)
        { 
            MoveServerRpc(movement, rotation);
        }
    }

    private void ApplyColor()
    {
        NetworkHelper.Log(this, $"Applying color {playerColor.Value}");
        playerBody.GetComponent<MeshRenderer>().material.color = playerColor.Value;
    }

    [ServerRpc(RequireOwnership = true)]
    private void MoveServerRpc(Vector3 movement, Vector3 rotation)
    {
        transform.Translate(movement);
        transform.Rotate(rotation);
    }

    // Rotate around the y axis when shift is not pressed
    private Vector3 CalcRotation()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown)
        {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }


    // Move up and back, and strafe when shift is pressed
    private Vector3 CalcMovement()
    {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown)
        {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

        return moveVect;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer)
        {
            return;
        }
        if (other.GetComponent<BulletSpawner>())
        {
            Debug.Log("Player damage " + other.GetComponent<NetworkObject>().OwnerClientId);

            playerHP.Value -= 50;
        }

        if (IsServer)
        {
            if (other.GetComponent<BulletSpawner>())
            {
                Debug.Log("Player damage " + other.GetComponent<NetworkObject>().OwnerClientId);

                playerHP.Value -= 50;

                if (playerHP.Value <= 0)
                {
                    Die();
                }
            }

            if (IsServer)
            {
                if (other.CompareTag("power_up"))
                {
                    other.GetComponent<BasePowerUp>().ServerPickUp(this);
                }
            }
        }
    }
}
