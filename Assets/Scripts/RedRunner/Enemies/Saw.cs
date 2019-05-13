using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Characters;

namespace RedRunner.Enemies
{

	public class Saw : Enemy
	{
		/* Can't serialize properties
		 * https://docs.unity3d.com/ScriptReference/SerializeField.html
		 * https://docs.unity3d.com/Manual/script-Serialization.html
		 */
		[SerializeField]
		private Collider2D m_Collider2D;
		[SerializeField]
		private Transform targetRotation;
		[SerializeField]
		/* One of the saws definitely speeds up, but I don't see that parameter here. 
		 * It's the big saw that chases you.
		 * I also don't see a parameter for the saw size. */
		private float m_Speed = 1f;
		[SerializeField]
		private bool m_RotateClockwise = false;
		[SerializeField]
		private AudioClip m_DefaultSound;
		[SerializeField]
		private AudioClip m_SawingSound;
		[SerializeField]
		private AudioSource m_AudioSource;

		public override Collider2D Collider2D {
			get {
				return m_Collider2D;
			}
		}

		void Start ()
		{
			if (targetRotation == null) {
				targetRotation = transform;
			}
		}

		void Update ()
		{
			Vector3 rotation = targetRotation.rotation.eulerAngles;
			if (!m_RotateClockwise) {
				rotation.z += m_Speed;
			} else {
				rotation.z -= m_Speed;
			}
			targetRotation.rotation = Quaternion.Euler (rotation);
		}

		void OnCollisionEnter2D (Collision2D collision2D)
		{
			Character character = collision2D.collider.GetComponent<Character> ();
			if (character != null) {
				Kill (character);
			}
		}

		void OnCollisionStay2D (Collision2D collision2D)
		{
			if (collision2D.collider.CompareTag ("Player")) {
				if (m_AudioSource.clip != m_SawingSound) {
					m_AudioSource.clip = m_SawingSound;
				} else if (!m_AudioSource.isPlaying) {
					/* Why the "else"? Does changing the clip automatically play?
					 * We should cause it to play the first time through, rather than needing two calls.
					 * Or, we should start it playing in the "OnCollisionEnter" rather than the CollisionStay */
					m_AudioSource.Play ();
				}
			}
		}

		void OnCollisionExit2D (Collision2D collision2D)
		{
			if (collision2D.collider.CompareTag ("Player")) {
				if (m_AudioSource.clip != m_DefaultSound) {
					m_AudioSource.clip = m_DefaultSound;
				}
				m_AudioSource.Play ();
			}
		}

		public override void Kill (Character target)
		{
			target.Die (true);
		}

	}

}