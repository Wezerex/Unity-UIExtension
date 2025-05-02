namespace UiExtension
{
    public class MouseOver : UiExtElement
    {
        public UnityEvent onMouseOver;
        public UnityEvent onMouseOut;

        private void Awake()
        {
            onMouseEnter += onMouseOver.Invoke;
            onMouseExit += onMouseOut.Invoke;
        }
    }
}
