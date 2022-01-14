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
		internal sealed class Spray : Projectiles
		{
			internal Spray(Ships ship) : base(ship, Weapons.Arsenal.PlasmaGun, (float)(System.Math.Sin((float)Variants.Imitator.Physics.CurrentTime.TotalSeconds * 3) / 20)) { }

			public override void Interact(Entity agent, params object[] args) { }

			internal override void Dispose() { }

			internal override void Update()
			{
				base.CommonBehavior();
			}
		}
	}
}