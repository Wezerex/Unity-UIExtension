namespace UiExtension
{
    public class UiExtManager : Singleton<UiExtManager>
    {
        public PointerEventData pointerEventData;
        private EventSystem eventSystem;
        public UiExtElement lastHoveredElement;
        public List<UiExtElement> activeElements = new List<UiExtElement>();

        private void Start()
        {
            eventSystem = EventSystem.current;
            if (pointerEventData == null)
            {
                pointerEventData = new PointerEventData(eventSystem);
            }
        }

        private void Update()
        {
            if (pointerEventData == null)
            {
                pointerEventData = new PointerEventData(eventSystem);
            }

            pointerEventData.position = Input.mousePosition;

            UiExtElement[] lastUiElements;

            if (lastHoveredElement != null)
            {
                lastUiElements = lastHoveredElement.GetComponents<UiExtElement>();
            }
            else
            {
                lastUiElements = null;
            }

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, raycastResults);

            if (raycastResults.Count > 0)
            {
                RaycastResult topResult = raycastResults[0];
                int rCount = raycastResults.Count;
                bool heresOne = false;
                for (int i = 0; i < rCount; i++)
                {
                    if (raycastResults[i].gameObject.GetComponent<UiExtElement>())
                    {
                        topResult = raycastResults[i];
                        heresOne = true;
                        break;
                    }
                }

                if (heresOne)
                {
                    UiExtElement uiElement = topResult.gameObject.GetComponent<UiExtElement>();
                    UiExtElement[] uiElements = uiElement.GetComponents<UiExtElement>();

                    if (uiElement != null)
                    {
                        if (uiElement != lastHoveredElement)
                        {
                            for (int i = 0; i < uiElements.Length; i++)
                            {
                                if (uiElements[i].onMouseEnter != null)
                                {
                                    uiElement.onMouseEnter.Invoke();
                                }
                            }
                        }
                    }

                    if (lastHoveredElement != null && lastHoveredElement != uiElement)
                    {
                        for (int i = 0; i < lastUiElements.Length; i++)
                        {
                            lastUiElements[i].onMouseExit?.Invoke();
                        }
                    }

                    for (int i = 0; i < uiElements.Length; i++)
                    {
                        HandleElementCalls(uiElements[i]);
                    }

                    lastHoveredElement = uiElement;
                }
                else
                {
                    if (lastHoveredElement != null)
                    {
                        for (int i = 0; i < lastUiElements.Length; i++)
                        {
                            lastUiElements[i].onMouseExit?.Invoke();
                        }

                        lastHoveredElement = null;
                    }
                }
            }
            else
            {
                if (lastHoveredElement != null)
                {
                    for (int i = 0; i < lastUiElements.Length; i++)
                    {
                        lastUiElements[i].onMouseExit?.Invoke();
                    }

                    lastHoveredElement = null;
                }
            }

            int count = activeElements.Count;
            for (int i = 0; i < count; i++)
            {
                HandleElementCalls(activeElements[i]);
            }
        }

        private void HandleElementCalls(UiExtElement element)
        {
            for (int i = 0; i <= 2; i++)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    element.onMouseClick?.Invoke(i, ClickType.Down);
                }
                else if (Input.GetMouseButton(i))
                {
                    element.onMouseClick?.Invoke(i, ClickType.Stay);
                }
                else if (Input.GetMouseButtonUp(i))
                {
                    element.onMouseClick?.Invoke(i, ClickType.Up);
                }
            }
        }
    }
}
