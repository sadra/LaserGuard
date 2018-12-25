using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] List<WaveConfig> waves;
    [SerializeField] int startingWave = 0;
    [SerializeField] bool looping = false;


    IEnumerator Start()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        } while (looping);
    }

    private IEnumerator SpawnAllWaves()
    {
        waves.Clear();
        waves.Add(waveConfigs[Random.Range(0, 4)]);
        waves.Add(waveConfigs[Random.Range(0, 4)]);
        waves.Add(waveConfigs[Random.Range(0, 4)]);
        waves.Add(waveConfigs[Random.Range(0, 4)]);

        for (int waveIndex = startingWave; waveIndex < waves.Count; waveIndex++)
        {
            var currentWave = waves[waveIndex];
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
        }
    }

    private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for(int enemyCount=0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            var newEnemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetWayPoints()[0].transform.position, Quaternion.identity);
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);
            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawn());
        }
    }


}
