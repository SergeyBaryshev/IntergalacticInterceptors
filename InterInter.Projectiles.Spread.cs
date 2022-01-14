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
		internal sealed class Spread : Projectiles
		{
			internal Spread(Ships ship) : base(ship, Weapons.Arsenal.RocketLauncher)
			{
				for (int index = -2; index <= +2; index += 1)
					if (index != 0)
						_ = new Spread(ship, (float)(index * System.Math.PI / 16));
			}

			private Spread(Ships ship, float angle) : base(ship, Weapons.Arsenal.RocketLauncher, angle) { }

			public override void Interact(Entity agent, params object[] args) { }

			internal override void Dispose() { }

			internal override void Update()
			{
				base.CommonBehavior();
			}
		}
	}
}