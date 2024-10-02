using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //all different variables for game modes
    [SerializeField] bool powerUpsEnabled;
    [SerializeField] private float powerUpSpawnTimerMax = 4f;
    [SerializeField] GameObject[] allPowerUps;
    private float powerUpSpawnTimer;
    //all map specific things
    [SerializeField] Vector4[] powerUpSpawnLocations;

	public void Update() {
        if (powerUpsEnabled) {
            if (powerUpSpawnTimer >= powerUpSpawnTimerMax) {
                //spawn PowerUp;
                //get random powerup
                int powerupInt = Random.Range(0, allPowerUps.Length);
                //get random location
                //instatiate powerup
                powerUpSpawnTimer = 0;
            } else {
                powerUpSpawnTimer += Time.deltaTime;
            }
        }
	}
}
