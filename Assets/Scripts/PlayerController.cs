using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f; //�������� ����������� �����
    public float forwardInput; //���������� ��� ����� � ����������
    public bool hasPowerup; //���������� ��� ����, ���� ������, ���� � ����� ������ ��������� ��� ��� (������ �� � �� ����� ��� ���) 
    public GameObject powerupIndicator; //������� ������ ���������� ��� ��������� (��������, ������� ����������, ��� ���� ���������)
    public PowerUpType currentPowerUp = PowerUpType.None; //������������ ����� PowerUp. ��� ����, ���� ������ ����������, ����� ������ �������� ��� ������, ����� ����� ������� ���������

    public GameObject rocketPrefab; //������� ������ ������
    private GameObject tmpRocket; //����������, ������ ��� ����������� �������� ������� ������
    private Coroutine powerupCountdown; //�������� ������ ��� ��������� ��������� (� ��������� ��������������� �����)

    public float hangTime; //�����, ����� ���� ������ ��� ������ ��������� � ����������� ���������
    public float smashSpeed; //�������� ����� ��� ������ �����
    public float explosionForce; //���� ������ ��� ��������� ������ �����
    public float explosionRadius; //������, �� ������� ���������������� ����� ��������� ������ �����
    bool smashing = false; //�� ������� ���� ������ �� ����� ��������� ��������� ������
    float floorY; //��� ��������, ��� ��� ���������� ��������� ������ ���� ����� ��������� ������ �� ��� Y

    private Rigidbody playerRB; //������ ����
    private GameObject focalPoint; //����� ��� �������� ������ ��
    private float powerupStrength = 15.0f; //���-�� ������, �� ������� �������� ���� ��� ���������� � ���� ���������

    void Start()
    {
        playerRB = GetComponent<Rigidbody>(); //���������� (��������������) ������ ����
        focalPoint = GameObject.Find("FocalPoint"); //������� ������� ������ (GameObject.Find) � ������ FocalPoint
    }

    void Update()
    {
        if (transform.position.y < -3) //���� ���� ������ ������ �� ������� ����
        {
            transform.position = new Vector3(0, 0.42f, 0); //��������� ��� � �������� �������
        }

        forwardInput = Input.GetAxis("Vertical"); //������� �� ������� �����-����
        playerRB.AddForce(focalPoint.transform.forward * speed * forwardInput); //���� ����� ������������ �� ��� Z, ������� ������� � �������� ������� FocalPoint (focalPoint.transform.forward) �� ��������� speed ��� ������� �� ������� �����-���� (forwardInput)

        powerupIndicator.transform.position = transform.position; //������� �������� (���������� ��� ���������) ����� ������� �������, � �������� ��������� ���� ������. �� ���� �������� ����� ��������� �� ������ ������

        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.F)) //���� ������������ �������� ��������������� ����� � ������� ��������� � ����� ����� �� ������� F, ��:
        {
            LaunchRockets(); //����������� ������� � ��������� LaunchRockets
        }

        if (currentPowerUp == PowerUpType.Smash && Input.GetKeyDown(KeyCode.Space) && !smashing) //���� ����� ������ ��������� ������ (PowerUpType.Smash) � ����� �� ������ � �� � ������ ������ �� ���������� ��������� ������ (!smashing), ��:
        {
            smashing = true; //���������� ��������� ������
            StartCoroutine(Smash()); //����������� �������� (������������������ ��������) � ������ Smash
        }
    }

    void OnTriggerEnter(Collider other) //����� ������� �����������, ����� ��������� ������������ � ���������. � ������� �������� other ���������� ������ ��������� (Collider), ����� ��� ����, ���� ��� ����� ���� �������
    {
        if (other.CompareTag("Powerup")) //���� ������� ������, � ������� ���������� ���� ������, � ����� Powerup, ��:
        {
            hasPowerup = true; //���������� ��������� (����� ����� � ���������, ���� ���������� ������)
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType; //����������� � ���������� currentPowerUp ���������� powerUpType �� �������� ������� PowerUp (�� ������� � ���� ������)
            powerupIndicator.gameObject.SetActive(true); //��������� ������� ������ powerupIndicator (��������, ������������, ��� ���� ����� ��������� ���������)
            Destroy(other.gameObject); //������� ���� ������ ������� ������ (���������)

            if (powerupCountdown != null) //���� �������� ������ ����������, ��:
            {
                StopCoroutine(powerupCountdown); //���������� �������� ��������������� ������
            }
            powerupCountdown = StartCoroutine(PowerupCountdownRoutine()); //�������� ������� PowerupCountdownRoutine ��� ������ ���������� powerupCountdown
        }
    }

    private void OnCollisionEnter(Collision collision) //����� ������� �����������, ����� ������������ ��� ������ ���� (Rigidbody). � ������� �������� collision ���������� ������ ������ ����, ���� ������, ���� �� ������������
    {
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp==PowerUpType.Pushback) //���� ������, � �������� ��������� ������ (�����) ���������� � ������� �������� � ����� Enemy (� ������) � �� ���� ��������� (������� ���������==�������� ������������ (�� ���� � ���� ������� �� ����������� ������, ���� ��� �� ��������� ������)), ��:
        {
            Rigidbody enemyRigidbody = collision.gameObject.GetComponent<Rigidbody>(); //�������� ������ ���� (Rigidbody) �������� ������� (gameObject), � ������� ���� ������ ������������ (� collision)
            Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position); //�������� ��������� �����: ��� ������� (collision.gameObject.transform.position) ��������� �� ������� �������, � �������� ��������� ���� ������ (transform.position)
            enemyRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse); //��� ����������� ����, � ������� ������� ���� ��� ���������� ���������. awayFromPlayer - ����� ������� ����� ������� �� ���������� powerupStrength. ForceMode.Impulse - ��� ����� ������ ������.

            Debug.Log("Player collided with"+collision.gameObject.name+" with powerup set to "+currentPowerUp.ToString()); //��������� ��� ��������� � �������
        }
        if (collision.gameObject.CompareTag("EnemyPower")) //���� ������������ � �������� � ����� EnemyPower (� ������, � �������� ����� ���), ��:
        {
            Rigidbody playerRigidbody = GameObject.Find("Player").GetComponent<Rigidbody>(); //�������� ������ ���� (Rigidbody) �������� ������� (GameObject) � ������ Player
            Vector3 awayFromPlayer = (transform.position - collision.gameObject.transform.position); //�������� ��������� �������, � �������� ��������� ���� ������: ��� ������� (transform.position) ��������� �� ������� ������� collision (�� ���� �� �����)
            playerRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse); //��� ����������� ����, � ������� ������� ���� ������ ��� ������������ � ������������ ������. awayFromPlayer - ����� ������� ����� ������� �� ���������� powerupStrength. ForceMode.Impulse - ��� ����� ������ ������.
            Debug.Log("Collided with" + collision.gameObject.name + " with powerup set to" + hasPowerup); //��������� ��� ��������� � �������
        }
    }

    void LaunchRockets() //������� ��� ������� ��������������� �����
    {
        foreach(var enemy in FindObjectsOfType<Enemy>()) //foreach - ��� ����, ������� ����� ��� �������� ���������, ������������ � ��. var - ��� ����������. enemy - �������� ����������, ������� ���������� ������ (in) ������� FindObjectsOfType<Enemy>() 
            //���� ����������, ����� ����� ������������ ��� �������� ������� (����� ������ ������ ������� ����� �� �����)
        {
            tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity); //����� ��������������� �����. rocketPrefab - ������ ������. transform.position + Vector3.up - �� ���� ������ ������� ������� �������� ����� ����� ������ (���� �� ���������� ���). 
            //Quaternion.identity - ��� �������� 
            tmpRocket.GetComponent<RocketBehaviour>().Fire(enemy.transform); //���������� ������� ����� (enemy.transform) � ������� Fire, ������� ��������� � ������� RocketBehaviour, ����� ��������������� ������ (tmpRocket) �����, �� ���� �� ������
        }
    }

    IEnumerator PowerupCountdownRoutine() //IEnumerator - ������������������ ��������. �� ���� ������� ����������� ������ ������, ����� ������ � �.�., ��� ����������� �� ������������. PowerupCountdownRoutine - �������� �������.
                                          //� ����� ������� ���������� �������� ������ �������, ����� �������� ������ ����� ������������ ���������. 
    {
        yield return new WaitForSeconds(7); //yield - ��, ��� ��������� ��������� �������� ������ ����� �� ��������� Update. return - �������. new WaitForSeconds(7) - �������� ������, ������������ � 7 ������ (new ����� ��� ����, ����� ������ ��������� ����������. �� ���� ���� ������ 7 ������, �� ��� ��������� ��������� ������� ������ ����� ���������� ����� � 7 �������).
        hasPowerup = false; //��������� �����������
        currentPowerUp = PowerUpType.None; //���������� ������� ��������� �� ����, ��������� �������� None
        powerupIndicator.gameObject.SetActive(false); //����������� �������� ������ �����, ������������, ��� ��������� ��������
    }

    IEnumerator Smash() //IEnumerator - ������������������ ��������. �� ���� ������� ����������� ������ ������, ����� ������ � �.�., ��� ����������� �� ������������. Smash - �������� �������.
                                          //� ����� ������� ���������� ������������������ �������� ��� ��������� ������ ����� ������
    {
        var enemies = FindObjectsOfType<Enemy>(); //��������� ��� ������� Enemy
        floorY = transform.position.y; //������������ ������� ����� ������ �� ��� y
        float jumpTime = Time.time + hangTime; //����� ������=�����(��� ���������� � ����)+���������� hangTime

        while (Time.time < hangTime) //���� while. ���� hangTime ������ Time.time, ������ ���� ������ ��� ��������� � ������, � ��� � ��� ����� ����������� ������ ����.
        {
            playerRB.velocity = new Vector2(playerRB.transform.position.x, smashSpeed); //������ ��������� ����� ������. ������ ������ �� ���� X � Y, ������� ���������� Vector2. playerRB.transform.position.x - ������� �� ��� X ���� � ����� ������.
            //smashSpeed - ������� �� ��� Y ����� �������� ���������� smashSpeed
            yield return null; //���������� ����� �������, ��� ����� ���� ��������� ����� ������ �� ������������ ��������� �� �����
        }

        while (transform.position.y > floorY)
        {
            playerRB.velocity = new Vector2(playerRB.transform.position.x, -smashSpeed*2); //������ ��������� ����� ������. ������ ������ �� ���� X � Y, ������� ���������� Vector2. playerRB.transform.position.x - ������� �� ��� X ���� � ����� ������.
            //-smashSpeed*2 - ������� �� ��� Y: ���������� ����� ������ ����� ������ ������� �� �����. ���� smashSpeed - ��� ������ �����, �� -smashSpeed*2 - �����������
            yield return null; //���������� ����� �������
        }

        for(int i = 0; i < enemies.Length; i++) //���� for. ���������� ���������� i = 0. ���� ��� ������ �������� ������� ���������� �������� � ������� �� ������� Enemy (i < enemies.Length), �� ���������� ������� (i++)
        {
            if (enemies != null) //���� ������, � �������� ��������� ���� ������ (���� ������) ��������� � enemies, ��:
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse); //����������� �����.
                //enemies[i] - ���������, ��� �� ���� (� ��� ������ i, ��� ���� ��������� ����� ������), ���������� ������ ����, ���� ����� ���� �� ���� ������ (GetComponent<Rigidbody>())
                //AddExplosionForce - ������������ ���� � ������� ����, ������� ��������� ������ ������.
                //explosionForce - ���� ������. transform.position - ����� �������, ������ ������� ���������� �����. explosionRadius - ������, � �������� �������� ���������� �����. 
                //0.0f - �������� ��������� (������) ��� ������ (����������� �� 0, ������ ����������� ��� �� ����� �� ��� Y). ForceMode.Impulse - ��� ����� ������ ������
            }
        }

        smashing = false; //����� ���� ������������� �������� � ���������� ���������� ��������� ������ ������������
    }
}
