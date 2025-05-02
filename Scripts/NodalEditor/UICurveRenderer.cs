namespace UiExtension
{
    public class UICurveRenderer : Graphic
    {
        public RectTransform from;
        public RectTransform to;
        public Transform referent;
        public float thickness = 2f;
        float baseThickness;
        public int segments = 20;
        public float curveStrength = 0.5f;
        public Gradient colorGradient;

        public int mouseButton;
        public ClickType clickType;

        public float hoverThickness = 4f; 
        public Gradient hoverGradient; 

        public float selectionThreshold = 5f; 

        private GameObject hoverFeedbackObject; 

        public delegate void OnMouseClick();
        public OnMouseClick onMouseClick;

        public void Initialized()
        {
            onMouseClick += CreateHoverFeedback;
            baseThickness = thickness;
        }

        private bool IsMouseOverCurve()
        {
            Vector2 mousePosition = UiExtManager.Instance.pointerEventData.position;
            Vector2 p1 = from.position;
            Vector2 p4 = to.position;

            Vector2 p2 = p1 + Vector2.right * (p4.x - p1.x) * curveStrength;
            Vector2 p3 = p4 + Vector2.left * (p4.x - p1.x) * curveStrength;

            Vector2[] points = new Vector2[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                points[i] = CalculateBezierPoint(t, p1, p2, p3, p4);
            }

            for (int i = 0; i < segments; i++)
            {
                if (IsMouseNearSegment(points[i], points[i + 1], mousePosition))
                {
                    return true; 
                }
            }
            return false;
        }

        private bool IsMouseNearSegment(Vector2 start, Vector2 end, Vector2 mousePosition)
        {
            Vector2 closestPoint = ClosestPointOnSegment(start, end, mousePosition);
            float distanceToMouse = Vector2.Distance(closestPoint, mousePosition);
            return distanceToMouse <= selectionThreshold;
        }

        private Vector2 ClosestPointOnSegment(Vector2 start, Vector2 end, Vector2 point)
        {
            Vector2 segmentDirection = (end - start).normalized;
            float segmentLength = Vector2.Distance(start, end);
            Vector2 projection = Vector2.Dot(point - start, segmentDirection) * segmentDirection;
            return start + Vector2.ClampMagnitude(projection, segmentLength);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            if (from == null || to == null)
                return;

            Vector2 p1 = referent.InverseTransformPoint(from.position);
            Vector2 p4 = referent.InverseTransformPoint(to.position);

            Vector2 p2 = p1 + Vector2.right * (p4.x - p1.x) * curveStrength;
            Vector2 p3 = p4 + Vector2.left * (p4.x - p1.x) * curveStrength;

            if (Mathf.Abs(p1.y - p4.y) < 0.1f)
            {
                p2.y = p1.y;
                p3.y = p4.y;
            }

            Vector2[] points = new Vector2[segments + 1];
            Color[] colors = new Color[segments + 1];

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                points[i] = CalculateBezierPoint(t, p1, p2, p3, p4);
                colors[i] = colorGradient.Evaluate(t);
            }

            for (int i = 0; i < segments; i++)
            {
                DrawQuad(vh, points[i], points[i + 1], colors[i], colors[i + 1], thickness);
            }
        }

        private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            Vector2 q0 = Vector2.Lerp(p0, p1, t);
            Vector2 q1 = Vector2.Lerp(p1, p2, t);
            Vector2 q2 = Vector2.Lerp(p2, p3, t);

            Vector2 r0 = Vector2.Lerp(q0, q1, t);
            Vector2 r1 = Vector2.Lerp(q1, q2, t);

            return Vector2.Lerp(r0, r1, t);
        }

        private void DrawQuad(VertexHelper vh, Vector2 start, Vector2 end, Color startColor, Color endColor, float width)
        {
            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * width / 2f;

            vh.AddVert(start - normal, startColor, Vector2.zero);
            vh.AddVert(start + normal, startColor, Vector2.zero);
            vh.AddVert(end + normal, endColor, Vector2.zero);
            vh.AddVert(end - normal, endColor, Vector2.zero);

            int startIndex = vh.currentVertCount - 4;
            vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vh.AddTriangle(startIndex, startIndex + 2, startIndex + 3);
        }

        private void CreateHoverFeedback()
        {
            if (hoverFeedbackObject != null) return; 

            hoverFeedbackObject = new GameObject("HoverFeedbackCurve");
            UICurveRenderer hoverCurve = hoverFeedbackObject.AddComponent<UICurveRenderer>();
            hoverFeedbackObject.AddComponent<CanvasRenderer>();

            thickness = (thickness + hoverThickness) / 2;

            hoverCurve.from = this.from;
            hoverCurve.to = this.to;
            hoverCurve.referent = this.referent;
            hoverCurve.thickness = hoverThickness;
            hoverCurve.segments = this.segments;
            hoverCurve.curveStrength = this.curveStrength;
            hoverCurve.colorGradient = hoverGradient;

            hoverCurve.raycastTarget = false;

            hoverFeedbackObject.transform.SetParent(transform.parent, false);
            hoverFeedbackObject.transform.SetAsFirstSibling(); 
        }

        public void RemoveHoverFeedback()
        {
            if (hoverFeedbackObject != null)
            {
                Destroy(hoverFeedbackObject);
                hoverFeedbackObject = null;
            }
            thickness = baseThickness;
        }

        private void Update()
        {
            SetVerticesDirty();

            if (clickType == ClickType.Down)
            {
                if (Input.GetMouseButtonDown(mouseButton))
                {
                    if (IsMouseOverCurve())
                    {
                        onMouseClick?.Invoke();
                    }
                }
            }
            else if (clickType == ClickType.Stay)
            {
                if (Input.GetMouseButton(mouseButton))
                {
                    if (IsMouseOverCurve())
                    {
                        onMouseClick?.Invoke();
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(mouseButton))
                {
                    if (IsMouseOverCurve())
                    {
                        onMouseClick?.Invoke();
                    }
                }
            }
        }
    }
}
