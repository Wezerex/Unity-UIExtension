namespace UiExtension
{
    public class ExtButton : UiExtElement
    {
        public int mouseButton;
        public ClickType clickType;

        public UnityEvent onClick;

        private void Awake()
        {
            onMouseClick += OnClickReceived;
        }

        public void OnClickReceived(int _mouseButton, ClickType _clickType)
        {
            if (_clickType == clickType && _mouseButton == mouseButton)
            {
                onClick?.Invoke();
            }
        }
    }
}
