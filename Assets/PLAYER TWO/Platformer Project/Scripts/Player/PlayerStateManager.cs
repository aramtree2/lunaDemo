using System.Collections.Generic;
using UnityEngine;

namespace PLAYERTWO.PlatformerProject
{
	[RequireComponent(typeof(Player))]
	[AddComponentMenu("PLAYER TWO/Platformer Project/Player/States/Player State Manager")]
	public class PlayerStateManager : EntityStateManager<Player> {
		public List<PlayerState> m_states;

		protected override List<EntityState<Player>> GetStateList()
		{
			return new List<EntityState<Player>>(m_states);
		}
	}
}
