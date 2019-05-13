using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* I think this is included so we can call the kill() method of the character.
 * This is ok since there is only one character, but might lead to issues if we
 * introduce additional playable characters. */
using RedRunner.Characters;

namespace RedRunner.Enemies
{

	/* MonoBehaviour seems to be required for all Unity scripts.
	 * https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	 */
	public abstract class Enemy : MonoBehaviour
	{
		// Every enemy must have these two methods defined
		public abstract Collider2D Collider2D { get; }

		public abstract void Kill ( Character target );

	}

}