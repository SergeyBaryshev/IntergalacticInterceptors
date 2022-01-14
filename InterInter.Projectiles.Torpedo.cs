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
		internal sealed class Torpedo : Projectiles
		{
			internal Torpedo(Ships ship) : base(ship, Weapons.Arsenal.RocketLauncher) { }

			public override void Interact(Entity agent, params object[] args) { }

			internal override void Dispose() { }

			internal override void Update()
			{
				base.CommonBehavior();
			}
		}
	}
}