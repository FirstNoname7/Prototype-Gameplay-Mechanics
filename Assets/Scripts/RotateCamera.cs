using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed=40.0f; //скорость вращения вокруг FocalPoint
    private float horizontalInput; //переменная для ввода с клавиатуры

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal"); //ввод с клавиатуры стрелками вправо-влево
        transform.Rotate(Vector3.down, horizontalInput * Time.deltaTime * rotationSpeed); //вращение камеры вокруг FocalPoint. Vector3.up - вращение по оси Y, дальше идёт то, что влияет на эту ось:
        //она будет вращаться со скоростью rotationSpeed если игрок нажмёт на кнопки вправо-влево (horizontalInput), вращаться будет одинаково на всех устройствах (Time.deltaTime)
    }
}
