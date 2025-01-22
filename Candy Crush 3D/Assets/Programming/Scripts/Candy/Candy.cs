using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Candy : MonoBehaviour
{
    public DecalProjector specialTypeProjector;

    private bool isDragging = false;
    private Renderer candyRenderer;

    private Vector2 _currentCoordinates;

    public Vector2 CurrentCoordinates
    {
        get => _currentCoordinates;
        set
        {
            _currentCoordinates = value;
            name = "Candy " + _currentCoordinates;
        }
    }
    private CandyType _candyType;

    public CandyType CandyType
    {
        get => _candyType;
        set
        {
            _candyType = value;
            OnCandyTypeChanged();
        }
    }

    private GridManager gridManager;
    public SpecialCandyManager specialCandyManager;

    private Color currentColor;
    private const string colorField = "_BaseColor";

    private Vector2 initialMousePosition;
    private float swipeThreshold = 50f;

    public SpecialCandyType specialCandyType;


    private void Awake()
    {
        candyRenderer = transform.GetComponent<Renderer>();
        specialTypeProjector.gameObject.SetActive(false);
    }

    private void Start()
    {
        gridManager = GridManager.Instance;
        specialCandyManager = SpecialCandyManager.Instance;
    }

    /// <summary>
    /// Change color automatically if candy type changes
    /// </summary>
    public void OnCandyTypeChanged()
    {
        if (ColorUtility.TryParseHtmlString(_candyType.ToString(), out Color parsedColor))
            currentColor = parsedColor;

        if (candyRenderer != null)
            candyRenderer.material.SetColor(colorField, currentColor);
    }

    /// <summary>
    /// Change special candy type of this candy and show decal with corresponding material
    /// </summary>
    /// <param name="material"></param>
    /// <param name="newSpecialCandyType"></param>
    public void ChangeSpecialCandyType(SpecialCandyType newSpecialCandyType)
    {       
        specialTypeProjector.gameObject.SetActive(true);
        specialTypeProjector.material = specialCandyManager.GetDecalMaterial(newSpecialCandyType);
        specialCandyType = newSpecialCandyType;
    }

    private void OnMouseDown()
    {
        initialMousePosition = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        if (!isDragging)
            return;

        Vector2 currentMousePosition = Input.mousePosition;
        Vector2 dragDirection = currentMousePosition - initialMousePosition;
        Vector2 swapDirection = GetDragDirection(dragDirection);

        if (Mathf.Abs(dragDirection.x) > swipeThreshold || Mathf.Abs(dragDirection.y) > swipeThreshold)
        {
            gridManager.CheckCandySwap(this, swapDirection);
        }
    }

    /// <summary>
    /// Determine drag direction of the player
    /// </summary>
    /// <param name="dragDirection"></param>
    /// <returns>Drag direction</returns>
    private Vector2 GetDragDirection(Vector2 dragDirection)
    {
        if (Mathf.Abs(dragDirection.x) > Mathf.Abs(dragDirection.y))
        {
            if (dragDirection.x > 0)
                return Vector2.right;
            else
                return Vector2.left;
        }

        if (dragDirection.y > 0)
            return Vector2.up;
        else
            return Vector2.down;
    }

    //private void SpawnDestroyParticles()
    //{
    //    GameObject particle = Instantiate(destroyedParticle, transform.position, Quaternion.identity);
    //    particle.GetComponent<Renderer>().material.SetColor("_BaseColor", currentColor);
    //}
}
