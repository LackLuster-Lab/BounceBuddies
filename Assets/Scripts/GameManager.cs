using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //all different variables for game modes
    enum GameState {
        Start,
        playing,
        Endgame
    }
    enum Gamemode {
        Fighter,
        Race,
        KingOfHill
    }
    enum DamageType {
        None,
        Stocks,
        Percentage
    }
    [SerializeField] Gamemode gamemode;//TODO implement
    [SerializeField] DamageType damageType;//TODO implement
    [SerializeField] bool powerUpsEnabled;
    [SerializeField] private float powerUpSpawnTimerMax = 4f;
    [SerializeField] GameObject[] allPowerUps;
    private float powerUpSpawnTimer;
    private GameObject currentPowerup;
    private GameState currentGameState = GameState.Start;
    //all map specific things
    [SerializeField] Vector4[] powerUpSpawnLocations;// X-Left, Y-Right, Z-Top, W-Bottom

	public void Update() {
        if (powerUpsEnabled) {
            if (powerUpSpawnTimer >= powerUpSpawnTimerMax && currentPowerup == null) {
                //spawn PowerUp;
                //get random powerup
                int powerupInt = Random.Range(0, allPowerUps.Length);
                //get random location
                int randomSpawnArea = Random.Range(0, powerUpSpawnLocations.Length);
                float randomX = Random.Range(powerUpSpawnLocations[randomSpawnArea].x, powerUpSpawnLocations[randomSpawnArea].y);
                float randomY = Random.Range(powerUpSpawnLocations[randomSpawnArea].w, powerUpSpawnLocations[randomSpawnArea].z);
                Vector3 position = new Vector3(randomX, randomY, 0);
                currentPowerup = Instantiate(allPowerUps[powerupInt], position, Quaternion.identity);
                //instatiate powerup
                powerUpSpawnTimer = 0;
            } else {
                powerUpSpawnTimer += Time.deltaTime;
            }
        }
	}
}
