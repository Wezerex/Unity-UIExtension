namespace UiExtension
{
    public class DropZone : UiExtElement
    {
        [Range(1, 100)]
        public int dragId = 1;

        public bool isParent = true;
        public Transform dropParent;

        public bool reposition;

        public Vector2 dropPosition;

        public UnityEvent onDrop;

        public void HandleDrop(DraggableElement element)
        {
            Transform targetParent = isParent ? transform : dropParent;

            if (reposition)
            {
                element.transform.SetParent(targetParent);
                element.transform.localPosition = dropPosition;
            }
            else
            {
                element.transform.SetParent(targetParent);
            }

            onDrop.Invoke();
        }
    }

    [CustomEditor(typeof(DropZone))]
    public class DropZoneEditor : Editor
    {
        private SerializedProperty onDropProperty;

        private void OnEnable()
        {
            onDropProperty = serializedObject.FindProperty("onDrop");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DropZone dropZone = (DropZone)target;

            dropZone.dragId = EditorGUILayout.IntField("Drag ID", dropZone.dragId);
            dropZone.dragId = Mathf.Max(1, dropZone.dragId);

            dropZone.isParent = EditorGUILayout.Toggle("Is Parent", dropZone.isParent);

            if (!dropZone.isParent)
            {
                dropZone.dropParent = (Transform)EditorGUILayout.ObjectField("Drop Parent", dropZone.dropParent, typeof(Transform), true);
            }

            dropZone.reposition = EditorGUILayout.Toggle("Reposition", dropZone.reposition);

            if (dropZone.reposition)
            {
                dropZone.dropPosition = EditorGUILayout.Vector2Field("Local drop Position", dropZone.dropPosition);
            }

            EditorGUILayout.PropertyField(onDropProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
