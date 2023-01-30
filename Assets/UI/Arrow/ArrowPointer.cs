using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPointer : MonoBehaviour
{
    public Transform target;
    private float radius = 10;

    private RectTransform rectTransform;
    private Image arrowSprite;

    private void Awake()
    {
        arrowSprite = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();

        radius = FindObjectOfType(typeof(CanvasScaler)).GetComponent<CanvasScaler>().referenceResolution.y / 2.5f;
    }

    private void Start()
    {
        if (target == null)
            Hide();
    }

    private void Update()
    {
        if (target == null) return;
        
        if (arrowSprite.enabled == true)
        {
            Transform origin = GameManager.Camera.transform;
            Vector3 targetDir = (target.position - origin.position).normalized;
            targetDir = Vector3.ProjectOnPlane(targetDir, origin.forward).normalized;
            targetDir = origin.worldToLocalMatrix * targetDir;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, targetDir);
            
            rectTransform.anchoredPosition = new Vector2(targetDir.x, targetDir.y) * radius;
        }
    }

    public void Show() => arrowSprite.enabled = true;
    public void Hide() => arrowSprite.enabled = false;
}
