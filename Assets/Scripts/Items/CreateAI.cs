using UnityEngine;

public class CreateAI : MonoBehaviour
{
    public GameObject[] enemyPrefabs; 
    public float spawnInterval = 2f; 
    public Vector3 spawnArea = new Vector3(10f, 0f, 10f); 
    private float timer; 

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval; 
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime; 

        if (timer <= 0f)
        {
            SpawnEnemy();
            timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        int prefabIndex = Random.Range(0, enemyPrefabs.Length);

        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            spawnArea.y,
            Random.Range(-spawnArea.z / 2, spawnArea.z / 2)
        );


        spawnPosition += transform.position;

        Instantiate(enemyPrefabs[prefabIndex], spawnPosition, Quaternion.identity);
    }
}
