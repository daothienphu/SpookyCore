// using UnityEngine;
// using UnityEngine.UI;
//
// namespace SpookyCore.Utilities.UI
// {
//     [ExecuteInEditMode]
//     [RequireComponent(typeof(GridLayoutGroup))]
//     public class AdjustGridLayoutCellSize : MonoBehaviour
//     {
//         public enum Axis
//         {
//             X,
//             Y
//         };
//
//         public enum RatioMode
//         {
//             Free,
//             Fixed
//         };
//
//         [SerializeField] private Axis _expand;
//         [SerializeField] private RatioMode _ratioMode;
//         [SerializeField] private float _cellRatio = 1;
//
//         private new RectTransform transform;
//         private GridLayoutGroup _grid;
//
//         private void Awake()
//         {
//             transform = (RectTransform)base.transform;
//             _grid = GetComponent<GridLayoutGroup>();
//         }
//         
//         private void Start()
//         {
//             UpdateCellSize();
//         }
//
//         private void OnRectTransformDimensionsChange()
//         {
//             UpdateCellSize();
//         }
//
// #if UNITY_EDITOR
//         [ExecuteAlways]
//         void Update()
//         {
//             UpdateCellSize();
//         }
// #endif
//
//         void OnValidate()
//         {
//             transform = (RectTransform)base.transform;
//             _grid = GetComponent<GridLayoutGroup>();
//             UpdateCellSize();
//         }
//
//         void UpdateCellSize()
//         {
//             if (!_grid) return;
//             var count = _grid.constraintCount;
//             if (_expand == Axis.X)
//             {
//                 var spacing = (count - 1) * _grid.spacing.x;
//                 var contentSize = transform.rect.width - _grid.padding.left - _grid.padding.right - spacing;
//                 var sizePerCell = contentSize / count;
//                 _grid.cellSize = new Vector2(sizePerCell, _ratioMode == RatioMode.Free ? _grid.cellSize.y : sizePerCell * _cellRatio);
//
//             }
//             else //Axis.Y
//             {
//                 var spacing = (count - 1) * _grid.spacing.y;
//                 var contentSize = transform.rect.height - _grid.padding.top - _grid.padding.bottom - spacing;
//                 var sizePerCell = contentSize / count;
//                 _grid.cellSize = new Vector2(_ratioMode == RatioMode.Free ? _grid.cellSize.x : sizePerCell * _cellRatio, sizePerCell);
//             }
//         }
//     }
// }