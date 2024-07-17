using System;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameObject _background;

    // Радиус возможного обзора камеры
    private float _spaceCircleRadius;

    // Исходные размеры объекта фона
    private float _backgroundOriginalSizeX;
    private float _backgroundOriginalSizeY;

    private void Awake() {
        _spaceCircleRadius = 0f;
        _backgroundOriginalSizeX = 0f;
        _backgroundOriginalSizeY = 0f;
    }

    void Start()
    {        
        // Исходные размеры фона
        SpriteRenderer sr = _background.GetComponent<SpriteRenderer>();
        var originalSize = sr.size;
        _backgroundOriginalSizeX = originalSize.x * 3f;
        _backgroundOriginalSizeY = originalSize.y * 3f;

        // Высота камеры равна ортографическому размеру
        float orthographicSize = _mainCamera.orthographicSize;
        // Ширина камеры равна ортографическому размеру, помноженному на соотношение сторон
        float screenAspect = (float)Screen.width / (float)Screen.height;
        // Радиус окружности, описывающей камеру
        _spaceCircleRadius = Mathf.Sqrt(orthographicSize * screenAspect * orthographicSize * screenAspect + orthographicSize * orthographicSize);

        // Конечный размер фона должен позволять сдвинуться на один базовый размер фона в любом направлении + перекрыть радиус камеры также во всех направлениях
        sr.size = new Vector2(_spaceCircleRadius * 2 + _backgroundOriginalSizeX * 2, _spaceCircleRadius * 2 + _backgroundOriginalSizeY * 2);
    }

    void Update()
    {
        float differenceX = _background.transform.position.x - _mainCamera.transform.position.x;
        float differenceY = _background.transform.position.y - _mainCamera.transform.position.y;

        if (differenceX >= _backgroundOriginalSizeX)
        {
            _background.transform.Translate(-_backgroundOriginalSizeX, 0, 0);
        }
        if (differenceX <= -_backgroundOriginalSizeX)
        {
            _background.transform.Translate(_backgroundOriginalSizeX, 0, 0);
        }
        if (differenceY >= _backgroundOriginalSizeY)
        {
            _background.transform.Translate(0, -_backgroundOriginalSizeY, 0);
        }
        if (differenceY <= -_backgroundOriginalSizeY)
        {
            _background.transform.Translate(0, _backgroundOriginalSizeY, 0);
        }
    }
}