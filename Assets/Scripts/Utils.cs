using System;
using Unity.VisualScripting;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Instance { get; private set; }
    public float leftLimit = 0f;
    public float rightLimit = 0f;
    public float topLimit = 0f;
    public float bottomLimit = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        SetLimits();
    }
    public void SetLimits()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        leftLimit = cam.transform.position.x - width / 2f;
        rightLimit = cam.transform.position.x + width / 2f;
        bottomLimit = cam.transform.position.y - height / 2f;
        topLimit = cam.transform.position.y + height / 2f;
    }

    public bool IsOutOfBounds(Vector2 position, float extraMargin = 0f, Vector2 direction = new Vector2())
    {
        if (direction != Vector2.zero)
        {
            // if direction, position.y doesn't matter
            return position.x < leftLimit - extraMargin && direction.x < 0 ||
                   position.x > rightLimit + extraMargin && direction.x > 0;
        }
        else return position.x < leftLimit - extraMargin || position.x > rightLimit + extraMargin ||
               position.y < bottomLimit - extraMargin || position.y > topLimit + extraMargin;
    }

    public Vector2 GetClampedPosition(Vector2 position)
    {
        float clampedX = Mathf.Clamp(position.x, leftLimit, rightLimit);
        float clampedY = Mathf.Clamp(position.y, bottomLimit, topLimit);

        return new Vector2(clampedX, clampedY);
    }

    public void ChangeLayerTo(GameObject gameObject, string newLayer)
    {
        gameObject.layer = LayerMask.NameToLayer(newLayer);
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(newLayer);
        }
    }
}
