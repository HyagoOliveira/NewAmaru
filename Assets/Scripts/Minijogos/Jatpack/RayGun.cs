using System.Collections;
using UnityEngine;

namespace Minijogos
{
    [RequireComponent(typeof(AudioSource))]
	public class RayGun : GameplayToolComponent
	{
		public Transform rayTransform;
		public LayerMask hitLayer;
        public GameObject bullet;

        private AudioSource audioSource;
        private LineRenderer rayRenderer;
        private RaycastHit hit;
        private bool canFire = true;

        private const float RAY_DISTANCE = 100f;

		protected override void Start()
        {
            base.Start();
            amaru = Amaru.Instance as Amaru;
            audioSource = GetComponent<AudioSource>();
            rayRenderer = rayTransform.GetComponent<LineRenderer>();
            if (rayRenderer == null)
                throw new UnityException("Ray Render is null.");

            Catch();
        }

        private void Update()
        {
            LockOff();
        	
        	if(Physics.Raycast(rayTransform.position, Vector3.right, out hit, 
                RAY_DISTANCE, hitLayer))
        	{
                LockOn();
            }

            if (Amaru.Instance.input.GetButtonDown("Action") && canFire)
            {
                Fire();
            }
        }

        private void Fire()
        {
            amaru.input.SetVibration(1f);
            amaru.animator.SetTrigger("fire");
            audioSource.Play();
            fireBullet();            

            StartCoroutine(RicochetCoroutine());
        }

        private void fireBullet()
        {
            GameObject bInstance = Instantiate(bullet, rayTransform.position, Quaternion.identity) 
                as GameObject;
            RayBullet rayBulletInstance = bInstance.GetComponent<RayBullet>();
            rayBulletInstance.direction = rayTransform.forward;
            
            //check if the bullet will colide with choice
            if (hit.transform != null)
            {
                float timeToColide = hit.distance / rayBulletInstance.speed;
                Destroy(rayBulletInstance.gameObject, timeToColide);

                GameplayItemComponent choiceInstance = hit.transform.GetComponent<GameplayItemComponent>();
                if (choiceInstance != null && !choiceInstance.DoingAnimation)
                {
                    StartCoroutine(BulletHitCoroutine(timeToColide, choiceInstance));
                }
            }
            else
            {
                Destroy(rayBulletInstance.gameObject, rayBulletInstance.timeAlive);
            }
        }

        private IEnumerator RicochetCoroutine()
        {
            canFire = false;
            rayTransform.gameObject.SetActive(canFire);
            //animação de tiro tem 15 frames
            yield return new WaitForSeconds(0.5f);
            canFire = true;
            rayTransform.gameObject.SetActive(canFire);
        }

        private IEnumerator BulletHitCoroutine(float time, GameplayItemComponent component)
        {
            yield return new WaitForSeconds(time);
            component.StartInteractionAnimation();
        }

        private void LockOn()
        {
            rayRenderer.SetPosition(1, Vector3.forward * Vector3.Distance(hit.point, rayTransform.position));
            rayRenderer.SetColors(Color.red, Color.red);
        }

        private void LockOff()
        {
            rayRenderer.SetPosition(1, Vector3.forward * RAY_DISTANCE);
            rayRenderer.SetColors(Color.green, Color.green);
        }

        public override void Drop()
        {
            transform.SetParent(null);
            Destroy(this.gameObject);
        }

        public override void Catch()
        {
            transform.SetParent(amaru.gunPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}