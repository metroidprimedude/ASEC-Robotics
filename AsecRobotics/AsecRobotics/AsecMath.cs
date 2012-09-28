using System;
using Microsoft.SPOT;

namespace Asec.Robotics
{
	public static class AsecMath : Math
	{
		const float sq2p1 = 2.414213562373095048802e0F;
		const float sq2m1 = .414213562373095048802e0F;
		const float pio2 = 1.570796326794896619231e0F;
		const float pio4 = .785398163397448309615e0F;
		const float atan_p4 = .161536412982230228262e2F;
		const float atan_p3 = .26842548195503973794141e3F;
		const float atan_p2 = .11530293515404850115428136e4F;
		const float atan_p1 = .178040631643319697105464587e4F;
		const float atan_p0 = .89678597403663861959987488e3F;
		const float atan_q4 = .5895697050844462222791e2F;
		const float atan_q3 = .536265374031215315104235e3F;
		const float atan_q2 = .16667838148816337184521798e4F;
		const float atan_q1 = .207933497444540981287275926e4F;
		const float atan_q0 = .89678597403663861962481162e3F;

		/// <summary>
		/// Returns the angle whose tangent is the specified number
		/// </summary>
		/// <param name="x">A number representing a tangent.</param>
		/// <returns>the arctangent of x</returns>
		public static float Atan(float x)
		{
			if (x > 0.0F)
				return (atans(x));
			else
				return (-atans(-x));
		}

		/// <summary>
		/// Returns the angle whose tangent is the quotient of two specified numbers.
		/// </summary>
		/// <param name="y">The y coordinate of a point.</param>
		/// <param name="x">The x coordinate of a point.</param>
		/// <returns>the arctangent of x and y</returns>
		public static float Atan2(float y, float x)
		{

			if ((x + y) == x)
			{
				if ((x == 0F) & (y == 0F)) return 0F;

				if (x >= 0.0F)
					return pio2;
				else
					return (-pio2);
			}
			else if (y < 0.0F)
			{
				if (x >= 0.0F)
					return ((pio2 * 2) - atans((-x) / y));
				else
					return (((-pio2) * 2) + atans(x / y));
			}
			else if (x > 0.0F)
			{
				return (atans(x / y));
			}
			else
			{
				return (-atans((-x) / y));
			}
		}
		private static float atans(float x)
		{
			if (x < sq2m1)
				return (atanx(x));
			else if (x > sq2p1)
				return (pio2 - atanx(1.0F / x));
			else
				return (pio4 + atanx((x - 1.0F) / (x + 1.0F)));
		}
		private static float atanx(float x)
		{
			float argsq;
			float value;

			argsq = x * x;
			value = ((((atan_p4 * argsq + atan_p3) * argsq + atan_p2) * argsq + atan_p1) * argsq + atan_p0);
			value = value / (((((argsq + atan_q4) * argsq + atan_q3) * argsq + atan_q2) * argsq + atan_q1) * argsq + atan_q0);
			return (value * x);
		}
	}
}
