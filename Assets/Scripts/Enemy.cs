using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject player; //����� � ���������� ������ ���� ������ (����� ��� ����, ����� ���� ����� �������� ����������� � ��� �������)
    private Rigidbody enemyRB; //���������� ������ ����
    public float speed; //�������� ����������� �����

    public bool isBoss = false; //�� ������� ���� �� ���������� ����� ��
    public float spawnInterval; //��������, ������ ������� ���������� ����
    private float nextSpawn; //����������� ������
    public int miniEnemySpawnCount; //��� ������� ����-�����, ������� ������ ����
    private SpawnManager spawnManager; //���������� ������ � ������ SpawnManager ����� ���������� spawnManager

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>(); //���������� ��������� ������� ����
        player = GameObject.Find("Player"); //���������� ������� ������ � ��������� Player

        if (isBoss) //���� �� ����� �������� ����, ��:
        {
            spawnManager = FindObjectOfType<SpawnManager>(); //��������� ��� ������� SpawnManager (������+������ � ��������)
        }
    }

    void Update()
    {
        Vector3 lookDirection = (player.transform.position - transform.position).normalized; //player.transform.position - ������� ������� ������,
        //transform.position - ������� ������� �����. �������� �� ������� ������ ������� �����, �� ���� ���������� ���� ������ ���� ������� � ������������ � ������.
        //normalized - ����� ��� ����������� �������� �����. ���� �� ���������, �� ��� ������� ���������� ����� ������ �� ����� � ��������� (player.transform.position - transform.position) ����� ������� ������� �������� � ��� ��������� �� �������� ������ ���, ��� ���� ����� ������� �� ����� ������ ��� �����������. � normalized �������������, ��� ���� ����� ��������� � ����� � ��� �� ��������� �� ������ ����������.
        enemyRB.AddForce(lookDirection * speed * Time.deltaTime); //���� �������� � ������. lookDirection - �����������, � ������� ���� �������� (��.���������� ���� ����). speed - ��������, � ������� �������� ����

        if (transform.position.y < -3) //���� ������� ����� ������ -3, ��:
        {
            Destroy(gameObject); //������� ���
        }

        if (isBoss) //���� ������ �� ����� ���� ����, ��:
        {
            if (Time.time > nextSpawn) //���� ������ ������� ������� (0) ������, ��� �����, ������ ��� ���������� ������, ��:
            {
                nextSpawn = Time.time + spawnInterval; //�����, ������ ��� ���������� ������ �������� = 0 + ����������� �������� �� ���������� spawnInterval
                spawnManager.SpawnMiniEnemy(miniEnemySpawnCount); //���������� ������� � ��������� SpawnMiniEnemy �� ������� SpawnManager, � ������ ������� miniEnemySpawnCount
            }
        }
    }

}
