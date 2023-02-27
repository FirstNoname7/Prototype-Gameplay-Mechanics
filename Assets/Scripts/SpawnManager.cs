using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab; //тут указан массив игровых объектов для спауна
    public GameObject[] powerupPrefabs; //тут указан массив игровых объектов суперсилы
    public int enemyCount; //количество активных врагов на сцене
    public int waveNumber = 1; //из скольки врагов состоит волна (как в растения против зомби волна)

    public GameObject bossPrefab; //тут указан игровой объект босса
    public GameObject[] miniEnemyPrefabs; //тут указан массив мини-врагов, которых порождает босс
    public int bossRound; //через сколько раундов появится босс

    private float spawnRange = 5.0f; //переменная для определения рандомной позиции для появления врага

    void Start()
    {
        int randomPowerup = Random.Range(0, powerupPrefabs.Length); //указываем индекс для массива суперсилы. От нуля до последнего по счёту элемента в массиве.
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation); //спаун префабов суперсилы: powerupPrefabs[randomPowerup] - объект[его порядковый номер в массиве],
                                                                                                                               //GenerateSpawnPosition() - позиция объекта выражена через функцию с таким именем, 
                                                                                                                               //powerupPrefabs[randomPowerup].transform.rotation - вращение объекта берётся у самого объекта
        SpawnEnemyWave(waveNumber); //в скобках указано значение для параметра enemiesToSpawn (то есть enemiesToSpawn = waveNumber). То есть при старте будет 1 враг.

    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length; //переменная enemyCount равна количеству игровых объектов с именем Enemy. Если на карте один враг, то enemyCount = 1.

        if (enemyCount == 0) //если переменная равна 0, то есть все враги исчезли с поля боя, то:
        {
            waveNumber++; //увеличиваем waveNumber на 1 (то есть при старте будет 1 враг, когда он упадёт, то появится 2 врага, когда 2 врага упадут, то появится 3 врага и т.д.)
            if (waveNumber % bossRound == 0) //если остаток от деления кол-ва боссов на кол-во раундов, оставшихся до появления босса, равен нулю, значит пришла пора спаунить босса:
            {
                SpawnBossWave(waveNumber); //используем функцию с именем SpawnBossWave и задаём параметр waveNumber (то есть спауним waveNumber боссов)
            }
            else //в обратном случае:
            {
                SpawnEnemyWave(waveNumber); //используем функцию с именем SpawnEnemyWave и задаём параметр waveNumber (то есть спауним waveNumber врагов)
            }
            int randomPowerup = Random.Range(0, powerupPrefabs.Length); //указываем индекс для массива суперсилы. От нуля до последнего по счёту элемента в массиве.
            Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation); //создаём спаун для суперсилы через Instantiate(копирование)
                                                                                                                                   //powerupPrefab[randomPowerup] - объект[его порядковый номер в массиве], который спаунится (суперсила)
                                                                                                                                   //GenerateSpawnPosition() - позиция суперсилы выражена через функцию с таким именем 
                                                                                                                                   //powerupPrefab[randomPowerup].transform.rotation - вращение берётся у объекта[его порядковый номер в массиве], который спаунится.

        }

    }

    private Vector3 GenerateSpawnPosition()
    {
        float x = Random.Range(-spawnRange, spawnRange); //рандомная позиция по оси Х
        float z = Random.Range(0, spawnRange); //рандомная позиция по оси Z
        Vector3 randomPos = new Vector3(x, 0.6f, z); //new Vector3(x, 0, z) - место спауна, по оси X = переменная X, по оси Y = 0, по оси Z = переменная z.
        return randomPos; //если где-то в другой функции ссылаемся на эту функцию (GenerateSpawnPosition), то отдаём только переменную randomPos, чтоб путаницы не вышло                                                                                                                                                                                                                                                                                   
    }

    void SpawnEnemyWave(int enemiesToSpawn) //в этой функции содержится спаун врагов
    {

        for(int i = 0; i < enemiesToSpawn; i++) //в цикле переменная i изначально равна нулю (int i = 0;). Если она меньше значения переменной enemiesToSpawn (i < enemiesToSpawn;), то к i прибавляется единица (i++). Как только i будет больше переменной enemiesToSpawn (i > enemiesToSpawn), цикл остановится.
                                                //если вкратце то логика этого цикла такова: создаём не больше enemiesToSpawn врагов, чтоб перс игрока не задавили
        {
            int enemyIndex = Random.Range(0, enemyPrefab.Length);

            Instantiate(enemyPrefab[enemyIndex], GenerateSpawnPosition(), enemyPrefab[enemyIndex].transform.rotation); //создаём спаун через Instantiate(копирование)
                                                                                               //enemyPrefab - объект, который спаунится (враги)
                                                                                               //GenerateSpawnPosition() - позиция врага выражена через функцию с таким именем 
                                                                                               //enemyPrefab.transform.rotation - вращение берётся у объекта, который спаунится.

        }
    }

    void SpawnBossWave(int currentRound) //функция для спауна боссов. currentRound - текущее указание, есть ли босс
    {
        int miniEnemysToSpawn; //кол-во мини-врагов на сцене, когда появляется босс
        if (bossRound != 0) //если кол-во раундов, по истечении которых появится босс, не равно 0 (то есть босс не на сцене), то:
        {
            miniEnemysToSpawn = currentRound / bossRound; //делим текущий раунд на кол-во раундов, спустя которое появится босс
        }
        else //в обратном случае (если босс на сцене):
        {
            miniEnemysToSpawn = 1; //спаунится 1 мини-враг
        }

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation); //спаун боссов. Появляется объект bossPrefab с позицией из функции GenerateSpawnPosition() и своим вращением bossPrefab.transform.rotation
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn; //мини-врага, прописанного в скрипте Enemy (miniEnemySpawnCount), включаю в переменную miniEnemysToSpawn, созданную в этом скрипте, чтобы использовать здесь его значение
    }

    public void SpawnMiniEnemy(int amount) //функция для спауна мини-врагов. amount - сумма для цикла
    {
        for(int i = 0; i < amount; i++) //изначально переменная i равна 0. Если она меньше amount, то к ней добавляется единица (i++) и цикл идёт.
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length); //рандомайзер: будут спаунится рандомные мини-враги
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation); //спаун мини-врагов:
            //miniEnemyPrefabs[randomMini] - объект[индекс объекта], GenerateSpawnPosition() - позиция из функции с таким названием, miniEnemyPrefabs[randomMini].transform.rotation - текущее вращение объекта
        }
    }
}
