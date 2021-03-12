namespace UnityEditor
{
    internal class DropDownData
    {
        private static DropDownData _instance;
        private int _id;
        private int _selectedIndex;

        public static int GetSelectedIndexById(int selectedIndex, int controlId)
        {
            if (_instance != null)
            {
                if (_instance._id == controlId)
                {
                    selectedIndex = _instance._selectedIndex;
                    _instance = null;
                }
            }

            return selectedIndex;
        }

        public static void MenuAction(int controlId, int index)
        {
            _instance ??= new DropDownData { _id = controlId };
            _instance._selectedIndex = index;
        }
    }
}
