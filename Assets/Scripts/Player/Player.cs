using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public static event EventHandler onAnyPlayerSpawned;
    public static Player LocalInstance { get; private set; }


    //GamePlay
    [SerializeField] private float HealthMax;
    [NonSerialized] public float Health;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float squishChange = 5f;
    [SerializeField] private Color BodyColor;
    [SerializeField] private Vector3 squishSize = new Vector3(0.2f, 1.5f, 1f);
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject Parent;
    //Anim
    [SerializeField] private Animator mouthAnim;
    [SerializeField] private Animator EyesAnim;
    [SerializeField] private SpriteRenderer MouthSprite;
    [SerializeField] private SpriteRenderer BodySprite;
    [SerializeField] private float damageMultiplier = 0.1f;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private playerVisual playerVisual;
    GameObject UsedUI;

	public static List<bool> numberOfPlayers = new List<bool>();
    private int playerPosition;
	private PowerUpFunctions.powerup currentPowerUp = PowerUpFunctions.powerup.None;
    private PowerUpItem powerup;
    private bool DealDamage;
    private bool gameStarted = false;
	Vector2 normalVector;
    private Rigidbody2D rb;
    private RoundManager.DamageType damageType;
    public int roundposition;
    //Events
    public static event EventHandler OnAnyPlayerHitWall;
    public Vector3 currentVelocity;

    public event EventHandler<HealthChangeEventArgs> HealthChange;
    public class HealthChangeEventArgs : EventArgs {
        public float newHealth;
    }


    public event EventHandler<UpdateIconArgs> UpdateIcon;
    public class UpdateIconArgs : EventArgs {
        public Sprite Icon;
    }

	public void Awake() {
        //Instance = this;
        playerPosition = numberOfPlayers.Count + 1;

		numberOfPlayers.Add(true);
        textMeshProUGUI.text = "Player " + playerPosition;
	}

	public void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        BodySprite.color = BodyColor;
        MouthSprite.color = BodyColor;
        Health = HealthMax;
        Parent = GameObject.Find("Canvas/PlayersUI");
        UsedUI = Instantiate(UI, Parent.gameObject.transform);//network issue
        UsedUI.GetComponent<PlayerUI>().setPlayer(this);
		GameInput.instance.onPowerUpPerformed += GameInput_onPowerUpPerformed;
        damageType = RoundManager.instance.damageType;
        RoundManager.instance.addPlayer();
        RoundManager.instance.isLocalPlayerReady = true;
        PlayerData playerData = MultiplayerManager.instance.GetPlayerDatafromClientId(OwnerClientId);
        playerVisual.setPlayerColor(MultiplayerManager.instance.getPlayerColor(playerData.ColorId));
	}

	public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }

		transform.position = RoundManager.instance.spawnPositions[MultiplayerManager.instance.GetPlayerDataIndexfromClientId(OwnerClientId)];
		onAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
        if (IsServer) {
            NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        }
	}

	private void Singleton_OnClientDisconnectCallback(ulong ClientId) {
        if (ClientId == OwnerClientId) {
            Destroy(UsedUI);
        }
	}

	public void Update() {
        if (!IsOwner) {
            return;
        }
        if (RoundManager.instance.IsGameplaying()) {
            if (!gameStarted) {
                rb.gravityScale = 1;
                gameStarted = true;
            }
            HandleMovement();
        } else {
            gameStarted = false;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        HandleSquish();
        updateVelocityServerRpc(GetComponent<Rigidbody2D>().velocity.x, GetComponent<Rigidbody2D>().velocity.y);
	}

    [ServerRpc]
    public void updateVelocityServerRpc(float x, float y) {
        updateVelocityClientRpc(x, y);
    }
    [ClientRpc]
    public void updateVelocityClientRpc(float x, float y) {
        currentVelocity = new Vector3(x, y, 0);
    }

    private void GameInput_onPowerUpPerformed(object sender, EventArgs e) {
        if (RoundManager.instance.IsGameplaying() && IsOwner) {
		    PowerUpFunctions.instance.UsePowerup(currentPowerUp, GameInput.instance, gameObject);
            currentPowerUp = PowerUpFunctions.powerup.None;
            emptyIconServerRpc();
        }
    }

    [ServerRpc]
    private void emptyIconServerRpc() {
        emptyIconClientRpc();
    }
    [ClientRpc]
    private void emptyIconClientRpc() {

		UpdateIcon?.Invoke(this, new UpdateIconArgs {
            Icon = null
        });
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        if (!IsOwner) { return; }
        normalVector = transform.position -  new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
        float rotation = Vector3.Angle(normalVector, Vector3.right);
        transform.localScale = Quaternion.Euler(0, 0, rotation) * squishSize;
        transform.localScale = transform.localScale.Abs();
        //do this on dealt damage
        if (collision.collider.gameObject.TryGetComponent<Player>(out var player)) {
            EyesAnim.SetBool("hit", true);
            mouthAnim.SetBool("Hit", true);
            
            PlayerDealDamage(collision, player, this);
        } else {
            onWallHitServerRpc();
        }
	}

	public void PlayerDealDamage(Collision2D collision, Player attacking, Player defending) {
		switch (damageType) {
			case RoundManager.DamageType.None://do nothing
				break;
			case RoundManager.DamageType.Stocks://need this coded
				break;
			case RoundManager.DamageType.Percentage:
                Vector3 damagingVelocity = (-1 * attacking.currentVelocity) + (currentVelocity);
                Vector3 PlayerDir = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) - collision.GetContact(0).point;
                double angle = (180 * Math.Acos(Vector2.Dot(PlayerDir, damagingVelocity)/((damagingVelocity.magnitude) * PlayerDir.magnitude)))/Math.PI;

                if (angle < 90 || angle > 270) {
                    //float damage = damageMultiplier * Vector3.Project(damagingVelocity, PlayerDir).magnitude;
                    float damage = damageMultiplier * damagingVelocity.magnitude;

                    Health = Health - damage;
                    
                    dealDamgeServerRpc(Health);
                } else {
                }
				break;
		}
	}

    [ServerRpc]
    private void dealDamgeServerRpc(float damage) {
        dealDamgeClientRpc(damage);
    }
    [ClientRpc]
    private void dealDamgeClientRpc(float damage) {
        changeHealth(damage);
    }
	[ServerRpc]
    public void onWallHitServerRpc() {
        onWallHitClientRpc();
    }

	[ClientRpc]
	public void onWallHitClientRpc() {
		OnAnyPlayerHitWall?.Invoke(this, EventArgs.Empty);
	}

	public void OnCollisionExit2D(Collision2D collision) {
		if (!IsOwner) { return; }
		EyesAnim.SetBool("hit", false);
		mouthAnim.SetBool("Hit", false);
	}

	public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.TryGetComponent<PowerUpItem>(out var powerupItem) && currentPowerUp == PowerUpFunctions.powerup.None) {
            powerup = powerupItem;
            FixedString32Bytes name = collision.gameObject.name;
            if (IsOwner) {
                currentPowerUp = powerupItem.powerup;
                collectServerRpc(name);
            }
        }
    }

    [ServerRpc]
    private void collectServerRpc(FixedString32Bytes name) {
        collectClientRpc(name);
    }

    [ClientRpc]
    private void collectClientRpc(FixedString32Bytes name) {
        powerup = GameObject.Find(name.ToString()).GetComponent<PowerUpItem>();
		powerup.collect();
		UpdateIcon?.Invoke(this, new UpdateIconArgs {
			Icon = powerup.powerUpSO.Sprite
		});

	}

    private void HandleMovement() {
        rb.velocity += GameInput.instance.getMovementVectorNormalized() * moveSpeed; //network issue
    }

    private void HandleSquish() {

		transform.right = Vector3.right;
		if (rb.velocity.magnitude > maxSpeed) {
			rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.normalized * maxSpeed, Time.deltaTime * 3);
		}

		transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * squishChange);
	}

    private void changeHealth(float health) {
        HealthChange?.Invoke(this, new HealthChangeEventArgs {
            newHealth = health
        });
        Health = health;
        Debug.Log("New Health: " + health);
        if (health <= 0) {
            //die
            numberOfPlayers[playerPosition - 1] = false;
            if (IsOwner) {
                gameObject.transform.position = new Vector3(1000, 1000, 0);
            }
            //Move Really far away
            //spawn death Particle
            //start respawn timer
            //at end of respawn bring back to map
        }
    }

    #region gets/sets
    public float getMoveSpeed() {
        return moveSpeed;
    }

	public static void ResetStaticData() {
        onAnyPlayerSpawned = null;
        OnAnyPlayerHitWall = null;
	}
	#endregion
}
