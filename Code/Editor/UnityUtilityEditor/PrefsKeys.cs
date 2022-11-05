﻿namespace UnityUtilityEditor
{
    internal static class PrefsKeys
    {
        public const string ASSEMBLY_INDEX = LibConstants.LIB_NAME + "_csowai";
        public const string WIDTH_TO_HEIGHT = LibConstants.LIB_NAME + "_cfw2h";
        public const string OPEN_FOLDERS_BY_CLICK = LibConstants.LIB_NAME + "_eofbc";
        public const string OPEN_SO_ASSETS_CODE_BY_CLICK = LibConstants.LIB_NAME + "_eosoacbc";
        public const string SUPPRESSED_WARNINGS_IN_IDE = LibConstants.LIB_NAME + "_eswiide";
#if UNITY_2019_3_OR_NEWER
        public const string SIDE_PANEL = LibConstants.LIB_NAME + "_gsp";
        public const string HIDE_CONTENT = LibConstants.LIB_NAME + "_ghnc";
        public const string SIDE_PANEL_WIDTH = LibConstants.LIB_NAME + "_gspw";
        public const string SIDE_PANEL_TAB = LibConstants.LIB_NAME + "_gspt";
        public const string GRID_SNAPING = LibConstants.LIB_NAME + "_ggs";
        public const string TRANSITION_VIEW_TYPE = LibConstants.LIB_NAME + "_gtvt";
#endif
    }
}