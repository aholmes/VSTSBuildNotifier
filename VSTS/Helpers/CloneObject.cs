using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VSTS
{
	public static class Helpers
	{
		public static void CloneObject<T>(this T source, T destination)
		{
			var TType = typeof(T);

			var propInfos = TType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var propInfo in propInfos)
			{
				var setter = propInfo.GetSetMethod(true);
				if (setter != null)
				{
					setter.Invoke(destination, new[] { propInfo.GetValue(source) });
				}
			}
		}
	}
}
