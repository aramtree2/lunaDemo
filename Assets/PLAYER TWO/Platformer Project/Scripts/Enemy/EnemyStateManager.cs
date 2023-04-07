using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(Enemy))]
	[AddComponentMenu("PLAYER TWO/Platformer Project/Enemy/Enemy State Manager")]
	public class EnemyStateManager : EntityStateManager<Enemy>
	{
		public List<EnemyState> m_states;

		protected override List<EntityState<Enemy>> GetStateList()
		{
			return new List<EntityState<Enemy>>(m_states);
		}
	}
}
