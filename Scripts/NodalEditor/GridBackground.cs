namespace UiExtension.NodalEditor
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class ProceduralGridBackground : Graphic
    {
        public float gridSize = 20f; 
        public Color gridColor = Color.gray; 
        public float lineThickness = 1f; 
        public Vector2 gridOffset = Vector2.zero; 

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect rect = GetPixelAdjustedRect();
            float width = rect.width;
            float height = rect.height;

            float startX = gridOffset.x % gridSize;
            float startY = gridOffset.y % gridSize;

            for (float x = startX; x <= width; x += gridSize)
            {
                DrawLine(vh, new Vector2(x, 0), new Vector2(x, height), gridColor, lineThickness);
            }

            for (float y = startY; y <= height; y += gridSize)
            {
                DrawLine(vh, new Vector2(0, y), new Vector2(width, y), gridColor, lineThickness);
            }
        }

        private void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, Color color, float thickness)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * thickness / 2f;

            Vector2 v1 = start - normal;
            Vector2 v2 = start + normal;
            Vector2 v3 = end + normal;
            Vector2 v4 = end - normal;

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            vertex.position = v1;
            vh.AddVert(vertex);

            vertex.position = v2;
            vh.AddVert(vertex);

            vertex.position = v3;
            vh.AddVert(vertex);

            vertex.position = v4;
            vh.AddVert(vertex);

            int startIndex = vh.currentVertCount - 4;
            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }

        public void UpdateGridOffset(Vector2 offset)
        {
            gridOffset += offset;
            SetVerticesDirty(); 
        }

        private void Update()
        {
            SetVerticesDirty();
        }
    }
}

