using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	public class AccessCache
	{
		Dictionary<Type, Dictionary<string, FieldInfo>> fields = new Dictionary<Type, Dictionary<string, FieldInfo>>();
		Dictionary<Type, Dictionary<string, PropertyInfo>> properties = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
		readonly Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>> methods = new Dictionary<Type, Dictionary<string, Dictionary<int, MethodBase>>>();

		public FieldInfo GetFieldInfo(Type type, string name)
		{
			if (!fields.TryGetValue(type, out Dictionary<string, FieldInfo> fieldsByType))
			{
				fieldsByType = new Dictionary<string, FieldInfo>();
				fields.Add(type, fieldsByType);
			}

			if (!fieldsByType.TryGetValue(name, out FieldInfo field))
			{
				field = AccessTools.Field(type, name);
				fieldsByType.Add(name, field);
			}
			return field;
		}

		public PropertyInfo GetPropertyInfo(Type type, string name)
		{
			if (!properties.TryGetValue(type, out Dictionary<string, PropertyInfo> propertiesByType))
			{
				propertiesByType = new Dictionary<string, PropertyInfo>();
				properties.Add(type, propertiesByType);
			}

			if (!propertiesByType.TryGetValue(name, out PropertyInfo property))
			{
				property = AccessTools.Property(type, name);
				propertiesByType.Add(name, property);
			}
			return property;
		}

		static int CombinedHashCode(IEnumerable<object> objects)
		{
			int hash1 = (5381 << 16) + 5381;
			int hash2 = hash1;
			int i = 0;
			foreach (var obj in objects)
			{
				if (i % 2 == 0)
					hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ obj.GetHashCode();
				else
					hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ obj.GetHashCode();
				++i;
			}
			return hash1 + (hash2 * 1566083941);
		}

		public MethodBase GetMethodInfo(Type type, string name, Type[] arguments)
		{
			if (!methods.TryGetValue(type, out Dictionary<string, Dictionary<int, MethodBase>> methodsByName))
			{
				methodsByName = new Dictionary<string, Dictionary<int, MethodBase>>();
				methods.Add(type, methodsByName);
			}

			if (!methodsByName.TryGetValue(name, out Dictionary<int, MethodBase> methodsByArguments))
			{
				methodsByArguments = new Dictionary<int, MethodBase>();
				methodsByName.Add(name, methodsByArguments);
			}

			var argumentsHash = CombinedHashCode(arguments);

			if (!methodsByArguments.TryGetValue(argumentsHash, out MethodBase method))
			{
				method = AccessTools.Method(type, name, arguments);
				methodsByArguments.Add(argumentsHash, method);
			}

			return method;
		}
	}
}