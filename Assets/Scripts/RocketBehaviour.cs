using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    //в этом скрипте содержится логика самонаводящихся ракет, которыми пуляется перс игрока во врагов
    private Transform target; //тут будет содержаться позиция цели, на которую будет направлена самонаводящаяся ракета (позиция врагов)
    private float speed = 15.0f; //скорость самонаводящейся ракеты
    private bool homing; //проверка, используются ли самонаводящиеся ракеты

    private float rocketStrength = 15.0f; //сила удара ракеты
    private float aliveTimer = 1.0f; //кол-во секунд, когда снаряд будет находиться в игре после того, как вылетел за пределы экрана

    void Update()
    {
        if (homing && target != null) //если используются самонаводящиеся ракеты (homing) и на карте есть объекты, на которые могут лететь ракеты (то есть враги), то:
        {
            Vector3 moveDirection = (target.transform.position - transform.position).normalized; //уменьшаем расстояние между врагом (target.transform.position) и ракетой (объектом, к которому прикреплён этот скрипт) (transform.position). Ну и ещё сглаживаем движение ракеты при помощи normalized
            transform.position += moveDirection * speed * Time.deltaTime; //позиция ракеты меняется с учётом направления moveDirection и скорости speed. Ну ещё она будет одинаково меняться на всех устройствах (Time.deltaTime)
            transform.LookAt(target); //ракета поворачивается (transform.LookAt) лицом к цели target (врагу), когда вылетает
        }
    }

    public void Fire(Transform newTarget) //функция для пуляния ракет
    {
        target = newTarget; //то есть положение самонаводящихся ракет будет стремиться к позиции цели, на которую они будут направлены (к врагам)
        homing = true; //пуляем ракеты
        Destroy(gameObject,aliveTimer); //удаляем игровой объект спустя aliveTimer секунд
    }

    void OnCollisionEnter(Collision col) //такая функция выполняется, когда сталкиваются два жёстких тела (Rigidbody). В скобках параметр col обозначает другое жёсткое тело, чтоб понять, было ли столкновение с ним
    {
        if (target != null) //если объект, к которому прикреплён этот скрипт (ракета), столкнулась с позицией target (с врагом)
        {
            if (col.gameObject.CompareTag(target.tag)) //если столкновение произошло с игровым объектом с тегом, который прикреплён к объекту target, то:
            {
                Rigidbody targetRigidbody = col.gameObject.GetComponent<Rigidbody>(); //подключаем жёсткое тело объекта (врага), с которым столкнулся объект, к которому прикреплён этот скрипт (ракета)
                Vector3 away = -col.contacts[0].normal; //определяем направление, куда полетит ракета. Она полетит к col, то есть к врагу. Точка соприкосновения (contacts) равно нулю, то есть когда расстояние между ракетой и врагом = 0, значит они столкнулись (значит точка соприкосновения активна). normal надо для нормализации значений.
                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse); //применяем силу к targetRigidbody (к врагу). направление (away) умножается на силу удара ракеты (rocketStrength). Именно умножается, чтоб это работало непрерывно, пока ракета не достигнет цели
                Destroy(gameObject); //удаляем игровой объект, прикреплённый к этому скрипту (ракету)
            }
        }
    }
}
