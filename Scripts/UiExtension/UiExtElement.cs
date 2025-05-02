namespace UiExtension
{
    public enum ClickType { Down, Stay, Up }
    public class UiExtElement : MonoBehaviour
    {
        public bool isActived = false;
        bool lastState = false;
        public delegate void OnMouseEnter();
        public OnMouseEnter onMouseEnter;
        public delegate void OnMouseExit();
        public OnMouseExit onMouseExit;
        public delegate void OnMouseClick(int mouseButton, ClickType clickType);
        public OnMouseClick onMouseClick;

        private void Update()
        {
            if (lastState != isActived)
            {
                if (isActived && !UiExtManager.Instance.activeElements.Contains(this))
                {
                    UiExtManager.Instance.activeElements.Add(this);
                }
                else if (!isActived && UiExtManager.Instance.activeElements.Contains(this))
                {
                    UiExtManager.Instance.activeElements.Remove(this);
                }

                lastState = isActived;
            }
        }
    }
}
