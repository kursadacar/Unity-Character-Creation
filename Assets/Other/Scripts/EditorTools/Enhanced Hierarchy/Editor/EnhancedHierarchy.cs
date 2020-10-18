using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedHierarchy {
    /// <summary>
    /// Main class, draws hierarchy items.
    /// </summary>
    [InitializeOnLoad]
    public static partial class EnhancedHierarchy {

        static EnhancedHierarchy() {
            if (Preferences.DebugEnabled || Preferences.ProfilingEnabled) {
                Utility.EnableFPSCounter();
                Utility.ForceUpdateHierarchyEveryFrame();
            }

            EditorApplication.hierarchyWindowItemOnGUI += SetItemInformation;
            EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void OnItemGUI(int id, Rect rect) {
            if (!Preferences.Enabled)
                return;

            using (ProfilerSample.Get("Enhanced Hierarchy"))
                try {
                    if (IsGameObject) {
                        for (var i = 0; i < Preferences.RightIcons.Value.Count; i++)
                            Preferences.RightIcons.Value[i].SafeInit();

                        for (var i = 0; i < Preferences.LeftIcons.Value.Count; i++)
                            Preferences.LeftIcons.Value[i].SafeInit();

                        Preferences.LeftSideButton.SafeInit();
                    }

                    if (IsFirstVisible && Reflected.HierarchyArea.Supported) {
                        Reflected.HierarchyArea.IndentWidth = Preferences.Indent;
                        Reflected.HierarchyArea.BaseIndent = Preferences.LeftMargin;
                    }

                    //SetTitle("EH 2.0");
                    CalculateIconsWidth();
                    DoSelection(RawRect);
                    IgnoreLockedSelection();
                    DrawTree(RawRect);
                    ChildToggle();
                    var trailingWidth = DoTrailing();
                    DrawHover();
                    ColorSort(RawRect);
                    DrawLeftSideIcons(RawRect);
                    DrawTooltip(RawRect, trailingWidth);

                    if (Reflected.IconWidthSupported)
                        Reflected.IconWidth = Preferences.DisableNativeIcon ? 0 : 16;

                    if (IsGameObject) {
                        rect.xMax -= Preferences.RightMargin;
                        rect.xMin = rect.xMax;
                        rect.y++;

                        for (var i = 0; i < Preferences.RightIcons.Value.Count; i++)
                            using (new GUIBackgroundColor(Styles.backgroundColorEnabled)) {
                                var icon = Preferences.RightIcons.Value[i];
                                rect.xMin -= icon.SafeGetWidth();
                                icon.SafeDoGUI(rect);
                                rect.xMax -= icon.SafeGetWidth();
                            }

                        var leftSideRect = RawRect;

                        if (Preferences.LeftmostButton)
                            leftSideRect.xMin = 0f;
                        else
                            leftSideRect.xMin -= 2f + CurrentGameObject.transform.childCount > 0 || Preferences.TreeOpacity > ALPHA_THRESHOLD ? 30f : 18f;

                        leftSideRect.xMax = leftSideRect.xMin + Preferences.LeftSideButton.SafeGetWidth();

                        using (new GUIBackgroundColor(Styles.backgroundColorEnabled))
                            Preferences.LeftSideButton.SafeDoGUI(leftSideRect);
                    }

                    DrawMiniLabel(ref rect);
                    DrawHorizontalSeparator(RawRect);
                } catch (Exception e) {
                    Utility.LogException(e);
                }
        }

        private static void DrawHover() {
            if (Reflected.NativeHierarchyHoverTintSupported) {

                if (IsFirstVisible && IsRepaintEvent)
                    Reflected.NativeHierarchyHoverTint = Preferences.HoverTintColor;

                return;
            }

            var tint = Preferences.HoverTintColor.Value;

            if (IsFirstVisible && Reflected.NativeHierarchyHoverTintSupported)
                Reflected.HierarchyWindowInstance.wantsMouseMove = tint.a >= ALPHA_THRESHOLD;

            if (tint.a < ALPHA_THRESHOLD)
                return;

            if (!Utility.ShouldCalculateTooltipAt(FullSizeRect))
                return;

            if (IsRepaintEvent)
                EditorGUI.DrawRect(FullSizeRect, tint);

            switch (Event.current.type) {
                case EventType.MouseMove:
                    Event.current.Use();
                    break;
            }
        }

        private static void IgnoreLockedSelection() {
            if (Preferences.AllowSelectingLocked || !IsFirstVisible || !IsRepaintEvent)
                return;

            using (ProfilerSample.Get()) {
                var selection = Selection.objects;
                var changed = false;

                for (var i = 0; i < selection.Length; i++)
                    if (selection[i] is GameObject && (selection[i].hideFlags & HideFlags.NotEditable) != 0 && !EditorUtility.IsPersistent(selection[i])) {
                        selection[i] = null;
                        changed = true;
                    }

                if (changed) {
                    Selection.objects = selection;
                    Reflected.SetHierarchySelectionNeedSync();
                    EditorApplication.RepaintHierarchyWindow();
                }
            }
        }

        private static void ChildToggle() {
            using (ProfilerSample.Get()) {
                if (!Preferences.NumericChildExpand || !IsRepaintEvent || !IsGameObject || CurrentGameObject.transform.childCount <= 0)
                    return;

                var rect = RawRect;
                var childString = CurrentGameObject.transform.childCount.ToString("00");
                var expanded = Reflected.GetTransformIsExpanded(CurrentGameObject);

                rect.xMax = rect.xMin - 1f;
                rect.xMin -= 15f;

                if (childString.Length > 2)
                    rect.xMin -= 4f;

                using (new GUIBackgroundColor(Styles.childToggleColor))
                    Styles.newToggleStyle.Draw(rect, Utility.GetTempGUIContent(childString), false, false, expanded, false);
            }
        }

        private static void DrawHorizontalSeparator(Rect rect) {
            if (Preferences.LineSize < 1 || Preferences.LineColor.Value.a <= ALPHA_THRESHOLD || !IsRepaintEvent)
                return;

            using (ProfilerSample.Get()) {
                rect.xMin = 0f;
                rect.xMax = rect.xMax + 50f;
                rect.yMin -= Preferences.LineSize / 2;
                rect.yMax = rect.yMin + Preferences.LineSize;

                EditorGUI.DrawRect(rect, Preferences.LineColor);

                if (!IsFirstVisible)
                    return;

                rect.y = FinalRect.y - Preferences.LineSize / 2;

                var height = Reflected.HierarchyWindowInstance.position.height;
                var count = (height - FinalRect.y) / FinalRect.height;

                if (FinalRect.height <= 0f)
                    count = 100f;

                for (var i = 0; i < count; i++) {
                    rect.y += RawRect.height;
                    EditorGUI.DrawRect(rect, Preferences.LineColor);
                }
            }
        }

        private static void ColorSort(Rect rect) {
            if (!IsRepaintEvent)
                return;

            using (ProfilerSample.Get()) {
                rect.xMin = 0f;
                rect.xMax = rect.xMax + 50f;

                var rowTint = GetRowTint();
                var rowLayerTint = GetRowLayerTint();

                if (rowLayerTint.a > ALPHA_THRESHOLD)
                    using (new GUIColor(rowLayerTint))
                        GUI.DrawTexture(rect, Styles.fadeTexture, ScaleMode.StretchToFill);

                if (rowTint.a > ALPHA_THRESHOLD)
                    EditorGUI.DrawRect(rect, rowTint);

                if (!IsFirstVisible)
                    return;

                rect.y = FinalRect.y;

                var height = Reflected.HierarchyWindowInstance.position.height;
                var count = (height - FinalRect.y) / FinalRect.height;

                if (FinalRect.height <= 0f)
                    count = 100f;

                for (var i = 0; i < count; i++) {
                    rect.y += RawRect.height;
                    rowTint = GetRowTint(rect);

                    if (rowTint.a > ALPHA_THRESHOLD)
                        EditorGUI.DrawRect(rect, rowTint);
                }
            }
        }

        private static void DrawTree(Rect rect) {
            if (Preferences.TreeOpacity <= ALPHA_THRESHOLD || !IsGameObject)
                return;

            if (!IsRepaintEvent && !Preferences.SelectOnTree)
                return;

            using (ProfilerSample.Get())
            using (new GUIColor(CurrentColor, Preferences.TreeOpacity)) {
                var indent = 16f;

                if (Reflected.HierarchyArea.Supported)
                    indent = Reflected.HierarchyArea.IndentWidth;

                rect.xMin -= 14f;
                rect.xMax = rect.xMin + 14f;

                if (CurrentGameObject.transform.childCount == 0 && CurrentGameObject.transform.parent) {
                    GUI.DrawTexture(rect, Utility.LastInHierarchy(CurrentGameObject.transform) ? Styles.treeEndTexture : Styles.treeMiddleTexture);

                    if (Preferences.SelectOnTree && GUI.Button(rect, GUIContent.none, Styles.labelNormal))
                        Selection.activeTransform = CurrentGameObject.transform.parent;
                }

                var currentTransform = CurrentGameObject.transform.parent;

                for (rect.x -= indent; rect.xMin > 0f && currentTransform && currentTransform.parent; rect.x -= indent) {
                    if (!Utility.LastInHierarchy(currentTransform))
                        using (new GUIColor(Utility.GetHierarchyColor(currentTransform.parent), Preferences.TreeOpacity)) {
                            GUI.DrawTexture(rect, Styles.treeLineTexture);

                            if (Preferences.SelectOnTree && GUI.Button(rect, GUIContent.none, Styles.labelNormal))
                                Selection.activeTransform = currentTransform.parent;
                        }

                    currentTransform = currentTransform.parent;
                }
            }
        }

        private static void CalculateIconsWidth() {
            using (ProfilerSample.Get()) {
                LeftIconsWidth = 0f;
                RightIconsWidth = 0f;

                if (!IsGameObject)
                    return;

                for (var i = 0; i < Preferences.RightIcons.Value.Count; i++)
                    RightIconsWidth += Preferences.RightIcons.Value[i].SafeGetWidth();

                for (var i = 0; i < Preferences.LeftIcons.Value.Count; i++)
                    LeftIconsWidth += Preferences.LeftIcons.Value[i].SafeGetWidth();
            }
        }

        private static void DrawLeftSideIcons(Rect rect) {
            if (!IsGameObject)
                return;

            using (ProfilerSample.Get()) {
                rect.xMin += LabelSize;
                rect.xMin = Math.Min(rect.xMax - RightIconsWidth - LeftIconsWidth - CalcMiniLabelSize() - 5f - Preferences.RightMargin, rect.xMin);

                for (var i = 0; i < Preferences.LeftIcons.Value.Count; i++)
                    using (new GUIBackgroundColor(Styles.backgroundColorEnabled)) {
                        var icon = Preferences.LeftIcons.Value[i];

                        rect.xMax = rect.xMin + icon.SafeGetWidth();
                        icon.SafeDoGUI(rect);
                        rect.xMin = rect.xMax;
                    }
            }
        }

        private static float DoTrailing() {
            if (!IsRepaintEvent || !Preferences.Trailing || !IsGameObject)
                return RawRect.xMax;

            using (ProfilerSample.Get()) {
                var size = LabelSize; // CurrentStyle.CalcSize(Utility.GetTempGUIContent(GameObjectName)).x;
                var iconsWidth = RightIconsWidth + LeftIconsWidth + CalcMiniLabelSize() + Preferences.RightMargin;

                var iconsMin = FullSizeRect.xMax - iconsWidth;
                var labelMax = LabelOnlyRect.xMax;

                var overlapping = iconsMin <= labelMax;

                if (!overlapping)
                    return labelMax;

                var rect = FullSizeRect;

                rect.xMin = iconsMin - 18;
                rect.xMax = labelMax;

                if (Selection.gameObjects.Contains(CurrentGameObject))
                    EditorGUI.DrawRect(rect, Reflected.HierarchyFocused ? Styles.selectedFocusedColor : Styles.selectedUnfocusedColor);
                else
                    EditorGUI.DrawRect(rect, Styles.normalColor);

                rect.y++;

                using (new GUIColor(CurrentColor))
                    EditorStyles.boldLabel.Draw(rect, trailingContent, 0);
                return iconsMin;
            }
        }

        private static void TagMiniLabel(ref Rect rect) {
            if (Event.current.type == EventType.Layout)
                return;

            using (ProfilerSample.Get())
            using (new GUIContentColor(CurrentColor * new Color(1f, 1f, 1f, CurrentGameObject.CompareTag(UNTAGGED) ? Styles.backgroundColorDisabled.a : Styles.backgroundColorEnabled.a))) {
                GUI.changed = false;
                Styles.miniLabelStyle.fontSize = Preferences.SmallerMiniLabel ? 8 : 9;

                rect.xMin -= Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(GameObjectTag)).x;

                var tag = EditorGUI.TagField(rect, GameObjectTag, Styles.miniLabelStyle);

                if (GUI.changed)
                    Icons.Tag.ChangeTagAndAskForChildren(GetSelectedObjectsAndCurrent(), tag);
            }
        }

        private static void LayerMiniLabel(ref Rect rect) {
            if (Event.current.type == EventType.Layout)
                return;

            using (ProfilerSample.Get())
            using (new GUIContentColor(CurrentColor * new Color(1f, 1f, 1f, CurrentGameObject.layer == UNLAYERED ? Styles.backgroundColorDisabled.a : Styles.backgroundColorEnabled.a))) {
                GUI.changed = false;
                Styles.miniLabelStyle.fontSize = Preferences.SmallerMiniLabel ? 8 : 9;

                rect.xMin -= Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(LayerMask.LayerToName(CurrentGameObject.layer))).x;

                var layer = EditorGUI.LayerField(rect, CurrentGameObject.layer, Styles.miniLabelStyle);

                if (GUI.changed)
                    Icons.Layer.ChangeLayerAndAskForChildren(GetSelectedObjectsAndCurrent(), layer);
            }
        }

        private static void DrawMiniLabel(ref Rect rect) {
            if (Preferences.MiniLabel.Value == MiniLabelType.None || !IsGameObject)
                return;

            rect.x -= 3f;

            using (ProfilerSample.Get())
                switch (Preferences.MiniLabel.Value) {
                    case MiniLabelType.Tag:
                        if (HasTag)
                            TagMiniLabel(ref rect);
                        break;

                    case MiniLabelType.Layer:
                        if (HasLayer)
                            LayerMiniLabel(ref rect);
                        break;

                    case MiniLabelType.TagOrLayer:
                        if (HasTag)
                            TagMiniLabel(ref rect);
                        else if (HasLayer)
                            LayerMiniLabel(ref rect);
                        break;

                    case MiniLabelType.LayerOrTag:
                        if (HasLayer)
                            LayerMiniLabel(ref rect);
                        else if (HasTag)
                            TagMiniLabel(ref rect);
                        break;

                    case MiniLabelType.TagAndLayer:
                        if (HasTag && HasLayer || !Preferences.CentralizeMiniLabelWhenPossible) {
                            var topRect = rect;
                            var bottomRect = rect;

                            topRect.yMax = RawRect.yMax - RawRect.height / 2f;
                            bottomRect.yMin = RawRect.yMin + RawRect.height / 2f;

                            if (HasTag)
                                TagMiniLabel(ref topRect);
                            if (HasLayer)
                                LayerMiniLabel(ref bottomRect);

                            rect.xMin = Mathf.Min(topRect.xMin, bottomRect.xMin);
                        } else if (HasLayer)
                            LayerMiniLabel(ref rect);
                        else if (HasTag)
                            TagMiniLabel(ref rect);

                        break;

                    case MiniLabelType.LayerAndTag:
                        if (HasTag && HasLayer || !Preferences.CentralizeMiniLabelWhenPossible) {
                            var topRect = rect;
                            var bottomRect = rect;

                            topRect.yMax = RawRect.yMax - RawRect.height / 2f;
                            bottomRect.yMin = RawRect.yMin + RawRect.height / 2f;

                            if (HasLayer)
                                LayerMiniLabel(ref topRect);
                            if (HasTag)
                                TagMiniLabel(ref bottomRect);

                            rect.xMin = Mathf.Min(topRect.xMin, bottomRect.xMin);
                        } else if (HasLayer)
                            LayerMiniLabel(ref rect);
                        else if (HasTag)
                            TagMiniLabel(ref rect);

                        break;
                }
        }

        private static float CalcMiniLabelSize() {
            Styles.miniLabelStyle.fontSize = Preferences.SmallerMiniLabel ? 8 : 9;

            using (ProfilerSample.Get())
                switch (Preferences.MiniLabel.Value) {
                    case MiniLabelType.Tag:
                        if (HasTag)
                            return Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(GameObjectTag)).x;
                        else
                            return 0f;

                    case MiniLabelType.Layer:
                        if (HasLayer)
                            return Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(LayerMask.LayerToName(CurrentGameObject.layer))).x;
                        else
                            return 0f;

                    case MiniLabelType.TagOrLayer:
                        if (HasTag)
                            return Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(GameObjectTag)).x;
                        else if (HasLayer)
                            return Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(LayerMask.LayerToName(CurrentGameObject.layer))).x;
                        else
                            return 0f;

                    case MiniLabelType.LayerOrTag:
                        if (HasLayer)
                            return Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(LayerMask.LayerToName(CurrentGameObject.layer))).x;
                        else if (HasTag)
                            return Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(GameObjectTag)).x;
                        else
                            return 0f;

                    case MiniLabelType.TagAndLayer:
                    case MiniLabelType.LayerAndTag:
                        var tagSize = 0f;
                        var layerSize = 0f;

                        if (HasTag)
                            tagSize = Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(GameObjectTag)).x;
                        if (HasLayer)
                            layerSize = Styles.miniLabelStyle.CalcSize(Utility.GetTempGUIContent(LayerMask.LayerToName(CurrentGameObject.layer))).x;

                        return Mathf.Max(tagSize, layerSize);

                    default:
                        return 0f;
                }
        }

        private static void DrawTooltip(Rect rect, float fullTrailingWidth) {
            if (!Preferences.Tooltips || !IsGameObject || !IsRepaintEvent)
                return;

            using (ProfilerSample.Get()) {
                if (DragSelection != null)
                    return;

                rect.xMax = Mathf.Min(fullTrailingWidth, rect.xMin + LabelSize);
                rect.xMin = 0f;

                if (!Utility.ShouldCalculateTooltipAt(rect))
                    return;

                var tooltip = new StringBuilder(100);

                tooltip.AppendLine(GameObjectName);
                tooltip.AppendFormat("\nTag: {0}", GameObjectTag);
                tooltip.AppendFormat("\nLayer: {0}", LayerMask.LayerToName(CurrentGameObject.layer));

                if (GameObjectUtility.GetStaticEditorFlags(CurrentGameObject) != 0)
                    tooltip.AppendFormat("\nStatic: {0}", Utility.EnumFlagsToString(GameObjectUtility.GetStaticEditorFlags(CurrentGameObject)));

                tooltip.AppendLine();
                tooltip.AppendLine();

                foreach (var component in Components)
                    if (component is Transform)
                        continue;
                    else if (component)
                        tooltip.AppendLine(ObjectNames.GetInspectorTitle(component));
                    else
                        tooltip.AppendLine("Missing Component");

                EditorGUI.LabelField(rect, Utility.GetTempGUIContent(null, tooltip.ToString().TrimEnd('\n', '\r')));
            }
        }

        private static void DoSelection(Rect rect) {
            if (!Preferences.EnhancedSelectionSupported || !Preferences.EnhancedSelection || Event.current.button != 1) {
                DragSelection = null;
                return;
            }

            using (ProfilerSample.Get()) {
                rect.xMin = 0f;

                switch (Event.current.type) {
                    case EventType.MouseDrag:
                        if (!IsFirstVisible)
                            return;

                        if (DragSelection == null) {
                            DragSelection = new List<Object>();
                            SelectionStart = Event.current.mousePosition;
                            SelectionRect = new Rect();
                        }

                        SelectionRect = new Rect() {
                            xMin = Mathf.Min(Event.current.mousePosition.x, SelectionStart.x),
                            yMin = Mathf.Min(Event.current.mousePosition.y, SelectionStart.y),
                            xMax = Mathf.Max(Event.current.mousePosition.x, SelectionStart.x),
                            yMax = Mathf.Max(Event.current.mousePosition.y, SelectionStart.y)
                        };

                        if (Event.current.control || Event.current.command)
                            DragSelection.AddRange(Selection.objects);

                        Selection.objects = DragSelection.ToArray();
                        Event.current.Use();
                        break;

                    case EventType.MouseUp:
                        if (DragSelection != null)
                            Event.current.Use();
                        DragSelection = null;
                        break;

                    case EventType.Repaint:
                        if (DragSelection == null || !IsFirstVisible)
                            break;

                        Rect scrollRect;

                        if (Event.current.mousePosition.y > FinalRect.y) {
                            scrollRect = FinalRect;
                            scrollRect.y += scrollRect.height;
                        } else if (Event.current.mousePosition.y < RawRect.y) {
                            scrollRect = RawRect;
                            scrollRect.y -= scrollRect.height;
                        } else
                            break;

                        SelectionRect = new Rect() {
                            xMin = Mathf.Min(scrollRect.xMax, SelectionStart.x),
                            yMin = Mathf.Min(scrollRect.yMax, SelectionStart.y),
                            xMax = Mathf.Max(scrollRect.xMax, SelectionStart.x),
                            yMax = Mathf.Max(scrollRect.yMax, SelectionStart.y)
                        };

                        if (Event.current.control || Event.current.command)
                            DragSelection.AddRange(Selection.objects);

                        Selection.objects = DragSelection.ToArray();

                        GUI.ScrollTowards(scrollRect, 9f);
                        EditorApplication.RepaintHierarchyWindow();
                        break;

                    case EventType.Layout:
                        if (DragSelection != null && IsGameObject)
                            if (!SelectionRect.Overlaps(rect) && DragSelection.Contains(CurrentGameObject))
                                DragSelection.Remove(CurrentGameObject);
                            else if (SelectionRect.Overlaps(rect) && !DragSelection.Contains(CurrentGameObject))
                                DragSelection.Add(CurrentGameObject);
                        break;
                }
            }
        }

        public static Color GetRowTint() {
            return GetRowTint(RawRect);
        }

        public static Color GetRowTint(Rect rect) {
            using (ProfilerSample.Get())
                if (rect.y / RawRect.height % 2 >= 0.5f)
                    return Preferences.OddRowColor;
                else
                    return Preferences.EvenRowColor;
        }

        public static Color GetRowLayerTint() {
            return GetRowLayerTint(CurrentGameObject);
        }

        public static Color GetRowLayerTint(GameObject go) {
            if (!go)
                return Color.clear;

            var layerColors = Preferences.PerLayerRowColors.Value;

            if (layerColors == null)
                return Color.clear;

            using (ProfilerSample.Get())
                for (var i = 0; i < layerColors.Count; i++)
                    if (layerColors[i] == go.layer)
                        return layerColors[i].color;

            return Color.clear;
        }

        private static List<GameObject> GetSelectedObjectsAndCurrent() {
            if (!Preferences.ChangeAllSelected || Selection.gameObjects.Length <= 1)
                return new List<GameObject> { CurrentGameObject };

            var selection = new List<GameObject>(Selection.gameObjects);

            for (var i = 0; i < selection.Count; i++)
                if (EditorUtility.IsPersistent(selection[i]))
                    selection.RemoveAt(i);

            if (!selection.Contains(CurrentGameObject))
                selection.Add(CurrentGameObject);

            selection.Remove(null);
            return selection;
        }
    }
}