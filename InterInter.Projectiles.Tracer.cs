using Imitator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntergalacticInterceptors
{
	partial class Projectiles
	{
		internal sealed class Tracer : Projectiles
		{
			internal Tracer(Ships ship) : base(ship, Weapons.Arsenal.RocketLauncher) { }

			public override void Interact(Entity agent, params object[] args) { }

			internal override void Dispose() { }

			internal override void Update()
			{
				base.CommonBehavior();
				System.Numerics.Vector3 direction = this.Physic.Node.Position - Entities.Sight.Position3D;
				this.Physic.Node.Velocity = -System.Numerics.Vector3.Normalize(direction) * this.Physic.Node.Velocity.Length();
				this.Physic.Node.Orientation = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, (float)System.Math.Atan2(-direction.X, -direction.Z));
			}
		}
	}
}