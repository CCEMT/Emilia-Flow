using UnityEngine;

namespace Emilia.Statescript
{
    public class PlayerBullet : MonoBehaviour
    {
        public MeshRenderer meshRenderer;

        public float speed = 10;
        public Color color;

        private Vector3 moveDirection;

        private void Awake()
        {
            this.meshRenderer.material.color = color;
        }

        public void Init(Vector3 moveDirection)
        {
            this.moveDirection = moveDirection;
            gameObject.SetActive(true);
            Destroy(gameObject, 3);
        }

        private void Update()
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }
}