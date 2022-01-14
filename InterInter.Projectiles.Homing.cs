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
		internal sealed class Homing : Projectiles
		{
			internal Homing(Ships ship) : base(ship, Weapons.Arsenal.RocketLauncher) { }

			public override void Interact(Entity agent, params object[] args) { }

			internal override void Dispose() { }

			internal override void Update()
			{
				base.CommonBehavior();
				System.Collections.Generic.List<Ships.Enemy> enemies = Ships.Enemy.List;
				if (enemies.Count > 0)
				{
					var closestDistance = new System.Numerics.Vector3(1000);
					int closestIndex = -1;
					for (int currentIndex = enemies.Count - 1; currentIndex >= 0; currentIndex -= 1)
					{
						if (!enemies[currentIndex].Dead)
						{
							System.Numerics.Vector3 currentDistance = this.Physic.Node.Position - enemies[currentIndex].Physic.Node.Position;
							if (currentDistance.LengthSquared() < closestDistance.LengthSquared())
							{
								closestDistance = currentDistance;
								closestIndex = currentIndex;
							}
						}
					}
					if (closestIndex > -1)
					{
						this.Physic.Node.Velocity = -System.Numerics.Vector3.Normalize(closestDistance) * this.Physic.Node.Velocity.Length();
						this.Physic.Node.Orientation = System.Numerics.Quaternion.CreateFromAxisAngle(System.Numerics.Vector3.UnitY, (float)System.Math.Atan2(-closestDistance.X, -closestDistance.Z));
						Variants.Imitator.Scene.Camera camera = Variants.Imitator.Scene.Camera.Default;
						closestDistance = camera.Project(enemies[closestIndex].Physic.Node.Position);
						camera.DrawSprite((InterInter.Randomizer.Next(2) == 0 ? Entities.Sight.TargetMaterial : Entities.Sight.DefaultMaterial), Variants.Imitator.Element.Material.TextureType.Base, System.Drawing.RectangleF.FromLTRB(0, 0, 1, 1), new System.Drawing.Rectangle((int)(closestDistance.X - Entities.Sight.Size2D.X), (int)(closestDistance.Y - Entities.Sight.Size2D.Y), (int)(Entities.Sight.Size2D.X * 2), (int)(Entities.Sight.Size2D.Y * 2)), System.Numerics.Vector3.Zero, System.Drawing.Color.White);
					}
				}
			}
		}
	}
}