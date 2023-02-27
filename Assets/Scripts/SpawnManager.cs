using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab; //��� ������ ������ ������� �������� ��� ������
    public GameObject[] powerupPrefabs; //��� ������ ������ ������� �������� ���������
    public int enemyCount; //���������� �������� ������ �� �����
    public int waveNumber = 1; //�� ������� ������ ������� ����� (��� � �������� ������ ����� �����)

    public GameObject bossPrefab; //��� ������ ������� ������ �����
    public GameObject[] miniEnemyPrefabs; //��� ������ ������ ����-������, ������� ��������� ����
    public int bossRound; //����� ������� ������� �������� ����

    private float spawnRange = 5.0f; //���������� ��� ����������� ��������� ������� ��� ��������� �����

    void Start()
    {
        int randomPowerup = Random.Range(0, powerupPrefabs.Length); //��������� ������ ��� ������� ���������. �� ���� �� ���������� �� ����� �������� � �������.
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation); //����� �������� ���������: powerupPrefabs[randomPowerup] - ������[��� ���������� ����� � �������],
                                                                                                                               //GenerateSpawnPosition() - ������� ������� �������� ����� ������� � ����� ������, 
                                                                                                                               //powerupPrefabs[randomPowerup].transform.rotation - �������� ������� ������ � ������ �������
        SpawnEnemyWave(waveNumber); //� ������� ������� �������� ��� ��������� enemiesToSpawn (�� ���� enemiesToSpawn = waveNumber). �� ���� ��� ������ ����� 1 ����.

    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length; //���������� enemyCount ����� ���������� ������� �������� � ������ Enemy. ���� �� ����� ���� ����, �� enemyCount = 1.

        if (enemyCount == 0) //���� ���������� ����� 0, �� ���� ��� ����� ������� � ���� ���, ��:
        {
            waveNumber++; //����������� waveNumber �� 1 (�� ���� ��� ������ ����� 1 ����, ����� �� �����, �� �������� 2 �����, ����� 2 ����� ������, �� �������� 3 ����� � �.�.)
            if (waveNumber % bossRound == 0) //���� ������� �� ������� ���-�� ������ �� ���-�� �������, ���������� �� ��������� �����, ����� ����, ������ ������ ���� �������� �����:
            {
                SpawnBossWave(waveNumber); //���������� ������� � ������ SpawnBossWave � ����� �������� waveNumber (�� ���� ������� waveNumber ������)
            }
            else //� �������� ������:
            {
                SpawnEnemyWave(waveNumber); //���������� ������� � ������ SpawnEnemyWave � ����� �������� waveNumber (�� ���� ������� waveNumber ������)
            }
            int randomPowerup = Random.Range(0, powerupPrefabs.Length); //��������� ������ ��� ������� ���������. �� ���� �� ���������� �� ����� �������� � �������.
            Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation); //������ ����� ��� ��������� ����� Instantiate(�����������)
                                                                                                                                   //powerupPrefab[randomPowerup] - ������[��� ���������� ����� � �������], ������� ��������� (���������)
                                                                                                                                   //GenerateSpawnPosition() - ������� ��������� �������� ����� ������� � ����� ������ 
                                                                                                                                   //powerupPrefab[randomPowerup].transform.rotation - �������� ������ � �������[��� ���������� ����� � �������], ������� ���������.

        }

    }

    private Vector3 GenerateSpawnPosition()
    {
        float x = Random.Range(-spawnRange, spawnRange); //��������� ������� �� ��� �
        float z = Random.Range(0, spawnRange); //��������� ������� �� ��� Z
        Vector3 randomPos = new Vector3(x, 0.6f, z); //new Vector3(x, 0, z) - ����� ������, �� ��� X = ���������� X, �� ��� Y = 0, �� ��� Z = ���������� z.
        return randomPos; //���� ���-�� � ������ ������� ��������� �� ��� ������� (GenerateSpawnPosition), �� ����� ������ ���������� randomPos, ���� �������� �� �����                                                                                                                                                                                                                                                                                   
    }

    void SpawnEnemyWave(int enemiesToSpawn) //� ���� ������� ���������� ����� ������
    {

        for(int i = 0; i < enemiesToSpawn; i++) //� ����� ���������� i ���������� ����� ���� (int i = 0;). ���� ��� ������ �������� ���������� enemiesToSpawn (i < enemiesToSpawn;), �� � i ������������ ������� (i++). ��� ������ i ����� ������ ���������� enemiesToSpawn (i > enemiesToSpawn), ���� �����������.
                                                //���� ������� �� ������ ����� ����� ������: ������ �� ������ enemiesToSpawn ������, ���� ���� ������ �� ��������
        {
            int enemyIndex = Random.Range(0, enemyPrefab.Length);

            Instantiate(enemyPrefab[enemyIndex], GenerateSpawnPosition(), enemyPrefab[enemyIndex].transform.rotation); //������ ����� ����� Instantiate(�����������)
                                                                                               //enemyPrefab - ������, ������� ��������� (�����)
                                                                                               //GenerateSpawnPosition() - ������� ����� �������� ����� ������� � ����� ������ 
                                                                                               //enemyPrefab.transform.rotation - �������� ������ � �������, ������� ���������.

        }
    }

    void SpawnBossWave(int currentRound) //������� ��� ������ ������. currentRound - ������� ��������, ���� �� ����
    {
        int miniEnemysToSpawn; //���-�� ����-������ �� �����, ����� ���������� ����
        if (bossRound != 0) //���� ���-�� �������, �� ��������� ������� �������� ����, �� ����� 0 (�� ���� ���� �� �� �����), ��:
        {
            miniEnemysToSpawn = currentRound / bossRound; //����� ������� ����� �� ���-�� �������, ������ ������� �������� ����
        }
        else //� �������� ������ (���� ���� �� �����):
        {
            miniEnemysToSpawn = 1; //��������� 1 ����-����
        }

        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation); //����� ������. ���������� ������ bossPrefab � �������� �� ������� GenerateSpawnPosition() � ����� ��������� bossPrefab.transform.rotation
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn; //����-�����, ������������ � ������� Enemy (miniEnemySpawnCount), ������� � ���������� miniEnemysToSpawn, ��������� � ���� �������, ����� ������������ ����� ��� ��������
    }

    public void SpawnMiniEnemy(int amount) //������� ��� ������ ����-������. amount - ����� ��� �����
    {
        for(int i = 0; i < amount; i++) //���������� ���������� i ����� 0. ���� ��� ������ amount, �� � ��� ����������� ������� (i++) � ���� ���.
        {
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length); //�����������: ����� ��������� ��������� ����-�����
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation); //����� ����-������:
            //miniEnemyPrefabs[randomMini] - ������[������ �������], GenerateSpawnPosition() - ������� �� ������� � ����� ���������, miniEnemyPrefabs[randomMini].transform.rotation - ������� �������� �������
        }
    }
}
