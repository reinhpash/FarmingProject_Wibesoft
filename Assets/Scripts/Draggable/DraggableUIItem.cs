using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector3 InitialPosition;
    Image image;
    [SerializeField] Transform parentAfterDrag;


    private void Start()
    {
        image = GetComponent<Image>();

        InitialPosition = this.transform.localPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CropManager.Instance.IsOnHarvestingMode = true;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CropManager.Instance.IsOnHarvestingMode = false;
        transform.SetParent(parentAfterDrag);
        this.transform.localPosition = InitialPosition;
        image.raycastTarget = true;
    }
}
