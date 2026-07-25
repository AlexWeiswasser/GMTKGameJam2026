using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] possibleEnemies;
    
    void Start()
    {
        int enemyChoice = UnityEngine.Random.Range(0, possibleEnemies.Length);
        Instantiate(possibleEnemies[enemyChoice], transform.position, Quaternion.identity);
    }
}
