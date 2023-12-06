using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionService : MonoBehaviour
{

    void Start()
    {
    }

        
    private void Update()
    {
        float objectWidth = 0.5f;
        float objectHeight = 0.5f; // Размер объекта по высоте

        // Получаем координаты объекта в мировых координатах
        Vector3 objectWorldPosition = transform.position;

        // Задаем координаты для правого нижнего угла экрана
        float newX = Screen.width - objectWidth / 2;
        float newY = objectHeight / 2;

        // Устанавливаем новые мировые координаты объекта
        var pos = Camera.main.ScreenToWorldPoint(new Vector3(newX, newY, objectWorldPosition.z));
        transform.position = new Vector3(pos.x, pos.y, 0);
    }
}