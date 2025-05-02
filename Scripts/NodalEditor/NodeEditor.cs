namespace UiExtension.NodalEditor
{
    public class NodeEditor : MonoBehaviour
    {
        public ProceduralGridBackground grid;
        public UiExtElement backGroundExtElement;

        public GameObject contextMenu;
        public Transform contextMenuScrollContent;
        public GameObject contextMenuButtonPrefab;
        public Vector2 contextMenuOffset;

        public GameObject[] nodesPrefabs;
        public Transform nodeParent;
        public GameObject connectionPrefab;
        public Transform connectionParent;
        public Transform cursor;

        [HideInInspector]
        public List<Node> nodes = new List<Node>();
        [HideInInspector]
        public List<Connection> connections = new List<Connection>();
        [HideInInspector]
        public List<Node> selectedNodes = new List<Node>();
        [HideInInspector]
        public Connection selectedConnection;

        public int scrollMouseButton = 2;
        public int contextMenuButton = 1;
        public KeyCode destroyButton;

        private Vector2 lastMousePosition;

        [HideInInspector]
        public GameObject connectionIncreate;

        Vector2 contextMenuClickPosition;

        public float zoomSpeed = 0.1f;
        public float minZoom = 0.5f;
        public float maxZoom = 2.0f;

        public float currentZoom = 1.0f;

        private void Awake()
        {
            backGroundExtElement.onMouseClick += OnMouseClick;
        }

        private void Start()
        {
            InitializeContextMenu();
            contextMenu.SetActive(false);
        }

        public void InitializeContextMenu()
        {
            for (int i = 0; i < nodesPrefabs.Length; i++)
            {
                ExtButton tempButton = Instantiate(contextMenuButtonPrefab, contextMenuScrollContent).GetComponent<ExtButton>();
                tempButton.GetComponentInChildren<Text>().text = nodesPrefabs[i].name;

                int capturedIndex = i;
                tempButton.onClick.AddListener(() => CreateNodeFromContextMenu(capturedIndex));
            }
        }

        private void Update()
        {
            HandleZoom();
            cursor.position = UiExtManager.Instance.pointerEventData.position;
            if (Input.GetKeyDown(destroyButton))
            {
                if (selectedNodes.Count > 0)
                {
                    DestroySelected();
                }
                if (selectedConnection)
                {
                    selectedConnection.DestroyConnection();
                }
            }
        }

        private void HandleZoom()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                float zoomDelta = Input.mouseScrollDelta.y * zoomSpeed;
                float newZoom = Mathf.Clamp(currentZoom + zoomDelta, minZoom, maxZoom);

                if (Mathf.Approximately(newZoom, currentZoom))
                    return;

                ZoomToMousePosition(newZoom);
                currentZoom = newZoom;
            }
        }

        private void ZoomToMousePosition(float newZoom)
        {
            Vector2 localCursor;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(grid.GetComponent<RectTransform>(), Input.mousePosition, null, out localCursor);

            Vector3 previousScale = nodeParent.localScale;
            nodeParent.localScale = Vector3.one * newZoom;

            Vector3 scaleDelta = nodeParent.localScale - previousScale;

            Vector3 contentPosition = nodeParent.localPosition;
            nodeParent.localPosition = contentPosition - (Vector3)(localCursor * scaleDelta.x);
        }

        public void OnMouseClick(int mouseButton, ClickType clickType)
        {
            if (clickType == ClickType.Up && selectedConnection)
            {
                selectedConnection.UnSelectConnection();
            }

            if (clickType == ClickType.Up && selectedNodes.Count > 0)
            {
                ClearSelectedNodes();
                return;
            }

            if (clickType == ClickType.Up && connectionIncreate)
            {
                Destroy(connectionIncreate);
                return;
            }

            if (scrollMouseButton == mouseButton)
            {
                if (clickType == ClickType.Down)
                {
                    lastMousePosition = UiExtManager.Instance.pointerEventData.position;
                }
                else if (clickType == ClickType.Stay)
                {
                    MooveEditor();
                }
            }
            else if (contextMenuButton == mouseButton)
            {
                if (clickType == ClickType.Up)
                {
                    if (!contextMenu.activeSelf)
                    {
                        DisplayContextMenu(true);
                    }
                    else
                    {
                        DisplayContextMenu(false);
                    }
                }
            }
        }

        public void MooveEditor()
        {
            Vector2 currentMousePosition = UiExtManager.Instance.pointerEventData.position;
            Vector2 mouseDelta = currentMousePosition - lastMousePosition;

            grid.UpdateGridOffset(mouseDelta);
            int nodeCount = nodes.Count;
            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i].transform.position += (Vector3)mouseDelta;
            }

            lastMousePosition = currentMousePosition;
        }

        public void DisplayContextMenu(bool state)
        {
            contextMenuClickPosition = cursor.position;
            contextMenu.transform.position = (Vector2)cursor.position + contextMenuOffset;
            contextMenu.SetActive(state);
        }

        public void CreateNodeFromContextMenu(int prefabIndex)
        {
            CreateNode(contextMenuClickPosition, prefabIndex);
            DisplayContextMenu(false);
        }

        public void CreateNode(Vector2 position, int prefabIndex)
        {
            if (prefabIndex >= 0 && prefabIndex < nodesPrefabs.Length)
            {
                GameObject nodePrefab = nodesPrefabs[prefabIndex];

                if (nodeParent == null)
                {
                    Debug.LogError("nodeParent is null. Assign a valid parent transform for nodes.");
                    return;
                }

                GameObject node = Instantiate(nodePrefab, nodeParent);
                node.transform.position = position;

                Node n = node.GetComponent<Node>();
                if (n != null)
                {
                    n.myEditor = this;
                    n.Initialize();
                }
                else
                {
                    Debug.LogError("Node component missing from prefab: " + nodePrefab.name);
                }
            }
            else
            {
                Debug.LogError("Invalid prefab index: " + prefabIndex);
            }
        }

        public void ClearSelectedNodes()
        {
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                selectedNodes[i].DiplaySelectedFeedback(false);
            }

            selectedNodes.Clear();
        }

        public void DestroySelected()
        {
            for (int i = selectedNodes.Count - 1; i >= 0; i--)
            {
                Node nodeToDestroy = selectedNodes[i];
                nodeToDestroy.DestroyNode();

                nodes.Remove(nodeToDestroy);

                selectedNodes.RemoveAt(i);
            }

            ClearSelectedNodes();
        }

        public void ClearEditor()
        {
            foreach (Node node in nodes)
            {
                if (node != null)
                {
                    Destroy(node.gameObject);
                }
            }
            nodes.Clear();

            foreach (Connection connection in connections)
            {
                if (connection != null)
                {
                    Destroy(connection.gameObject);
                }
            }
            connections.Clear();

            Debug.Log("Node editor cleared.");
        }

        public GameObject GetNodePrefab(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                Debug.LogError("Keyword is null or empty. Cannot fetch node prefab.");
                return null;
            }

            var prefab = nodesPrefabs.FirstOrDefault(prefab => prefab.name.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            if (prefab == null)
            {
                Debug.LogWarning($"No prefab found with keyword '{keyword}'.");
            }

            return prefab;
        }

    }
}
