namespace UiExtension.NodalEditor
{
    public enum ConnectionPointType { inPoint, outPoint }
    public class ConnectionPoint : MonoBehaviour
    {
        public RectTransform pointCenter;
        public Node owner;
        public List<Connection> connections = new List<Connection>();
        public ConnectionPointType type;
        public bool limitConnection;
        public int maxConnection;

        public void CreateConnection()
        {
            if (limitConnection && connections.Count >= maxConnection)
            {
                RemoveOldestConnection(); 
            }

            Connection connection = Instantiate(owner.myEditor.connectionPrefab, owner.myEditor.connectionParent).GetComponent<Connection>();
            connection.Initialize(this);
            connections.Add(connection);
            owner.myEditor.connectionIncreate = connection.gameObject;
        }

        public void Connect()
        {
            Connection connection = owner.myEditor.connectionIncreate.GetComponent<Connection>();

            if (!IsValidConnection(connection))
            {
                RemoveConnection(connection);
                return;
            }

            if (limitConnection && connections.Count >= maxConnection)
            {
                RemoveOldestConnection();
            }

            owner.myEditor.connections.Add(connection);
            connections.Add(connection);
            connection.EndConnection(this);
            owner.myEditor.connectionIncreate = null;
        }

        private bool IsValidConnection(Connection connection)
        {
            bool fromOutPoint = connection.from == this;
            bool fromInPoint = connection.to == this;

            if (fromOutPoint)
            {
                if (owner == connection.from.owner || type == connection.from.type)
                {
                    return false;
                }
            }
            else if (fromInPoint) 
            {
                if (owner == connection.to.owner || type == connection.to.type)
                {
                    return false;
                }
            }

            return true;
        }

        private void RemoveConnection(Connection connection)
        {
            if (connections.Contains(connection))
            {
                connections.Remove(connection);

                if (connection.from == this)
                {
                    connection.to?.connections.Remove(connection);
                }
                else if (connection.to == this)
                {
                    connection.from?.connections.Remove(connection);
                }

                Destroy(connection.gameObject);
            }
        }

        private void RemoveOldestConnection()
        {
            if (connections.Count > 0)
            {
                Connection oldestConnection = connections[0];
                RemoveConnection(oldestConnection);
            }
        }

        public override bool Equals(object obj) { if (obj is ConnectionPoint other) { return this.gameObject == other.gameObject; } return false; }
        public override int GetHashCode() { return gameObject.GetHashCode(); }
    }

}
