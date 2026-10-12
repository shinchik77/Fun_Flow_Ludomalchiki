using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FruitSp : MonoBehaviour
{
    private Collider spawnArea;

    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)]
    public float bombChance = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    [Header("Рост сложности")]
    public float difficultyInterval = 10f;   // Каждые 10 секунд усложняем
    public float difficultyStep = 0.1f;      // Насколько уменьшаем задержку
    public float minSpawnDelayLimit = 0.1f;  // Минимум задержки
    public float forceIncrease = 0.5f;       // Насколько увеличиваем силу

    private float difficultyTimer;

    private void Awake()
    {
        spawnArea = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            // === Рост сложности ===
            difficultyTimer += Time.deltaTime;
            if (difficultyTimer >= difficultyInterval)
            {
                minSpawnDelay = Mathf.Max(minSpawnDelayLimit, minSpawnDelay - difficultyStep);
                maxSpawnDelay = Mathf.Max(minSpawnDelayLimit * 2f, maxSpawnDelay - difficultyStep);
                minForce += forceIncrease;
                maxForce += forceIncrease;
                difficultyTimer = 0;
                Debug.Log("Сложность выросла! Задержка: " + minSpawnDelay + " - " + maxSpawnDelay);
            }

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (Random.value < bombChance)
                prefab = bombPrefab;

            Vector3 position = new Vector3
            {
                x = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                y = Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
                z = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
            };

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(fruit.transform.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}