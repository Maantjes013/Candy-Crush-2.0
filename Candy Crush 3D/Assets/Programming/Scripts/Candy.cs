using UnityEngine;

public class Candy : MonoBehaviour
{
    public static bool canConnect = false;
    private bool isDragging = false;
    private Renderer candyRenderer;

    public Vector2 currentCoordinates;
    public CandyType candyType;

    private GridFactory gridFactory;

    private Color currentColor;
    private const string colorField = "_BaseColor";

    private Vector2 initialMousePosition;
    private float swipeThreshold = 50f;

    // Start is called before the first frame update
    private void Awake()
    {
        candyRenderer = transform.GetComponent<Renderer>();
        gridFactory = FindAnyObjectByType<GridFactory>();
    }

    public void OnCandyTypeChanged()
    {
        Color parsedColor;
        if (ColorUtility.TryParseHtmlString(candyType.ToString(), out parsedColor))
        {
            currentColor = parsedColor;
            Debug.Log($"Color set to {parsedColor}");
        }

        if (candyRenderer != null)
        {
            candyRenderer.material.SetColor(colorField, currentColor);
        }
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
            gridFactory.CheckCandySwap(this, swapDirection);
        }

        initialMousePosition.Set(0, 0);
    }

    private Vector2 GetDragDirection(Vector2 dragDirection)
    {
        // Determine the direction based on the larger axis (X or Y)
        if (Mathf.Abs(dragDirection.x) > Mathf.Abs(dragDirection.y))
        {
            // Horizontal swipe (left or right)
            if (dragDirection.x > 0)
                return Vector2.right;
            else
                return Vector2.left;
        }
        else
        {
            // Vertical swipe (up or down)
            if (dragDirection.y > 0)
                return Vector2.up;
            else
                return Vector2.down;
        }
    }

    //private void ActivateEffect(int effectIndex)
    //{
    //    Effect eff = EffectCatalogue.Instance.GetEffect(effectIndex);
    //    eff.gameObject.SetActive(true);
    //    eff.ActivateEffect();
    //}

    private void DestroyCandy()
    {

    }

    //private void SpawnDestroyParticles()
    //{
    //    GameObject particle = Instantiate(destroyedParticle, transform.position, Quaternion.identity);
    //    particle.GetComponent<Renderer>().material.SetColor("_BaseColor", currentColor);
    //    FindObjectOfType<SoundManager>().PlayBlockBreak(particle);
    //}
}
