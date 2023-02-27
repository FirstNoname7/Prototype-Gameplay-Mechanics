using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    //� ���� ������� ���������� ������ ��������������� �����, �������� �������� ���� ������ �� ������
    private Transform target; //��� ����� ����������� ������� ����, �� ������� ����� ���������� ��������������� ������ (������� ������)
    private float speed = 15.0f; //�������� ��������������� ������
    private bool homing; //��������, ������������ �� ��������������� ������

    private float rocketStrength = 15.0f; //���� ����� ������
    private float aliveTimer = 1.0f; //���-�� ������, ����� ������ ����� ���������� � ���� ����� ����, ��� ������� �� ������� ������

    void Update()
    {
        if (homing && target != null) //���� ������������ ��������������� ������ (homing) � �� ����� ���� �������, �� ������� ����� ������ ������ (�� ���� �����), ��:
        {
            Vector3 moveDirection = (target.transform.position - transform.position).normalized; //��������� ���������� ����� ������ (target.transform.position) � ������� (��������, � �������� ��������� ���� ������) (transform.position). �� � ��� ���������� �������� ������ ��� ������ normalized
            transform.position += moveDirection * speed * Time.deltaTime; //������� ������ �������� � ������ ����������� moveDirection � �������� speed. �� ��� ��� ����� ��������� �������� �� ���� ����������� (Time.deltaTime)
            transform.LookAt(target); //������ �������������� (transform.LookAt) ����� � ���� target (�����), ����� ��������
        }
    }

    public void Fire(Transform newTarget) //������� ��� ������� �����
    {
        target = newTarget; //�� ���� ��������� ��������������� ����� ����� ���������� � ������� ����, �� ������� ��� ����� ���������� (� ������)
        homing = true; //������ ������
        Destroy(gameObject,aliveTimer); //������� ������� ������ ������ aliveTimer ������
    }

    void OnCollisionEnter(Collision col) //����� ������� �����������, ����� ������������ ��� ������ ���� (Rigidbody). � ������� �������� col ���������� ������ ������ ����, ���� ������, ���� �� ������������ � ���
    {
        if (target != null) //���� ������, � �������� ��������� ���� ������ (������), ����������� � �������� target (� ������)
        {
            if (col.gameObject.CompareTag(target.tag)) //���� ������������ ��������� � ������� �������� � �����, ������� ��������� � ������� target, ��:
            {
                Rigidbody targetRigidbody = col.gameObject.GetComponent<Rigidbody>(); //���������� ������ ���� ������� (�����), � ������� ���������� ������, � �������� ��������� ���� ������ (������)
                Vector3 away = -col.contacts[0].normal; //���������� �����������, ���� ������� ������. ��� ������� � col, �� ���� � �����. ����� ��������������� (contacts) ����� ����, �� ���� ����� ���������� ����� ������� � ������ = 0, ������ ��� ����������� (������ ����� ��������������� �������). normal ���� ��� ������������ ��������.
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse); //��������� ���� � targetRigidbody (� �����). ����������� (away) ���������� �� ���� ����� ������ (rocketStrength). ������ ����������, ���� ��� �������� ����������, ���� ������ �� ��������� ����
                Destroy(gameObject); //������� ������� ������, ������������ � ����� ������� (������)
            }
        }
    }
}
