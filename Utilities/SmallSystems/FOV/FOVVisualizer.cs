using System.Collections.Generic;
using UnityEngine;

namespace SpookyCore.Utilities
{
    public class FOVVisualizer : MonoBehaviour
    {
        #region Stucts & Enums

        private struct ViewCastInfo
        {
            public bool Hit;
            public Vector3 Point;
            public float Distance;
            public float Angle;

            public ViewCastInfo(bool hit, Vector3 point, float distance, float angle)
            {
                Hit = hit;
                Point = point;
                Distance = distance;
                Angle = angle;
            }
        }

        private struct EdgeInfo
        {
            public Vector3 PointA;
            public Vector3 PointB;

            public EdgeInfo(Vector3 pointA, Vector3 pointB)
            {
                PointA = pointA;
                PointB = pointB;
            }
        }

        #endregion

        #region Fields
        
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private LayerMask _targetMask;
        [SerializeField] private LayerMask _obstacleMask;
        
        [field: SerializeField] public float _viewRadius = 6;
        [field: SerializeField] public float _viewAngle = 90;
        [SerializeField] private float _scannerDelay = 0.2f;
        [SerializeField] [Range(0.1f, 1f)] private float _meshResolution = 0.5f;
        [SerializeField] private int _edgeResolution = 3;
        [SerializeField] private float _edgeDstThreshold = 0.05f;
        
        private Mesh _mesh;
        private Collider2D[] _targetCache;
        public List<Transform> _visibleTargets;
        private float _findTargetTimer;

        #endregion

        #region MyRegion

        private void Start()
        {
            _mesh = new Mesh
            {
                name = "FOV Mesh"
            };
            _meshFilter.mesh = _mesh;
            _targetCache = new Collider2D[30];
        }

        private void LateUpdate()
        {
            _findTargetTimer -= Time.deltaTime;
            if (_findTargetTimer <= 0)
            {
                _findTargetTimer = _scannerDelay;
                FindVisibleTargets();
            }
            DrawFOV();
        }

        #endregion

        #region Public Methods

        public Vector3 AngleToDirection(float angleInDegrees, bool isGlobalAngle)
        {
            if (!isGlobalAngle)
            {
                angleInDegrees += transform.eulerAngles.z;
            }
            var radianAngle = angleInDegrees * Mathf.Deg2Rad;
            return new Vector3(-Mathf.Sin(radianAngle), Mathf.Cos(radianAngle), 0);
        }

        #endregion
        
        #region Private Methods

        private void DrawFOV()
        {
            var stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
            var stepAngleSize = _viewAngle / stepCount;

            var viewPoints = new List<Vector3>();
            var oldViewCast = new ViewCastInfo();

            for (var i = 0; i <= stepCount; ++i)
            {
                var angle = transform.eulerAngles.z - _viewAngle / 2 + stepAngleSize * i;
                var viewCast = ViewCast(angle);

                if (i > 0)
                {
                    var edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.Distance - viewCast.Distance) > _edgeDstThreshold;
                    if (oldViewCast.Hit != viewCast.Hit || (oldViewCast.Hit && edgeDstThresholdExceeded))
                    {
                        var edge = FindEdge(oldViewCast, viewCast);
                        if (edge.PointA != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointA);
                        }

                        if (edge.PointB != Vector3.zero)
                        {
                            viewPoints.Add(edge.PointB);
                        }
                    }
                }
                
                viewPoints.Add(viewCast.Point);
                oldViewCast = viewCast;
            }

            var vertexCount = viewPoints.Count + 1;
            var vertices = new Vector3[vertexCount];
            var triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;
            for (var i = 0; i < vertexCount - 1; ++i)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }
            
            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;
            _mesh.RecalculateNormals();
        }
        
        private void FindVisibleTargets()
        {
             _visibleTargets.Clear();
             var size = Physics2D.OverlapCircleNonAlloc(transform.position, _viewRadius, _targetCache, _targetMask);
             
             for (var i = 0; i < size; i++)
             {
                 var targetCollider = _targetCache[i];
                 var target = targetCollider.transform;
                 var dirToTarget = (target.position - transform.position).normalized;

                 if (!(Vector3.Angle(transform.up, dirToTarget) < _viewAngle / 2)) continue;
                 
                 var dstToTarget = Vector3.Distance(transform.position, target.position);
                 if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                 {
                     _visibleTargets.Add(target);
                 }
             }
        }

        private ViewCastInfo ViewCast(float globalAngle)
        {
            var dir = AngleToDirection(globalAngle, true);
            var hit = Physics2D.Raycast(transform.position, dir, _viewRadius, _obstacleMask);

            return hit.transform != null 
                ? new ViewCastInfo(true, hit.point, hit.distance, globalAngle) 
                : new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius, globalAngle);
        }
        
        private EdgeInfo FindEdge(ViewCastInfo min, ViewCastInfo max)
        {
            var minAngle = min.Angle;
            var maxAngle = max.Angle;
            var minPoint = Vector3.zero;
            var maxPoint = Vector3.zero;

            for (var i = 0; i < _edgeResolution; ++i)
            {
                var angle = (minAngle + maxAngle) / 2;
                var newViewCast = ViewCast(angle);
                
                var edgeDstThresholdExceeded = Mathf.Abs(min.Distance - newViewCast.Distance) > _edgeDstThreshold;
                if (newViewCast.Hit == min.Hit && !edgeDstThresholdExceeded)
                {
                    minAngle = angle;
                    minPoint = newViewCast.Point;
                }
                else
                {
                    maxAngle = angle;
                    maxPoint = newViewCast.Point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        #endregion
    }
}
