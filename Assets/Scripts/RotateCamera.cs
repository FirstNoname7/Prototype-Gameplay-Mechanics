using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed=40.0f; //�������� �������� ������ FocalPoint
    private float horizontalInput; //���������� ��� ����� � ����������

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); //���� � ���������� ��������� ������-�����
        transform.Rotate(Vector3.down, horizontalInput * Time.deltaTime * rotationSpeed); //�������� ������ ������ FocalPoint. Vector3.up - �������� �� ��� Y, ������ ��� ��, ��� ������ �� ��� ���:
        //��� ����� ��������� �� ��������� rotationSpeed ���� ����� ����� �� ������ ������-����� (horizontalInput), ��������� ����� ��������� �� ���� ����������� (Time.deltaTime)
    }
}
