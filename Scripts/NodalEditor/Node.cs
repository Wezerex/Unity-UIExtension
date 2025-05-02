namespace UiExtension.NodalEditor
{
    public class Node : MonoBehaviour
    {
        public NodeEditor myEditor;
        public List<ConnectionPoint> inPoints;
        public List<ConnectionPoint> outPoints;

        public GameObject selectedFeeback;

        DraggableElement draggableElement;

        public delegate void OnOutConnectionCreate(Connection connection);
        public OnOutConnectionCreate onOutConnectionCreate;

        public delegate void OnInConnectionCreate(Connection connection);
        public OnInConnectionCreate onInConnectionCreate;

        public delegate void OnInitialize();
        public OnInitialize onInitialize;

        public virtual void Initialize()
        {
            int inCount = inPoints.Count;
            for (int i = 0; i < inCount; i++)
            {
                inPoints[i].owner = this;
            }

            int outCount = outPoints.Count;
            for (int i = 0;i < outCount; i++)
            {
                outPoints[i].owner = this;
            }

            myEditor.nodes.Add(this);

            draggableElement = GetComponent<DraggableElement>();
            draggableElement.onUnclick.AddListener(SelectNode);

            onInitialize.Invoke();
        }

        public void DiplaySelectedFeedback(bool display)
        {
            selectedFeeback.gameObject.SetActive(display);
        }

        private void SelectNode()
        {
            myEditor.ClearSelectedNodes();
            myEditor.selectedNodes.Add(this);
            DiplaySelectedFeedback(true);
        }

        public void DestroyNode()
        {
            int inCount = inPoints.Count;
            for (int i = 0; i < inCount; i++)
            {
                ConnectionPoint inPoint = inPoints[i];
                for (int j = inPoint.connections.Count - 1; j >= 0; j--)
                {
                    Connection connection = inPoint.connections[j];
                    connection.from.connections.Remove(connection);
                    Destroy(connection.gameObject);
                }
            }

            int outCount = outPoints.Count;
            for (int i = 0; i < outCount; i++)
            {
                ConnectionPoint outPoint = outPoints[i];
                for (int j = outPoint.connections.Count - 1; j >= 0; j--)
                {
                    Connection connection = outPoint.connections[j];
                    connection.to.connections.Remove(connection);
                    Destroy(connection.gameObject);
                }
            }

            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class SerializedNode
    {
        public Vector2 nodePosition;
        public string nodeType;

        public SerializedNode(Node node)
        {
            nodePosition = node.transform.position;
            nodeType = node.GetType().Name; 
        }
    }
}
