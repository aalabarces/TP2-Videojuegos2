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
    private Vector2 hotspotCrosshair;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        hotspotCrosshair = new Vector2(
            crosshairCursor.width / 2f,
            crosshairCursor.height / 2f
        );
        // SetDefaultCursor();
        SetCrosshairCursor();
    }
    // Había hecho un cursor animado muy lindo
    // Tenía una animación de disparo y sonido
    // Pero la verdad es que nunca sabías muy bien dónde hacías click
    // Así que la tuve que sacar :(
    // Pero dejo el código, para que vean qué lindo es
    // void Update()
    // {
    //     if (Input.GetMouseButtonDown(0) && !GameManager.Instance.isGameActive)
    //         {
    //             StartCoroutine(ActivateMouseClickAnimation());
    //         }
    // }

    public IEnumerator ActivateMouseClickAnimation()
    {
        AudioManager.Instance.PlaySound("FX_Fire");
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
