using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { None,Pushback,Rockets,Smash } //enum делает выпадающее меню для переключения между значениями перечисления
                                                  //(в данном случае между None - никакое действие,Pushback - отталкивание врага (то, что делает кружок powerup при его активации),
                                                  //Rockets - запуск самонаводящихся ракет во врага), Smash - прыжок перса игрока, который отталкивает близнаходящихся врагов

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
}
