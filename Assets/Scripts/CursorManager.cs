using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private List<Texture2D> interactCursor;
    [SerializeField] private float animationSpeed = 0.05f;
    [SerializeField] private Texture2D crosshairCursor;
    [SerializeField] private Vector2 hotspotDefault;
    [SerializeField] private Vector2 hotspotCrosshair;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        SetDefaultCursor();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isGameActive)
            {
                StartCoroutine(ActivateMouseClickAnimation());
            }
    }

    public IEnumerator ActivateMouseClickAnimation()
    {
        for (int i = 0; i < interactCursor.Count; i++)
        {
            Cursor.SetCursor(interactCursor[i], hotspotDefault, CursorMode.Auto);
            yield return new WaitForSeconds(animationSpeed);
        }
        Cursor.SetCursor(defaultCursor, hotspotDefault, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, hotspotDefault, CursorMode.Auto);
    }

    public void SetCrosshairCursor()
    {
        Cursor.SetCursor(crosshairCursor, hotspotCrosshair, CursorMode.Auto);
    }
}
