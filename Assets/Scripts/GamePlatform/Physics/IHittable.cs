namespace GamePlatform.Physics
{
	public interface IHittable
	{
		void OnHit();

		void OnHitByObject();

		void OnThrowAway();

	}
}

