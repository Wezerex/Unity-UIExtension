namespace UiExtension
{
    public class DraggableElement : UiExtElement
    {
        public int mouseButton;
        public int dragId = 0;
        Vector2 offset;
        Vector2 startPos;

        public UnityEvent onClick;
        public UnityEvent onDrag;
        public UnityEvent onUnclick;

        [HideInInspector]
        public DropZone myDropZone;

        private void Start()
        {
            onMouseClick += CheckClickInfo;
            if (GetComponentInParent<DropZone>())
            {
                myDropZone = GetComponentInParent<DropZone>();
            }
        }

        public void CheckClickInfo(int _mouseButton, ClickType _clickType)
        {
            if (_mouseButton == mouseButton)
            {
                if (_clickType == ClickType.Down)
                {
                    StartDrag();
                }
                else if (_clickType == ClickType.Stay)
                {
                    UpdateDrag();
                }
                else if (_clickType == ClickType.Up)
                {
                    EndDrag();
                }
            }
        }

        public void StartDrag()
        {
            onClick.Invoke();
            startPos = transform.localPosition;
            offset = (Vector2)transform.position - UiExtManager.Instance.pointerEventData.position;
            isActived = true;
            SetRaycastTarget(false);
        }

        public void UpdateDrag()
        {
            if (isActived)
            {
                onDrag.Invoke();
                transform.position = UiExtManager.Instance.pointerEventData.position + offset;
            }
        }

        public void EndDrag()
        {
            onUnclick.Invoke();
            if (dragId != 0)
            {
                if (UiExtManager.Instance.lastHoveredElement)
                {
                    if (UiExtManager.Instance.lastHoveredElement.GetComponent<DropZone>())
                    {
                        DropZone dropZone = UiExtManager.Instance.lastHoveredElement as DropZone;

                        if (dropZone.dragId == dragId)
                        {
                            dropZone.HandleDrop(this);
                        }
                    }
                    else if (UiExtManager.Instance.lastHoveredElement.GetComponent<DraggableElement>())
                    {
                        DraggableElement draggableElement = UiExtManager.Instance.lastHoveredElement as DraggableElement;

                        if(draggableElement.myDropZone)
                        {
                            if(draggableElement.dragId == dragId && draggableElement.myDropZone.reposition)
                            {
                                draggableElement.SwitchPlaces(this);
                            }
                        }
                    }
                    else
                    {
                        transform.localPosition = startPos;
                    }
                }
            }

            
            isActived = false;
            SetRaycastTarget(true);
        }

        public void SwitchPlaces(DraggableElement other)
        {
            Transform his = other.transform.parent;
            Vector3 tempPosition = other.startPos;

            other.transform.SetParent(transform.parent);
            other.transform.localPosition = transform.localPosition;
            other.startPos = transform.position;

            transform.SetParent(his);
            transform.localPosition = tempPosition;
            startPos = tempPosition;

            if (myDropZone)
            {
                myDropZone.HandleDrop(this);
                myDropZone.HandleDrop(other);
            }
        }

        private void SetRaycastTarget(bool state)
        {
            foreach (Graphic graphic in GetComponentsInChildren<Graphic>())
            {
                graphic.raycastTarget = state;
            }
        }
    }
}
