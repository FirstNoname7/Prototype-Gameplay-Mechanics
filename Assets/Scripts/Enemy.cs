using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player; //здесь в инспекторе указан перс игрока (нужен дл€ того, чтобы враг своей позицией приближалс€ к его позиции)
    private Rigidbody enemyRB; //подключаем жЄсткое тело
    public float speed; //скорость перемещени€ врага

    public bool isBoss = false; //по дефолту босс не по€вл€етс€ сразу же
    public float spawnInterval; //интервал, спуст€ который по€вл€етс€ босс
    private float nextSpawn; //продолжение спауна
    public int miniEnemySpawnCount; //тут указаны мини-враги, которых плодит босс
    private SpawnManager spawnManager; //используем скрипт с именем SpawnManager через переменную spawnManager

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>(); //подключаем компонент жЄсткого тела
        player = GameObject.Find("Player"); //подключаем игровой объект с названием Player

        if (isBoss) //если на карте по€вилс€ босс, то:
        {
            spawnManager = FindObjectOfType<SpawnManager>(); //подключаю тип объекта SpawnManager (скрипт+объект в иерархии)
        }
    }

    void Update()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized; //player.transform.position - текуща€ позици€ игрока,
        //transform.position - текуща€ позици€ врага. ¬ычитаем из позиции игрока позицию врага, то есть постепенно враг тер€ет свою позицию и приближаетс€ к игроку.
        //normalized - нужен дл€ сглаживани€ движени€ врага. если не поставить, то при большом отклонении перса игрока от врага в выражении (player.transform.position - transform.position) будет слишком большое значение и при умножении на скорость выйдет так, что враг будет нестись на перса игрока как сумасшедший. ј normalized устанавливает, что враг будет двигатьс€ с одной и той же скоростью на разном рассто€нии.
        enemyRB.AddForce(lookDirection * speed * Time.deltaTime); //враг движетс€ к игроку. lookDirection - направление, в котором враг движетс€ (см.переменную чуть выше). speed - скорость, с которой движетс€ враг

        if (transform.position.y < -3) //если позици€ врага меньше -3, то:
        {
            Destroy(gameObject); //удал€ем его
        }

        if (isBoss) //если сейчас на карте есть босс, то:
        {
            if (Time.time > nextSpawn) //если начало отсчЄта времени (0) больше, чем врем€, нужное дл€ следующего спауна, то:
            {
                nextSpawn = Time.time + spawnInterval; //врем€, нужное дл€ следующего спауна объектов = 0 + определЄнный интервал из переменной spawnInterval
                spawnManager.SpawnMiniEnemy(miniEnemySpawnCount); //используем функцию с названием SpawnMiniEnemy из скрипта SpawnManager, а именно объекты miniEnemySpawnCount
            }
        }
    }

}
