using UnityEngine;

namespace Emilia.Statescript
{
    public class Player : MonoBehaviour
    {
        public CharacterController characterController;
        public MeshRenderer meshRenderer;
        public PlayerBullet normalBullet;
        public PlayerBullet heavyBullet;

        public float speed = 5.0f;
        public Vector2 moveDirection = Vector2.zero;

        void Update()
        {
            Vector3 direction = new Vector3(this.moveDirection.x, 0, this.moveDirection.y);
            characterController.Move(direction * speed * Time.deltaTime);
            if (direction != Vector3.zero) transform.forward = direction;
        }

        public void Attack(bool isHeavy)
        {
            PlayerBullet bulletPrefab = isHeavy ? heavyBullet : normalBullet;

            Vector3 position = transform.position + transform.forward.normalized * 1f;
            PlayerBullet bullet = Instantiate(bulletPrefab, position, transform.rotation);
            bullet.Init(transform.forward.normalized);
        }

        public void SetColor(Color color)
        {
            meshRenderer.material.color = color;

        }
    }
}