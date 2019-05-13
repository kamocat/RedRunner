using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RedRunner.Characters;

namespace RedRunner.Enemies
{

	public class Eye : MonoBehaviour
	{
		/* Can't serialize properties
		 * https://docs.unity3d.com/ScriptReference/SerializeField.html
		 * https://docs.unity3d.com/Manual/script-Serialization.html
		 */
		[SerializeField]
		protected float m_Radius = 1f;
		[SerializeField]
		protected Transform m_Pupil;
		[SerializeField]
		protected Transform m_Eyelid;
		[SerializeField]
		protected float m_MaximumDistance = 5f;
		[SerializeField]
		protected Character m_LatestCharacter;
		[SerializeField]
		protected Vector3 m_InitialPosition;
		[SerializeField]
		protected float m_Speed = 0.01f;
		[SerializeField]
		protected float m_DeadSpeed = 0.005f; // What is this???
		[SerializeField]
		protected Vector3 m_DeadPosition;
		protected Vector3 m_PupilDestination;
		
		/* We don't need a method to dynamically change the size of the eyes.
		 * This should be in the initializer.
		 */
		public virtual float Radius {
			get {
				return m_Radius;
			}
			set {
				m_Radius = value;
			}
		}

		public virtual Transform Pupil {
			get {
				return m_Pupil;
			}
		}

		public virtual Vector3 InitialPosition {
			get {
				return m_InitialPosition;
			}
		}

		public virtual Vector3 PupilDestination {
			get {
				return m_PupilDestination;
			}
		}

		public virtual float Speed {
			get {
				return m_Speed;
			}
		}

		protected virtual void Awake ()
		{
//			m_InitialPosition = m_Pupil.transform.position;
		}


		protected virtual void Update ()
		{
			/* This is weird.
			 * Why get a list of all characters, and only return one?
			 * What is the order of the list? Will it also return other enemies?
			 * Given that this only controls eye movement, this isn't a critical portion of the game.
			 */
			Collider2D [] colliders = Physics2D.OverlapCircleAll ( transform.parent.position, m_MaximumDistance, LayerMask.GetMask ( "Characters" ) );
			for ( int i = 0; i < colliders.Length; i++ )
			{
				Character character = colliders [ i ].GetComponent<Character> ();
				if ( character != null )
				{
					m_LatestCharacter = character;
				}
			}
			SetupPupil (); // Update the position of the eye
		}
		
		/* No idea what this is for. Do we need it? */
		protected virtual void OnDrawGizmos ()
		{
			Gizmos.DrawWireSphere ( transform.position, m_Radius );
			Gizmos.DrawWireSphere ( transform.parent.position, m_MaximumDistance );
		}

		/* This function updates where the eye points.
		 * In general, I think it's overly complicated. The eyes don't need to have a "movement speed"
		 * since their maximum movement speed will be limited by Red's maximum travel speed.
		 */
		protected virtual void SetupPupil ()
		{
			if ( m_LatestCharacter != null )
			{
				float speed = m_Speed;
				Vector3 distanceToTarget = m_LatestCharacter.transform.position - m_Pupil.position;
				if ( m_LatestCharacter.IsDead.Value )
				{
					/* What problem does this fix?
					 * Do the eyes go crazy when the player dies?
					 * Maybe we don't need to update the pupil position at all after a player dies? '*/
					speed = m_DeadSpeed;
					distanceToTarget = Vector3.ClampMagnitude ( m_DeadPosition, m_Radius );
					Vector3 finalPupilPosition = transform.position + distanceToTarget; // Needless extra variable.
					m_PupilDestination = finalPupilPosition;
				}
				else
				{
					float distance = Vector3.Distance ( m_LatestCharacter.transform.position, transform.parent.position );
					if ( distance <= m_MaximumDistance )
					{
						distanceToTarget = Vector3.ClampMagnitude ( distanceToTarget, m_Radius );
					}
					else
					{
						distanceToTarget = Vector3.ClampMagnitude ( m_InitialPosition, m_Radius );
					}
					Vector3 finalPupilPosition = transform.position + distanceToTarget; // Needless extra variable
					m_PupilDestination = finalPupilPosition;
				}
				/* Why make the eyes "move towards" the character? They should be fast enough to always track.
				 * This adds extra complexity that isn't appreciated when playing the game. */
				m_Pupil.position = Vector3.MoveTowards ( m_Pupil.position, m_PupilDestination, speed );
			}
		}

	}

}