//using System.Linq;
//using System.Drawing;
// these "usings" is not supported in script files!

public static class Common
{
	///<summary>Генератор псевдослучайных чисел.</summary>
	public static readonly System.Random Randomizer = new System.Random();

	///<summary>Percentage of age.</summary>
	///<param name="lt">Total life time in seconds.</param>
	///<param name="p">Current particle.</param>
	///<returns>Scale from 0F (born) to 1F (death) of particle.</returns>
	public static float Age(float lt, Variants.Imitator.Engine.Particle p)
	{
		return System.Math.Min(System.Math.Max(((float)Variants.Imitator.Physics.CurrentTime.TotalSeconds - p.Age) / lt, 0F), 1.0F);
	}

	///<summary>Clamps Int32 to range 0-255.</summary>
	///<param name="b">Int32.</param>
	///<returns>Byte as Int32.</returns>
	public static int ClampToByte(int b)
	{
		return System.Math.Min(System.Math.Max(b, byte.MinValue), byte.MaxValue);
	}

	///<summary>Sets the speed of animation.</summary>
	///<param name="s">Speed.</param>
	///<returns>Interval.</returns>
	public static float Interval(float s)
	{
		return s * (float)Variants.Imitator.Physics.ElapsedTime.TotalSeconds;
	}
}

public static class Explosion
{
	internal static int sngProgression = 10;
	public static bool Create(ref Variants.Imitator.Engine.Particle particle)
	{
		//if (particle.index == 0) sngProgression = (sngProgression + 10) % 100;
		float r = (particle.Index / 80F) * (float)System.Math.Exp(particle.Index / sngProgression % 3);

		particle.Position.X += (r * (float)System.Math.Cos((particle.Index)));
		particle.Position.Z += (r * (float)System.Math.Sin((particle.Index)));

		if (r < 10)
			particle.Color = System.Drawing.Color.FromArgb(byte.MaxValue, byte.MaxValue, (int)(.6 * byte.MaxValue), (int)(.3 * byte.MaxValue)).ToArgb();
		else
			particle.Color = System.Drawing.Color.Gray.ToArgb();

		particle.Scale = 7;
		return true;
	}

	public static int Update(ref Variants.Imitator.Engine.Particle particle)
	{
		System.Drawing.Color diffuse = System.Drawing.Color.FromArgb(particle.Color);
		particle.Color = System.Drawing.Color.FromArgb(System.Math.Max(0, (int)(diffuse.A - Common.Interval(200 + 10 * (float)Common.Randomizer.NextDouble()))), diffuse).ToArgb();
		return 0;
	}

	public static bool Delete(ref Variants.Imitator.Engine.Particle particle)
	{
		return System.Drawing.Color.FromArgb(particle.Color).A == 0;
	}
}

/* Примеры из версии для VB.NET
Module Common
	'''<summary>Clamps Integer to range 0-255.</summary>
	'''<param name="b">Integer.</param>
	'''<returns>Byte.</returns>
	Function ClampToByte(ByVal b As Integer) As Integer
		Return System.Math.Min(System.Math.Max(b, Byte.MinValue), Byte.MaxValue)
	End Function

	'''<summary>Sets the speed of animation.</summary>
	'''<param name="s">Speed.</param>
	'''<returns>Interval.</returns>
	Function Interval(ByVal s As Single) As Single
		Return s * Imitator.Maths.Timer.Interval
	End Function
End Module

Module Idle
	Function Create(ByRef particle As Imitator.Engine.Particle) As Boolean
		Return True
	End Function

	Function Update(ByRef particle As Imitator.Engine.Particle) As Integer
		Return 0
	End Function

	Function Delete(ByRef particle As Imitator.Engine.Particle) As Boolean
		Return True
	End Function

	Sub MAIN(ByVal act As Integer)

	End Sub
End Module

Module Explosion
	Const lifetime As Single = 3.0F

	Function Create(ByRef particle As Imitator.Engine.Particle) As Boolean
		particle.Position.X += Microsoft.VisualBasic.VBMath.Rnd * 40 - 20
		particle.Position.Y += Microsoft.VisualBasic.VBMath.Rnd * 40 - 20
		particle.Position.Z += Microsoft.VisualBasic.VBMath.Rnd * 40 - 20
		particle.Rotation = Microsoft.VisualBasic.VBMath.Rnd * 7
		particle.Scale = 10
		Return True
	End Function

	Function Update(ByRef particle As Imitator.Engine.Particle) As Integer
		Dim linear As Color = Color.FromArgb(particle.Color)
		Dim fader As Single = (lifetime - particle.Age) / lifetime
		Dim a As Integer = ClampToByte(Byte.MaxValue * fader)
		'particle.Position.X += System.Math.Sign(particle.Position.X) * Interval(10)
		'particle.Position.Y += System.Math.Sign(particle.Position.Y) * Interval(10)
		'particle.Position.Z += System.Math.Sign(particle.Position.Z) * Interval(10)

		'Dim r As Integer = ClampToByte(Byte.MaxValue - (linear.R + (Byte.MaxValue - linear.R) * fader))
		'Dim g As Integer = ClampToByte(Byte.MaxValue - (linear.G + (Byte.MaxValue - linear.G) * fader))
		'Dim b As Integer = ClampToByte(Byte.MaxValue - (linear.B + (Byte.MaxValue - linear.B) * fader))
		particle.Color = Color.FromArgb(a, linear).ToArgb
		Return 0
	End Function

	Function Delete(ByRef particle As Imitator.Engine.Particle) As Boolean
		Return particle.Age > lifetime
	End Function

	Sub MAIN(ByVal act As Integer)

	End Sub
End Module

'Module Thrust 'SmokeFX
'    Function Create(ByRef particle As Imitator.Engine.Particle) As Boolean
'        particle.Color = Color.orange.toargb ' Color.FromArgb((0.7 + (0.2 * Microsoft.VisualBasic.VBMath.rnd))*255, 0.25*255, 0.25*255, 0.25*255).toargb
'        particle.scale = 5
'        Return Microsoft.VisualBasic.VBMath.rnd < 0.2
'    End Function

'    Function Update(ByRef particle As Imitator.Engine.Particle) As Integer
'        'dim velocity as new Plasticine.Structures.Engine.Vector3D(Particle.age)
'        particle.Position.X += (Microsoft.VisualBasic.VBMath.Rnd * 20 - 10) * Plasticine.Maths.Timer.Interval '* Particle.age
'        particle.Position.z -= (Microsoft.VisualBasic.VBMath.Rnd * 100) * Plasticine.Maths.Timer.Interval '* Particle.age
'        Dim diffuse As color = color.fromargb(particle.Color)
'        If particle.age > 0.02F Then
'            diffuse = color.fromargb(diffuse.a, color.gray)
'        End If
'        particle.Color = Color.FromArgb(max(0, diffuse.a - (5 + Microsoft.VisualBasic.VBMath.Rnd * 5) * 255 * Plasticine.Maths.Timer.Interval), diffuse).toargb
'        Return 0
'    End Function

'    Function Delete(ByRef particle As Imitator.Engine.Particle) As Boolean
'        If color.fromargb(particle.Color).a = 0 Then
'            Return True
'        Else
'            Return False
'        End If
'    End Function

'    Sub MAIN(ByVal act As Integer)

'    End Sub
'End Module

'Module Explosion'Wormhole
'	Private sngProgression As Integer = 10
'	function create(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as boolean
'		'If Particle.index = 0 Then sngProgression = (sngProgression + 10) mod 100
'		dim r as single = (Particle.index / 80) * Exp(Particle.index / sngProgression Mod 3)
'		Particle.Position.X += (r * Cos((Particle.index)))
'		Particle.Position.Z += (r * Sin((Particle.index)))
'		if r < 10 then
'			Particle.Color = Color.FromArgb(255, 255, .6*255, .3*255).toargb
'		else
'			Particle.Color = color.gray.toargb
'		end if
'		particle.scale=7
'		return true
'	end function

'	function update(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as integer
'		dim diffuse as color = color.fromargb(Particle.Color)
'		Particle.Color = Color.FromArgb(max(0, diffuse.a - (200 + 10 * Microsoft.VisualBasic.VBMath.rnd) * Plasticine.Maths.Timer.Interval), diffuse).toargb
'		return 0
'	end function

'	function Delete(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as boolean
'		if color.fromargb(Particle.Color).a = 0 then
'			return true
'		else
'			return false
'		end if
'	end Function

'    Sub MAIN(ByVal act As Integer)

'    End Sub
'End Module

'Module BlueTwirl
'	Private sngProgression As Integer = 0
'	Private intDirection As Integer = 1
'	function create(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as boolean
'		Dim Q As Single
'		Dim a As Single, b As Single
'		a = 10 + sngProgression
'		b = 10 + sngProgression 'Q=Theta Angle 'HEART
'		'Only if it's the first item
'		sngProgression = sngProgression + intDirection
'		If sngProgression > 50 Then intDirection = -1
'		If sngProgression < -50 Then intDirection = 1
'		Q = (Particle.index / 100) + sngProgression
'		Particle.Position.X += b * Sin(Q * 2) / 10
'		Particle.Position.Z -= a * Cos(Q * 2) / 10
'		Particle.color = Color.FromArgb(255, .25*255, .25*255, 255).toargb
'		particle.scale=1
'		return true
'	end function

'	function update(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as integer
'		Particle.color = Color.FromArgb(0, Color.FromArgb(Particle.color)).toargb
'		return 0
'	end function

'	function Delete(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as boolean
'		if color.fromargb(Particle.color).a = 0 then
'			return true
'		else
'			return false
'		end if
'	end Function

'    Sub MAIN(ByVal act As Integer)

'    End Sub
'End Module

'Module BlueExplosion
'	Private sngProgression As Integer = 0
'	function create(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as boolean
'		If Particle.index = 0 Then sngProgression = 10'sngProgression + 10
'		dim r as single = Atan(Particle.index Mod 12)
'		Particle.Position.x += r * Cos(Particle.index / sngProgression) * 30
'		Particle.Position.z += r * Sin(Particle.index / sngProgression) * 30
'		'Particle.Velocity.x = cos(Particle.index)
'		'Particle.Velocity.y = sin(Particle.index)
'		Particle.color = Color.FromArgb(255, 0.3*255, .6*255, 255).toargb
'		particle.scale=5
'		return true
'	end function

'	function update(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as integer
'		'Particle.Velocity += Particle.Acceleration * Plasticine.Maths.Timer.Interval
'		Particle.Position += new Plasticine.Structures.Engine.Vector3D(cos(Particle.index),0,sin(Particle.index)) * Plasticine.Maths.Timer.Interval
'		dim diffuse as color = color.fromargb(Particle.Color)
'		Particle.color = Color.FromArgb(max(0, diffuse.a - 100 * Plasticine.Maths.Timer.Interval), diffuse).toargb
'		return 0
'	end function

'	function Delete(byref Particle as Plasticine.Structures.Engine.Scene.EmitterInterface.ParticleDefinition.EachParticle) as boolean
'		if color.fromargb(Particle.color).a = 0 then
'			return true
'		else
'			return false
'		end if
'	end Function

'    Sub MAIN(ByVal act As Integer)

'    End Sub
'End Module
*/