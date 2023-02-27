using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f; //скорость перемещения перса
    public float forwardInput; //переменная для ввода с клавиатуры
    public bool hasPowerup; //переменная для того, чтоб узнать, есть у перса игрока суперсила или нет (собрал он её на карте или нет) 
    public GameObject powerupIndicator; //игровой объект индикатора для суперсилы (кружочка, который показывает, что есть суперсила)
    public PowerUpType currentPowerUp = PowerUpType.None; //перечисление типов PowerUp. Это надо, чтоб помочь определить, какую логику включить для игрока, когда будет собрана суперсила

    public GameObject rocketPrefab; //игровой объект ракета
    private GameObject tmpRocket; //переменная, нужная для размножения игрового объекта ракеты
    private Coroutine powerupCountdown; //обратный отсчёт для включения суперсилы (а конкретно самонаводящихся ракет)

    public float hangTime; //время, когда перс игрока при прыжке находится в подвешенном состоянии
    public float smashSpeed; //скорость удара при прыжке перса
    public float explosionForce; //сила взрыва при суперсиле прыжка перса
    public float explosionRadius; //радиус, на который распространяется взрыв суперсилы прыжка перса
    bool smashing = false; //по дефолту перс игрока не может применять суперсилу прыжка
    float floorY; //тут показано, что при применении суперсилы прыжка перс будет совершать прыжок по оси Y

    private Rigidbody playerRB; //жёсткое тело
    private GameObject focalPoint; //точка для вращения вокруг неё
    private float powerupStrength = 15.0f; //кол-во юнитов, на которое отлетает враг при применении к нему суперсилы

    void Start()
    {
        playerRB = GetComponent<Rigidbody>(); //подключаем (инициализируем) жёсткое тело
        focalPoint = GameObject.Find("FocalPoint"); //находим игровой объект (GameObject.Find) с именем FocalPoint
    }

    void Update()
    {
        if (transform.position.y < -3) //если перс игрока улетел за пределы игры
        {
            transform.position = new Vector3(0, 0.42f, 0); //возвращаю его в исходную позицию
        }

        forwardInput = Input.GetAxis("Vertical"); //нажатие на стрелки вверх-вниз
        playerRB.AddForce(focalPoint.transform.forward * speed * forwardInput); //перс будет перемещаться по оси Z, которая указана у игрового объекта FocalPoint (focalPoint.transform.forward) со скоростью speed при нажатии на стрелки вверх-вниз (forwardInput)

        powerupIndicator.transform.position = transform.position; //позиция кружочка (индикатора для суперсилы) равна позиции объекта, к которому прикреплён этот скрипт. То есть кружочек будет следовать за персом игрока

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F)) //если используется значение самонаводящихся ракет в текущей суперсиле и игрок нажал на клавишу F, то:
        {
            LaunchRockets(); //выполняется функция с названием LaunchRockets
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing) //если игрок собрал суперсилу прыжка (PowerUpType.Smash) и нажал на пробел и он в данный момент не использует суперсилу прыжка (!smashing), то:
        {
            smashing = true; //подключаем суперсилу прыжка
            StartCoroutine(Smash()); //выполняется корутина (последовательность действий) с именем Smash
        }
    }

    void OnTriggerEnter(Collider other) //такая функция выполняется, когда коллайдер сталкивается с триггером. В скобках параметр other обозначает другой коллайдер (Collider), нужен для того, чтоб его можно было удалять
    {
        if (other.CompareTag("Powerup")) //если игровой объект, с которым столкнулся перс игрока, с тегом Powerup, то:
        {
            hasPowerup = true; //появляется суперсила (игрок может её применить, чтоб оттолкнуть врагов)
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType; //прикрепляем к переменной currentPowerUp переменную powerUpType из игрового объекта PowerUp (из скрипта с этим именем)
            powerupIndicator.gameObject.SetActive(true); //включится игровой объект powerupIndicator (кружочек, показывающий, что перс может применить суперсилу)
            Destroy(other.gameObject); //удаляем этот другой игровой объект (суперсилу)

            if (powerupCountdown != null) //если обратный отсчёт закончился, то:
            {
                StopCoroutine(powerupCountdown); //прекращаем спаунить самонаводящиеся ракеты
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine()); //включаем функцию PowerupCountdownRoutine при помощи переменной powerupCountdown
        }
    }

    private void OnCollisionEnter(Collision collision) //такая функция выполняется, когда сталкиваются два жёстких тела (Rigidbody). В скобках параметр collision обозначает другое жёсткое тело, чтоб понять, было ли столкновение
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp==PowerUpType.Pushback) //если объект, к которому прикреплён скрипт (игрок) столкнулся с игровым объектом с тегом Enemy (с врагом) и он взял суперсилу (текущая суперсила==значение отталкивания (то есть в этом условии мы отталкиваем врагов, пока ещё не запускаем ракеты)), то:
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>(); //вызываем жёсткое тело (Rigidbody) игрового объекта (gameObject), с которым перс игрока сталкивается (с collision)
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position); //меняется положение врага: его позиция (collision.gameObject.transform.position) удаляется от позиции объекта, к которому прикреплён этот скрипт (transform.position)
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse); //тут указывается сила, с которой отлетит враг при применении суперсилы. awayFromPlayer - смена позиции врага зависит от переменной powerupStrength. ForceMode.Impulse - для более чистой физики.

            Debug.Log("Player collided with"+collision.gameObject.name+" with powerup set to "+currentPowerUp.ToString()); //выводится это сообщение в консоли
        }
        if (collision.gameObject.CompareTag("EnemyPower")) //если сталкиваемся с объектом с тегом EnemyPower (с врагом, у которого много сил), то:
        {
            Rigidbody playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>(); //вызываем жёсткое тело (Rigidbody) игрового объекта (GameObject) с именем Player
            Vector3 awayFromPlayer = (transform.position - collision.gameObject.transform.position); //меняется положение объекта, к которому прикреплён этот скрипт: его позиция (transform.position) удаляется от позиции объекта collision (то есть от врага)
            playerRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse); //тут указывается сила, с которой отлетит перс игрока при столкновении с суперсильным врагом. awayFromPlayer - смена позиции перса зависит от переменной powerupStrength. ForceMode.Impulse - для более чистой физики.
            Debug.Log("Collided with" + collision.gameObject.name + " with powerup set to" + hasPowerup); //выводится это сообщение в консоли
        }
    }

    void LaunchRockets() //функция для запуска самонаводящихся ракет
    {
        foreach(var enemy in FindObjectsOfType<Enemy>()) //foreach - это цикл, который нужен для перебора элементов, содержащихся в нём. var - тип переменной. enemy - название переменной, которая содержится внутри (in) массива FindObjectsOfType<Enemy>() 
            //цикл закончится, когда будут пересмотрены все элементы массива (когда ракета добьёт каждого врага на карте)
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity); //спаун самонаводящихся ракет. rocketPrefab - объект спауна. transform.position + Vector3.up - то есть ракета сначала немного взлетает вверх перса игрока (чтоб не оттолкнуть его). 
            //Quaternion.identity - без вращения 
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform); //используем позицию врага (enemy.transform) в функции Fire, которая находится в скрипте RocketBehaviour, чтобы самонаводящаяся ракета (tmpRocket) знала, на кого ей лететь
        }
    }

    IEnumerator PowerupCountdownRoutine() //IEnumerator - последовательность действий. То есть сначала выполняется первая строка, потом вторая и т.д., они выполняются НЕ одновременно. PowerupCountdownRoutine - название функции.
                                          //в такой функции содержится обратный отсчёт времени, после которого нельзя будет использовать суперсилу. 
    {
        yield return new WaitForSeconds(7); //yield - то, что позволяет выполнять действия каждый фрейм за пределами Update. return - возврат. new WaitForSeconds(7) - обратный отсчёт, начинающийся с 7 секунд (new нужно для того, чтобы таймер постоянно обновлялся. То есть если прошло 7 секунд, то при повторном включении функции отсчёт будет начинаться снова с 7 секунды).
        hasPowerup = false; //суперсила отключается
        currentPowerUp = PowerUpType.None; //возвращаем текущую суперсилу до нуля, используя значение None
        powerupIndicator.gameObject.SetActive(false); //отключается кружочек вокруг перса, показывающий, что суперсила доступна
    }

    IEnumerator Smash() //IEnumerator - последовательность действий. То есть сначала выполняется первая строка, потом вторая и т.д., они выполняются НЕ одновременно. Smash - название функции.
                                          //в такой функции содержится последовательность действий для суперсилы прыжка перса игрока
    {
        var enemies = FindObjectsOfType<Enemy>(); //подключаю тип объекта Enemy
        floorY = transform.position.y; //замораживаем позицию перса игрока по оси y
        float jumpTime = Time.time + hangTime; //время прыжка=время(оно начинается с нуля)+переменная hangTime

        while (Time.time < hangTime) //цикл while. Если hangTime больше Time.time, значит перс игрока ещё находится в прыжке, и вот в это время выполняется данный цикл.
        {
            playerRB.velocity = new Vector2(playerRB.transform.position.x, smashSpeed); //меняем положение перса игрока. Меняем только по осям X и Y, поэтому используем Vector2. playerRB.transform.position.x - позицию по оси X берём у перса игрока.
            //smashSpeed - позиция по оси Y равна значению переменной smashSpeed
            yield return null; //сбрасываем новую позицию, ибо потом надо отправить перса игрока из подвешенного состояния на землю
        }

        while (transform.position.y > floorY)
        {
            playerRB.velocity = new Vector2(playerRB.transform.position.x, -smashSpeed*2); //меняем положение перса игрока. Меняем только по осям X и Y, поэтому используем Vector2. playerRB.transform.position.x - позицию по оси X берём у перса игрока.
            //-smashSpeed*2 - позиция по оси Y: возвращаем перса игрока после прыжка обратно на землю. Если smashSpeed - это прыжок вверх, то -smashSpeed*2 - приземление
            yield return null; //сбрасываем новую позицию
        }

        for(int i = 0; i < enemies.Length; i++) //цикл for. Изначально переменная i = 0. Если она меньше значения индекса последнего элемента в массиве из скрипта Enemy (i < enemies.Length), то прибавляем единицу (i++)
        {
            if (enemies != null) //если объект, к которому прикреплён этот скрипт (перс игрока) столкнётся с enemies, то:
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse); //отталкиваем врага.
                //enemies[i] - указываем, что за враг (и его индекс i, ибо есть несколько типов врагов), подключаем жёсткое тело, чтоб можно было на него влиять (GetComponent<Rigidbody>())
                //AddExplosionForce - прикладывает силу к твёрдому телу, которая имитирует эффект взрыва.
                //explosionForce - сила взрыва. transform.position - центр позиции, внутри которой происходит взрыв. explosionRadius - радиус, в пределах которого происходит взрыв. 
                //0.0f - поднятие предметов (врагов) при взрыве (установлено на 0, значит подниматься они не будут по оси Y). ForceMode.Impulse - для более чистой физики
            }
        }

        smashing = false; //после всех вышеуказанных действий в нумераторе применение суперсилы прыжка прекращается
    }
}
