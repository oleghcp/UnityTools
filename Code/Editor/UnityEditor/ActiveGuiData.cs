using UnityObject = UnityEngine.Object;

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
            if (_instance != null)
                _instance._id = controlId;
            else
                _instance = new DropDownData { _id = controlId };

            _instance._selectedIndex = index;
        }
    }

    internal class DragAndDropData
    {
        private static DragAndDropData _instance;
        private int _id;
        private UnityObject[] _droppedObjects;

        public static UnityObject[] GetObjectsById(int controlId)
        {
            UnityObject[] objects = null;

            if (_instance != null)
            {
                if (_instance._id == controlId)
                {
                    objects = _instance._droppedObjects;
                    _instance = null;
                }
            }

            return objects;
        }

        public static void PlaceObjects(int controlId, UnityObject[] objects)
        {
            if (_instance != null)
                _instance._id = controlId;
            else
                _instance = new DragAndDropData { _id = controlId };

            _instance._droppedObjects = objects;
        }
    }
}
