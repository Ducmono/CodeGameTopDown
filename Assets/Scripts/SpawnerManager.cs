using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public float startTimeBtwSpawn;
    
    private float timeBtwSpawn;

    public GameObject[] enemies;

    public WeaponManager weaponManager;

    public List<Spawner> spawners;

    private Player player;
    int maxEnemy = 1;
    int roundCount = 0;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        timeBtwSpawn = startTimeBtwSpawn;
        
    }
    private void FixedUpdate()
    {
        this.FixedUpdate2();
    }
    private void FixedUpdate2()
    {
            timeBtwSpawn -= Time.deltaTime;

        if (weaponManager.Enemies.Count >= maxEnemy / 2)//số quái hiện tại lớn hơn hoặc bằng
        {
            startTimeBtwSpawn = Mathf.Clamp(startTimeBtwSpawn + 0.5f, 1f, 5f); // Tăng dần thời gian chờ giữa các lần spawn khi có nhiều quái
        }
        else
        {
            startTimeBtwSpawn = Mathf.Clamp(startTimeBtwSpawn - 0.1f, 1f, 5f); // Giảm thời gian chờ khi quái ít hơn
        }

        if (timeBtwSpawn <= 0)
        {
            SpawnEnemies();
            timeBtwSpawn = startTimeBtwSpawn;
            UpdateRound();
        }
    }

    private void SpawnEnemies()
    {
        int currentEnemyCount = weaponManager.Enemies.Count; // Số lượng quái hiện tại

        // Nếu số lượng quái đã đạt maxEnemy thì không sinh thêm
        if (currentEnemyCount >= maxEnemy) return; // Không sinh thêm quái nếu đã đạt giới hạn
       
        int randEnemyCount = GetEnemyCount();

        List<int> randomIndices = GetRandomIndices(spawners.Count, randEnemyCount);

        foreach (int index in randomIndices)
        {
            int randEnemy = UnityEngine.Random.Range(0, enemies.Length);
            spawners[index].spawnEnemy(enemies[randEnemy]);
        }
    }
    private int GetEnemyCount()//số quái sẽ spawn trong 1 lần
    {
        return weaponManager.Enemies.Count <= 5
            ? UnityEngine.Random.Range(1, Mathf.Min(maxEnemy, spawners.Count))
            : UnityEngine.Random.Range(0, maxEnemy);
    }
    private void UpdateRound()
    {
        roundCount++;
        if (roundCount > 12)
        {
            roundCount = 0;
            maxEnemy = Mathf.Min(spawners.Count, maxEnemy + 1);
        }
        startTimeBtwSpawn = Mathf.Clamp(startTimeBtwSpawn + 0.5f, 2f, 5f);
        maxEnemy = Mathf.Clamp(maxEnemy, 1, 10);
    }

    public List<int> GetRandomIndices(int n, int k)
    {
        List<int> allIndices = new List<int>(Enumerable.Range(0, n));
        List<int> randomIndices = new List<int>();

        int remainingItems = n;
        for (int i = 0; i < k; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, remainingItems);
            randomIndices.Add(allIndices[randomIndex]);
            allIndices[randomIndex] = allIndices[remainingItems - 1];
            remainingItems--;
        }

        return randomIndices;
    }
}
