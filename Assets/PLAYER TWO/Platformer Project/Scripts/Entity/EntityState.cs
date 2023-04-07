using UnityEngine;
using UnityEngine.Events;

namespace PLAYERTWO.PlatformerProject
{
	public abstract class EntityState<T> : MonoBehaviour where T : Entity<T>
	{
		public UnityEvent onEnter;
		public UnityEvent onExit;

		public float timeSinceEntered { get; private set; }

		public void Enter(T entity)
		{
			timeSinceEntered = 0;
			onEnter?.Invoke();
			OnEnter(entity);
		}

		public void Exit(T entity)
		{
			onExit?.Invoke();
			OnExit(entity);
		}

		public void Step(T entity)
		{
			OnStep(entity);
			timeSinceEntered += Time.deltaTime;
		}

		/// <summary>
		/// Called when this State is invoked.
		/// </summary>
		protected abstract void OnEnter(T entity);

		/// <summary>
		/// Called when this State changes to another.
		/// </summary>
		protected abstract void OnExit(T entity);

		/// <summary>
		/// Called every frame where this State is activated.
		/// </summary>
		protected abstract void OnStep(T entity);

		/// <summary>
		/// Called when the Entity is in contact with a collider.
		/// </summary>
		public abstract void OnContact(T entity, Collider other);
	}
}
