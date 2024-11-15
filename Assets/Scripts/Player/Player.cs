using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : NetworkBehaviour {

    public static event EventHandler onAnyPlayerSpawned;
    public static Player LocalInstance { get; private set; }

    public LobbyUI mylobbyui;
    //GamePlay
    [SerializeField] private float HealthMax;
    [NonSerialized] public float Health;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] public float MaxSpeed = 30f;
    public float currentMaxSpeed = 30f;
    [SerializeField] public float weight = 1f;
    [SerializeField] private float squishChange = 5f;
    [SerializeField] private Color BodyColor;
    [SerializeField] private Vector3 squishSize = new Vector3(0.2f, 1.5f, 1f);
    [SerializeField] private GameObject FighterUI;
    [SerializeField] private GameObject KOTHUI;
    [SerializeField] private GameObject RaceUI;
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
    private float powerupTimer = 0;
	public static Dictionary<ulong, int> numberOfPlayers = new Dictionary<ulong, int>();
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
    float kothTime = 1f;

    public event EventHandler<HealthChangeEventArgs> HealthChange;
    public class HealthChangeEventArgs : EventArgs {
        public float newHealth;
    }

	public event EventHandler<PointsChangeEventArgs> PointsChange;
	public class PointsChangeEventArgs : EventArgs {
		public float newPoints;
	}

	public event EventHandler<UpdateIconArgs> UpdateIcon;
    public class UpdateIconArgs : EventArgs {
        public Sprite Icon;
    }

	public void Awake() {
        //Instance = this;
	}

	public void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        BodySprite.color = BodyColor;
        MouthSprite.color = BodyColor;
        Health = HealthMax;
        if (RoundManager.instance.damageType == RoundManager.DamageType.Percentage) {
            Parent = GameObject.Find("Canvas/PlayersUI");
            UsedUI = Instantiate(FighterUI, Parent.gameObject.transform);//network issue
            UsedUI.GetComponent<PlayerUI>().setPlayer(this);
        }

		if (RoundManager.instance.GetGamemode() == RoundManager.Gamemode.KingOfHill) {
			Parent = GameObject.Find("Canvas/PlayersUI");
			UsedUI = Instantiate(KOTHUI, Parent.gameObject.transform);//network issue
			UsedUI.GetComponent<PlayerUI>().setPlayer(this);
		}
		if (RoundManager.instance.GetGamemode() == RoundManager.Gamemode.Race) {
			Parent = GameObject.Find("Canvas/PlayersUI");
			UsedUI = Instantiate(RaceUI, Parent.gameObject.transform);//network issue
			UsedUI.GetComponent<PlayerUI>().setPlayer(this);
		}
		GameInput.instance.onPowerUpPerformed += GameInput_onPowerUpPerformed;
        damageType = RoundManager.instance.damageType;
        RoundManager.instance.addPlayer();
        RoundManager.instance.isLocalPlayerReady = true;
        PlayerData playerData = MultiplayerManager.instance.GetPlayerDatafromClientId(OwnerClientId);
        playerVisual.setPlayerColor(MultiplayerManager.instance.getPlayerColor(playerData.ColorId));
        UsedUI.GetComponent<PlayerUI>().setPlayerColor(MultiplayerManager.instance.getPlayerColor(playerData.ColorId));
	}

	public override void OnNetworkSpawn() {
        if (IsOwner) {
            LocalInstance = this;
        }

		if (!numberOfPlayers.ContainsKey(OwnerClientId)) {
			numberOfPlayers.Add(OwnerClientId, 1);
		} else {
			numberOfPlayers[OwnerClientId] = 1;
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

        powerupTimer -= Time.deltaTime;

        if (powerupTimer <= 0) {
            powerupTimer = 1;
            currentMaxSpeed = MaxSpeed;
            updateWeight(1, 1);
            playerVisual.Normal();
        }
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
                Vector3 damagingVelocity = (attacking.currentVelocity * attacking.weight) + (defending.currentVelocity * defending.weight);
                Vector3 PlayerDir = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y) - collision.GetContact(0).point;
                double angle = (180 * Math.Acos(Vector2.Dot(PlayerDir, damagingVelocity)/((damagingVelocity.magnitude) * PlayerDir.magnitude)))/Math.PI;

                if (angle < 90 || angle > 270) {
                    //float damage = damageMultiplier * Vector3.Project(damagingVelocity, PlayerDir).magnitude;
                    float damage = damageMultiplier * damagingVelocity.magnitude;

                    Health = Health - damage;
                    
                    attacking.dealDamgeServerRpc(Health);
                } else {
                }
				break;
		}
	}

    public void updateWeight(float multiplier, float time) {
        updateWeightServerRpc(multiplier, time);
	}
    [ServerRpc(RequireOwnership = false)]
    public void updateWeightServerRpc(float multiplier, float time) {
        updateWeightClientRpc(multiplier,time);
	}
    [ClientRpc]
    public void updateWeightClientRpc(float multiplier, float time) {
		powerupTimer = time;
		weight = multiplier;
        if (weight > 1) {
            playerVisual.Heavy();
        }
        if (weight < 1) {
            playerVisual.Light();
        }
    }

		[ServerRpc(RequireOwnership = false)]
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

		if (collision.gameObject.tag == "RaceFinish" && RoundManager.instance.GetGamemode() == RoundManager.Gamemode.Race) {
            RoundManager.instance.OnfinishRaceServerRpc();
		}


	}

	private void OnTriggerStay2D(Collider2D collision) {
		if (collision.gameObject.tag == "KOTH" && RoundManager.instance.GetGamemode() == RoundManager.Gamemode.KingOfHill) {
			kothTime -= Time.deltaTime;
			if (IsOwner && kothTime <= 0) {
				kothTime = 1f;
				addKOTHPointsServerRpc();
			}
		}
		if (IsOwner && collision.gameObject.tag == "TimeBomb") {
			currentMaxSpeed = MaxSpeed / 4;
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if (IsOwner && collision.gameObject.tag == "TimeBomb") {
			currentMaxSpeed *= 4;
		}
	}

	[ServerRpc]
    public void addKOTHPointsServerRpc() {
        addKOTHPointsClientRpc();
    }

    [ClientRpc]
    public void addKOTHPointsClientRpc() {
        numberOfPlayers[OwnerClientId] = numberOfPlayers[OwnerClientId] + 1;
        PointsChange?.Invoke(this, new PointsChangeEventArgs {
            newPoints = numberOfPlayers[OwnerClientId]
        });
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
        if (rb.velocity.magnitude > currentMaxSpeed) {
            rb.velocity = rb.velocity.normalized * currentMaxSpeed;
        }
    }

    private void HandleSquish() {

		transform.right = Vector3.right;
		if (rb.velocity.magnitude > currentMaxSpeed) {
			rb.velocity = Vector3.Lerp(rb.velocity, rb.velocity.normalized * currentMaxSpeed, Time.deltaTime * 3);
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
            numberOfPlayers[OwnerClientId] = 0;
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
