using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerEnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    public int level;
    public Text round;

    [SerializeField]
    private int spawnMultiplyer;

    private bool _stopSpawning;
    // Start is called before the first frame update
    void Start()
    {
        level = 1;
        round.text = "Round " + level;
        spawnMultiplyer = 5;
        _stopSpawning = false;
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //spawn gameObjects every 5sec

    IEnumerator SpawnEnemy()
    {

        while (_stopSpawning == false)
        {
            yield return null;
            for (int i = 0; i < spawnMultiplyer * level; i++)
            {
                if (_stopSpawning == false)
                {
                    yield return null;
                    float spawnSide = Random.Range(0f, 1.0f);
                    Debug.Log("spawn side = " + spawnSide);
                    Vector3 posToSpawn;
                    if (spawnSide < 0.5f)
                    {
                        posToSpawn = new Vector3(-64f, 1.1f, 0);
                    }
                    else
                    {
                        posToSpawn = new Vector3(18f, 1.1f, 0);

                    }
                    GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);


                    newEnemy.transform.parent = _enemyContainer.transform;
                    yield return new WaitForSeconds(5.0f);

                }
                else
                    break;
            }
            while (_enemyContainer.transform.childCount > 0)
            {
                yield return null;
            }
            level++;
            round.text = "Round " + level;
            yield return new WaitForSeconds(10.0f);

        }



    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
