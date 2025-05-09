using UnityEngine;

namespace SpookyCore.Utilities
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private void Update()
        {
            var worldPoint = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).normalized;
            var velocity = worldPoint * _speed;

            var diff = (GameCache.Camera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            var zRotation = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, zRotation - 90);
            transform.position += velocity * Time.deltaTime;
        }
    }
}