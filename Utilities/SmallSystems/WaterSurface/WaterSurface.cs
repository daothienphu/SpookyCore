using UnityEngine;

namespace SpookyCore.Utilities
{
    public class WaterSurface : MonoBehaviour
    {
        #region Structs & Enums

        private struct WaterColumn
        {
            public float TargetHeight;
            public float Height;
            public float Speed;

            public void Update(float dampening, float tension)
            {
                var x = TargetHeight - Height;
                Speed += tension * x - Speed * dampening;
                Height += Speed;
            }
        }

        #endregion

        #region Fields

        public WaterSurfaceSettings WaterSurfaceSettings;
        [SerializeField] private Material _material;
        
        private int _columnCount;
        private float _columnWidth;
        private float _columnHeight;
        private float _dampening;
        private float _tension;
        private float _spread;
        private int _ripplePasses;
        
        private WaterColumn[] _columnsData;
        
        private Mesh _mesh;
        private Vector3[] _vertices;
        private int[] _triangles;
        private float[] _lDeltas;
        private float[] _rDeltas;

        #endregion

        #region Life Cycle

        private void Start()
        {
            InitParams();
            
            //Handle rendering
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshRenderer>().material = _material;
            _vertices = new Vector3[_columnCount * 4];
            _triangles = new int[_columnCount * 6];
            GenerateMesh();
            UpdateMesh();
            
            //Handle Logic
            for (var i = 0; i < _columnsData.Length; i++)
            {
                _columnsData[i] = new WaterColumn
                {
                    Height = _columnHeight,
                    TargetHeight = _columnHeight,
                    Speed = 0
                };
            }
        }

        private void Update()
        {
            for (var i = 0; i < _columnsData.Length; i++)
            {
                _columnsData[i].Update(_dampening,_tension);
            }
            RippleLogic();
            UpdateTopVerticesVisual();
        }

        #endregion

        #region Public Methods

        public void PreviewMeshInEditor()
        {
            _columnCount = WaterSurfaceSettings.ColumnCount;
            _columnWidth = WaterSurfaceSettings.ColumnWidth;
            _columnHeight = WaterSurfaceSettings.ColumnHeight;

            _columnsData = new WaterColumn[_columnCount];

            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            GetComponent<MeshRenderer>().material = _material;
            _vertices = new Vector3[_columnCount * 4];
            _triangles = new int[_columnCount * 6];
            GenerateMesh();
            UpdateMesh();
        }

        public void UpdateWaterParamsInEditor()
        {
            _dampening = WaterSurfaceSettings.Dampening;
            _tension = WaterSurfaceSettings.Tension;
            _spread = WaterSurfaceSettings.Spread;
            _ripplePasses = WaterSurfaceSettings.RipplePasses;
        }
        
        public float GetSurfaceLevel()
        {
            return transform.position.y + _columnHeight;
        }
        
        /// <summary>
        /// Create a starting splash at xPosition which will ripples toward both sides.
        /// The higher the magnitude of the speed, the bigger the starting splash.
        /// speed can take a negative value.
        /// </summary>
        /// <param name="xPosition"></param>
        /// <param name="speed"></param>
        public void CreateSplashAt(float xPosition, float speed)
        {
            var (lIndex, rIndex)  = GetIndexFromPositionX(xPosition);
            
            _columnsData[lIndex].Speed += speed;
            _columnsData[rIndex].Speed += speed;

            if (lIndex > 0) _columnsData[lIndex - 1].Speed += speed * 0.85f;
            if (rIndex < _columnCount - 1) _columnsData[rIndex + 1].Speed += speed * 0.85f;
        }

        public void MoveWaterToDesiredHeight(float height)
        {
            //var originalSurfacePosition = transform.position;
            transform.position = new Vector3(
                transform.position.x,
                height - WaterSurfaceSettings.ColumnHeight, 
                -1f);
        }

        #endregion

        #region Private Methods

        private void InitParams()
        {
            _columnCount = WaterSurfaceSettings.ColumnCount;
            _columnWidth = WaterSurfaceSettings.ColumnWidth;
            _columnHeight = WaterSurfaceSettings.ColumnHeight;
            _dampening = WaterSurfaceSettings.Dampening;
            _tension = WaterSurfaceSettings.Tension;
            _spread = WaterSurfaceSettings.Spread;
            _ripplePasses = WaterSurfaceSettings.RipplePasses;
            
            _columnsData = new WaterColumn[_columnCount];
            _lDeltas = new float[_columnCount];
            _rDeltas = new float[_columnCount];
        }
        
        private (int, int) GetIndexFromPositionX(float xPosition)
        {
            var transformX = transform.position.x;
            var lowerIndex =
                Mathf.FloorToInt(Mathf.Clamp((xPosition - transformX) / _columnWidth, 0, _columnCount - 1));
            return (lowerIndex, lowerIndex < _columnCount - 1 ? lowerIndex + 1 : lowerIndex);
        }

        private void RippleLogic()
        {
            for (var j = 0; j < _ripplePasses; j++)
            {
                for (var i = 0; i < _columnsData.Length; i++)
                {
                    if (i > 0)
                    {
                        _lDeltas[i] = _spread * (_columnsData[i].Height - _columnsData[i - 1].Height);
                        _columnsData[i - 1].Speed += _lDeltas[i];
                    }
                    
                    if (i < _columnsData.Length - 1)
                    {
                        _rDeltas[i] = _spread * (_columnsData[i].Height - _columnsData[i + 1].Height);
                        _columnsData[i + 1].Speed += _rDeltas[i];
                    }
                }
            
                for (var i = 0; i < _columnsData.Length; i++)
                {
                    if (i > 0)
                    {
                        _columnsData[i - 1].Height += _lDeltas[i];
                    }
            
                    if (i < _columnsData.Length - 1)
                    {
                        _columnsData[i + 1].Height += _rDeltas[i];
                    }
                }
            }
        }
        
        private void GenerateMesh()
        {
            for (var i = 0; i < _columnCount; i++)
            {
                //Calculate the base index for the vertices
                var vertexIndex = i * 4;

                //Bottom vertices
                _vertices[vertexIndex + 0] = new Vector3(i * _columnWidth, 0, 0);
                _vertices[vertexIndex + 1] = new Vector3((i + 1) * _columnWidth, 0, 0);

                //Top vertices (initially positioned at height)
                _vertices[vertexIndex + 2] = new Vector3(i * _columnWidth, _columnHeight, 0);
                _vertices[vertexIndex + 3] = new Vector3((i + 1) * _columnWidth, _columnHeight, 0);

                //Define the triangles (2 per trapezoid)
                var triangleIndex = i * 6;

                //Triangle 1 (Bottom left, top left, top right)
                _triangles[triangleIndex + 0] = vertexIndex + 0;
                _triangles[triangleIndex + 1] = vertexIndex + 2;
                _triangles[triangleIndex + 2] = vertexIndex + 3;

                //Triangle 2 (Bottom left, top right, bottom right)
                _triangles[triangleIndex + 3] = vertexIndex + 0;
                _triangles[triangleIndex + 4] = vertexIndex + 3;
                _triangles[triangleIndex + 5] = vertexIndex + 1;
            }
        }

        private void UpdateMesh()
        {
            _mesh.Clear();
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            _mesh.RecalculateNormals();
        }
        
        private void UpdateTopVerticesVisual()
        {
            for (var i = 0; i < _columnCount; ++i)
            {
                var vertexIndex = i * 4;

                //Top Left, Top Right
                _vertices[vertexIndex + 2].y = _columnsData[i].Height;
                _vertices[vertexIndex + 3].y = i < _columnCount - 1 
                    ? _columnsData[i + 1].Height 
                    : _columnsData[i].Height;
            }

            UpdateMesh();
        }

        #endregion
    }
}

