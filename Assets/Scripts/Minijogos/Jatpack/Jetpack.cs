using System.Collections;
using UnityEngine;

namespace Minijogos
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(AudioSource))]
    public class Jetpack : GameplayToolComponent
	{
		public float horizontalGap = 2f;
        public float animationAmplitude = 0.8f;
        public ParticleSystem tail;

		private Vector2 maxPoint;
		private Vector2 minPoint;
        private SphereCollider sphereCollider;
        private AudioSource audioSource;		

		protected override void Start()
        {
            base.Start();
        	sphereCollider = GetComponent<SphereCollider>();
            audioSource = GetComponent<AudioSource>();
            tail.Pause();
            maxPoint = new Vector2(transform.position.x + horizontalGap, transform.position.y + 100f);
        	minPoint = new Vector2(transform.position.x - horizontalGap, transform.position.y);
        }

        void Update()
        {
        	if(Catched)
        	{
        		Vector3 amaruInput = new Vector3(amaru.input.GetAxis("Horizontal"), amaru.input.GetAxis("Vertical"));
        		Vector3 newPos = amaru.transform.position + amaruInput * speed * Time.deltaTime;
        		newPos.x = Mathf.Clamp(newPos.x, minPoint.x, maxPoint.x);
        		newPos.y = Mathf.Clamp(newPos.y, minPoint.y, maxPoint.y);

        		amaru.transform.position = newPos;
        	}
            else if (Exiting)
            {
                transform.position += Vector3.up * speed * Time.deltaTime;
            }
            else // floating (waiting to be Catched)
            {
                transform.position = InicitalPos + Vector3.up * Mathf.PingPong(Time.time, animationAmplitude) * 0.2f;
                transform.Rotate(Vector3.forward* Time.deltaTime * 24f);
            }        	
        }

        public override void Drop()
        {
            amaru.TurnOn();
            amaru.animator.SetBool("jetpack", false);
            transform.SetParent(null);
            Catched = false;            
            Exiting = true;
            StartCoroutine(FadeAudioAndDestroyCoroutine());
        }

        private IEnumerator FadeAudioAndDestroyCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                if (audioSource.volume > 0)
                {
                    audioSource.volume -= Time.deltaTime;
                }
                else
                {
                    break;
                }                
            }

            Destroy(this.gameObject);
        }
        

        public void SetTopPoint(float y)
        {
        	maxPoint.y = y;
        }


        public override void Catch()
        {
        	if(amaru == null)
        		throw new UnityException("Amaru é nulo!");

        	
        	amaru.TurnOff();
            amaru.animator.SetBool("jetpack", true);
            amaru.transform.eulerAngles = Vector3.up * 180f;
        	transform.SetParent(amaru.jetpackPoint);
        	transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            tail.Play();
            audioSource.Play();
        	Catched = true;
            GerenciadorTarefas.Instance.CurrentMinijogoGameplay.OnGetItemTool(this);
        }


		private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag.Equals("Player")){
            	amaru = other.GetComponent<Amaru>();                
                sphereCollider.enabled = false;
                Catch();
            }
        }

        public override void OnPause()
        {
            base.OnPause();
            tail.Pause();
        }

        public override void OnResume()
        {
            base.OnResume();
            tail.Play();
        }
    }
}