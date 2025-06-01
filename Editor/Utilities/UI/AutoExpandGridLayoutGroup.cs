// using System;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace SpookyCore.Utilities.UI
// {
//     [AddComponentMenu("Layout/Auto Expand Grid Layout Group", 152)]
//     public class AutoExpandGridLayoutGroup : LayoutGroup
//     {
//         public enum Corner { UpperLeft = 0, UpperRight = 1, LowerLeft = 2, LowerRight = 3 }
//         public enum Axis { Horizontal = 0, Vertical = 1 }
//         public enum Constraint { Flexible = 0, FixedColumnCount = 1, FixedRowCount = 2 }
//         public enum ExpandSetting { X, Y, Both, None };
//
//         [SerializeField]
//         protected Corner m_StartCorner = Corner.UpperLeft;
//         public Corner StartCorner { get => m_StartCorner; set => SetProperty(ref m_StartCorner, value); }
//
//         [SerializeField]
//         protected Axis m_StartAxis = Axis.Horizontal;
//         public Axis StartAxis { get => m_StartAxis; set => SetProperty(ref m_StartAxis, value); }
//
//         [SerializeField]
//         protected Vector2 m_CellSize = new Vector2(100, 100);
//         public Vector2 CellSize { get => m_CellSize; set => SetProperty(ref m_CellSize, value); }
//
//         [SerializeField]
//         protected Vector2 m_Spacing = Vector2.zero;
//         public Vector2 Spacing { get => m_Spacing; set => SetProperty(ref m_Spacing, value); }
//
//         [SerializeField]
//         protected Constraint _mConstraints = Constraint.Flexible;
//         public Constraint Constraints { get => _mConstraints; set => SetProperty(ref _mConstraints, value); }
//
//         [SerializeField]
//         protected int m_ConstraintCount = 2;
//         public int ConstraintCount { get => m_ConstraintCount; set => SetProperty(ref m_ConstraintCount, Mathf.Max(1, value)); }
//
//         public ExpandSetting ExpandSettings;
//         public bool PreserveAspectRatio;
//         
//         protected AutoExpandGridLayoutGroup()
//         { }
//
// #if UNITY_EDITOR
//         protected override void OnValidate()
//         {
//             base.OnValidate();
//             ConstraintCount = ConstraintCount;
//         }
// #endif
//
//         public override void CalculateLayoutInputHorizontal()
//         {
//             base.CalculateLayoutInputHorizontal();
//
//             var minColumns = 0;
//             var preferredColumns = 0;
//             if (_mConstraints == Constraint.FixedColumnCount)
//             {
//                 minColumns = preferredColumns = m_ConstraintCount;
//             }
//             else if (_mConstraints == Constraint.FixedRowCount)
//             {
//                 minColumns = preferredColumns = Mathf.CeilToInt(rectChildren.Count / (float)m_ConstraintCount - 0.001f);
//             }
//             else
//             {
//                 minColumns = 1;
//                 preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));
//             }
//
//             SetLayoutInputForAxis(
//                 padding.horizontal + (CellSize.x + Spacing.x) * minColumns - Spacing.x,
//                 padding.horizontal + (CellSize.x + Spacing.x) * preferredColumns - Spacing.x,
//                 -1, 0);
//         }
//
//         public override void CalculateLayoutInputVertical()
//         {
//             var minRows = 0;
//             if (_mConstraints == Constraint.FixedColumnCount)
//             {
//                 minRows = Mathf.CeilToInt(rectChildren.Count / (float)m_ConstraintCount - 0.001f);
//             }
//             else if (_mConstraints == Constraint.FixedRowCount)
//             {
//                 minRows = m_ConstraintCount;
//             }
//             else
//             {
//                 var width = rectTransform.rect.size.x;
//                 var cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));
//                 minRows = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX);
//             }
//
//             var minSpace = padding.vertical + (CellSize.y + Spacing.y) * minRows - Spacing.y;
//             SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
//         }
//
//         public override void SetLayoutHorizontal()
//         {
//             SetCellsAlongAxis(0);
//         }
//
//         public override void SetLayoutVertical()
//         {
//             SetCellsAlongAxis(1);
//         }
//
//         private void SetCellsAlongAxis(int axis)
//         {
//             // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
//             // and only vertical values when invoked for the vertical axis.
//             // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
//             // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
//             // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.
//
//             if (axis == 0)
//             {
//                 // Only set the sizes when invoked for horizontal axis, not the positions.
//                 foreach (var rect in rectChildren)
//                 {
//                     m_Tracker.Add(this, rect,
//                         DrivenTransformProperties.Anchors |
//                         DrivenTransformProperties.AnchoredPosition |
//                         DrivenTransformProperties.SizeDelta);
//
//                     rect.anchorMin = Vector2.up;
//                     rect.anchorMax = Vector2.up;
//                     rect.sizeDelta = CellSize;
//                 }
//
//                 return;
//             }
//
//             var width = rectTransform.rect.size.x;
//             var height = rectTransform.rect.size.y;
//
//             var cellCountX = 1;
//             var cellCountY = 1;
//             if (_mConstraints == Constraint.FixedColumnCount)
//             {
//                 cellCountX = m_ConstraintCount;
//                 cellCountY = Mathf.CeilToInt(rectChildren.Count / (float)cellCountX - 0.001f);
//             }
//             else if (_mConstraints == Constraint.FixedRowCount)
//             {
//                 cellCountY = m_ConstraintCount;
//                 cellCountX = Mathf.CeilToInt(rectChildren.Count / (float)cellCountY - 0.001f);
//             }
//             else
//             {
//                 if (CellSize.x + Spacing.x <= 0)
//                     cellCountX = int.MaxValue;
//                 else
//                     cellCountX = Mathf.Max(1, Mathf.FloorToInt((width - padding.horizontal + Spacing.x + 0.001f) / (CellSize.x + Spacing.x)));
//
//                 if (CellSize.y + Spacing.y <= 0)
//                     cellCountY = int.MaxValue;
//                 else
//                     cellCountY = Mathf.Max(1, Mathf.FloorToInt((height - padding.vertical + Spacing.y + 0.001f) / (CellSize.y + Spacing.y)));
//             }
//
//             var cornerX = (int)StartCorner % 2;
//             var cornerY = (int)StartCorner / 2;
//
//             int cellsPerMainAxis, actualCellCountX, actualCellCountY;
//             if (StartAxis == Axis.Horizontal)
//             {
//                 cellsPerMainAxis = cellCountX;
//                 actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
//                 actualCellCountY = Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
//             }
//             else
//             {
//                 cellsPerMainAxis = cellCountY;
//                 actualCellCountY = Mathf.Clamp(cellCountY, 1, rectChildren.Count);
//                 actualCellCountX = Mathf.Clamp(cellCountX, 1, Mathf.CeilToInt(rectChildren.Count / (float)cellsPerMainAxis));
//             }
//
//             var requiredSpace = new Vector2(
//                     actualCellCountX * CellSize.x + (actualCellCountX - 1) * Spacing.x,
//                     actualCellCountY * CellSize.y + (actualCellCountY - 1) * Spacing.y
//                     );
//             var startOffset = new Vector2(
//                     GetStartOffset(0, requiredSpace.x),
//                     GetStartOffset(1, requiredSpace.y)
//                     );
//
//             for (var i = 0; i < rectChildren.Count; i++)
//             {
//                 int positionX;
//                 int positionY;
//                 if (StartAxis == Axis.Horizontal)
//                 {
//                     positionX = i % cellsPerMainAxis;
//                     positionY = i / cellsPerMainAxis;
//                 }
//                 else
//                 {
//                     positionX = i / cellsPerMainAxis;
//                     positionY = i % cellsPerMainAxis;
//                 }
//
//                 if (cornerX == 1)
//                     positionX = actualCellCountX - 1 - positionX;
//                 if (cornerY == 1)
//                     positionY = actualCellCountY - 1 - positionY;
//
//                 float realSizeX;
//                 float realSizeY;
//
//                 switch (ExpandSettings)
//                 {
//                     case ExpandSetting.X:
//                         realSizeX = (width - padding.left - padding.right - Spacing[0] * (actualCellCountX - 1)) / actualCellCountX;
//                         realSizeY = PreserveAspectRatio 
//                             ? realSizeX * CellSize[1] / CellSize[0]
//                             : CellSize[1];
//                         break;
//                     case ExpandSetting.Y:
//                         realSizeY = rectChildren.Count != 1
//                             ? (height - Spacing[0] * (actualCellCountY - 1)) / actualCellCountY
//                             : (height / ConstraintCount - Spacing[1] * (actualCellCountY - 1)) / actualCellCountY;
//                         realSizeX = PreserveAspectRatio 
//                             ? realSizeY * CellSize[0] / CellSize[1] 
//                             : CellSize[0];
//                         break;
//                     case ExpandSetting.Both:
//                         realSizeX = (width - Spacing[0] * (actualCellCountX - 1)) / actualCellCountX;
//                         realSizeY = rectChildren.Count != 1
//                             ? (height - Spacing[0] * (actualCellCountY - 1)) / actualCellCountY
//                             : (height / ConstraintCount - Spacing[1] * (actualCellCountY - 1)) / actualCellCountY;
//                         break;
//                     case ExpandSetting.None:
//                         realSizeX = CellSize[0];
//                         realSizeY = CellSize[1];
//                         break;
//                     default:
//                         realSizeX = CellSize[0];
//                         realSizeY = CellSize[1];
//                         break;
//                 }
//                 
//                 SetChildAlongAxis(rectChildren[i], 0, startOffset.x + (realSizeX + Spacing[0]) * positionX, realSizeX);
//                 SetChildAlongAxis(rectChildren[i], 1, startOffset.y + (realSizeY + Spacing[1]) * positionY, realSizeY);
//             }
//         }
//     }
// }