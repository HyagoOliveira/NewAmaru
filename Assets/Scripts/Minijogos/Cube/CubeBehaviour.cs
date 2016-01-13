using GamePlatform.Physics;
using UnityEngine;

namespace Minijogos
{    
	public class CubeBehaviour : GameplayItemComponent, IJumpableItem
	{
        public const float ANIM_SPEED = 5f;
        public const float ANIM_AMPLITUDE = 1f;   
               
        private float animationTime = 0f;

        public override void OnAnimation()
        {
            animationTime += Time.deltaTime;

            float lastVerticalPosition = transform.position.y;
            transform.position = InitialPosition + Vector3.up * 
                Mathf.PingPong(animationTime * ANIM_SPEED, ANIM_AMPLITUDE);

            float verticalDirection = Mathf.Sign(transform.position.y - lastVerticalPosition);

            if (verticalDirection < 0f && Vector3.Distance(InitialPosition, transform.position) < 0.1f)
            {
                StopAnimation();
                OnAnimationFinish();
            }
        }

        public override void StopAnimation()
        {
            transform.position = InitialPosition;
            animationTime = 0f;
            DoingAnimation = false;
            base.StopAnimation();
        }

        public override void OnAnimationFinish()
        {
            OnConfirmAction();
        }

        #region métodos da interface jumpable item
        //ativado quando Amaru pula sobre o cubo
        public virtual void OnJump()
        {
            //do nothing
        }

        //ativado quando Amaru cabecea o cubo
        public virtual void OnHead()
        {
            StartInteractionAnimation();
        }
        

        //valor da aceleração quando Amaru pula sobre o cubo
        public virtual float GetJumpHeight()
        {
            return 0;
        }
        #endregion
    }
}

