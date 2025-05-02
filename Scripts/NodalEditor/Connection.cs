namespace UiExtension.NodalEditor
{
    public class Connection : MonoBehaviour
    {
        public ConnectionPoint from;
        public ConnectionPoint to;
        public UICurveRenderer curve;

        public void Initialize(ConnectionPoint point)
        {
            curve.Initialized();
            curve.raycastTarget = false;

            if(point.type == ConnectionPointType.outPoint)
            {
                from = point;
                curve.from = point.pointCenter;
                curve.to = from.owner.myEditor.cursor.GetComponent<RectTransform>();
                curve.referent = from.owner.myEditor.transform;
            }
            else
            {
                to = point;
                curve.to = point.pointCenter;
                curve.from = to.owner.myEditor.cursor.GetComponent<RectTransform>();
                curve.referent = to.owner.myEditor.transform;
            }
        }

        public void EndConnection(ConnectionPoint point)
        {
            curve.raycastTarget = true;
            if (point.type == ConnectionPointType.inPoint)
            {
                to = point;
                curve.to = point.pointCenter;
                to.owner.onInConnectionCreate?.Invoke(this);
            }
            else
            {
                from = point;
                curve.from = point.pointCenter;
                from.owner.onOutConnectionCreate?.Invoke(this);
            }
            curve.onMouseClick += SelectConnection;
        }

        public void SelectConnection()
        {
            if(from.owner.myEditor.selectedConnection)
            {
                from.owner.myEditor.selectedConnection.UnSelectConnection();
            }
            from.owner.myEditor.selectedConnection = this;
        }

        public void UnSelectConnection()
        {
            from.owner.myEditor.selectedConnection = null;
            curve.RemoveHoverFeedback();
        }

        public void DestroyConnection()
        {
            from.owner.myEditor.connections.Remove(this);
            from.connections.Remove(this);
            to.connections.Remove(this);
            curve.RemoveHoverFeedback();
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class SerializedConnection
    {
        public ConnectionPointReference from;
        public ConnectionPointReference to;

        public SerializedConnection(NodeEditor editor, ConnectionPoint fromPoint, ConnectionPoint toPoint)
        {
            int fromNodeIndex = editor.nodes.IndexOf(fromPoint.owner);
            if (fromNodeIndex == -1)
            {
                Debug.LogError("fromPoint's owner node not found in editor nodes list!");
                return;
            }
            from = new ConnectionPointReference
            {
                nodeIndex = fromNodeIndex,
                pointIndex = editor.nodes[fromNodeIndex].outPoints.IndexOf(fromPoint)
            };

            int toNodeIndex = editor.nodes.IndexOf(toPoint.owner);
            if (toNodeIndex == -1)
            {
                Debug.LogError("toPoint's owner node not found in editor nodes list!");
                return;
            }
            to = new ConnectionPointReference
            {
                nodeIndex = toNodeIndex,
                pointIndex = editor.nodes[toNodeIndex].inPoints.IndexOf(toPoint)
            };
        }
    }

    [System.Serializable]
    public class ConnectionPointReference
    {
        public int nodeIndex;
        public int pointIndex;
    }

}
