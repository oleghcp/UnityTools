namespace UnityUtilityEditor
{
    internal static class PrefsConstants
    {
        public const string ASSEMBLY_INDEX_KEY = LibConstants.LIB_NAME + "_csowai";
        public const string WIDTH_TO_HEIGHT_KEY = LibConstants.LIB_NAME + "_cfw2h";
#if UNITY_2019_3_OR_NEWER
        public const string SIDE_PANEL_KEY = LibConstants.LIB_NAME + "_gsp";
        public const string HIDE_CONTENT_KEY = LibConstants.LIB_NAME + "_ghnc";
        public const string SIDE_PANEL_WIDTH_KEY = LibConstants.LIB_NAME + "_gspw";
        public const string SIDE_PANEL_TAB_KEY = LibConstants.LIB_NAME + "_gspt";
        public const string GRID_SNAPING_KEY = LibConstants.LIB_NAME + "_ggs";
        public const string TRANSITION_VIEW_TYPE_KEY = LibConstants.LIB_NAME + "_gtvt";
#endif
    }
}
