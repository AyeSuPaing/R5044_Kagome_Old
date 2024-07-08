/*
=========================================================================================================
  Module      : IdPassデシリアライザ (IdPassDeserializer.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using w2.Common.Extensions;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// IdPassデシリアライザ
	/// </summary>
	internal static class IdPassDeserializer
	{
		/// <summary>
		/// デシリアライズ
		/// </summary>
		/// <typeparam name="T">デシリアライズする型</typeparam>
		/// <param name="value">文字列</param>
		/// <returns>デシリアライズしたオブジェクト</returns>
		public static T Deserialize<T>(string value)
		{
			var constructor = typeof(T).GetConstructor(Type.EmptyTypes);
			if (constructor == null)
			{
				throw new NotSupportedException("Could not construct an instance of " + typeof(T).Name);
			}

			var dic = value
				.Split('&')
				.Select(s => s.Split('='))
				.Where(kv => ((kv.Length == 2) && kv.All(s => (string.IsNullOrEmpty(s) == false))))
				.ToDictionary(s => s[0], s => s[1]);

			var instance = (T)constructor.Invoke(null);
			foreach (var prop in EnumerateAvailableProperties(instance))
			{
				if (dic.ContainsKey(prop.Key) == false) continue;

				prop.Property.SetValue(
					instance,
					(prop.Type == typeof(string[]))
						? (object)dic[prop.Key].Split('|')
						: dic[prop.Key]);
			}

			return instance;
		}

		/// <summary>
		/// シリアライズ
		/// </summary>
		/// <param name="obj">オブジェクト</param>
		/// <returns>IdPass形式文字列</returns>
		public static string Serialize(object obj)
		{
			var result = EnumerateAvailableProperties(obj)
				.Where(
					prop =>
					{
						if (prop.Type == typeof(string)) return (string.IsNullOrEmpty((string)prop.Value) == false);
						if (prop.Type == typeof(string[])) return ((IEnumerable<string>)prop.Value).Any();

						throw new NotSupportedException();
					})
				.Select(
					prop =>
					{
						if (prop.Type == typeof(string))
						{
							var value = (string)prop.Value;
							return string.Format("{0}={1}", prop.Key, value);
						}

						if (prop.Type == typeof(string[]))
						{
							var values = (IEnumerable<string>)prop.Value;
							return string.Format("{0}={1}", prop.Key, values.JoinToString("|"));
						}

						throw new NotSupportedException();
					})
				.JoinToString("&");

			return result;
		}

		/// <summary>
		/// IdPassPropertyが利用できるプロパティ情報を列挙
		/// </summary>
		/// <param name="obj">オブジェクト</param>
		/// <returns>プロパティ情報</returns>
		private static IEnumerable<IdPassPropertyInformations> EnumerateAvailableProperties(object obj)
		{
			var availableTypes = new[] { typeof(string), typeof(string[]) };
			foreach (var prop in obj.GetType().GetProperties())
			{
				if ((availableTypes.Contains(prop.PropertyType) == false)
					|| (prop.CanRead == false)
					|| (prop.GetCustomAttributes(inherit: true).OfType<IdPassIgnoreAttribute>().Any())) continue;

				var attr = prop.GetCustomAttributes().OfType<IdPassPropertyAttribute>().FirstOrDefault();
				if (attr == null) continue;

				yield return new IdPassPropertyInformations
				{
					Key = attr.Key,
					Value = prop.GetValue(obj),
					Property = prop,
				};
			}
		}

		/// <summary>
		/// IdPassプロパティ情報
		/// </summary>
		private class IdPassPropertyInformations
		{
			/// <summary>型情報</summary>
			public Type Type
			{
				get { return this.Property.PropertyType; }
			}
			/// <summary>プロパティ情報</summary>
			public PropertyInfo Property { get; set; }
			/// <summary>内部値</summary>
			public object Value { get; set; }
			/// <summary>IdPassキー</summary>
			public string Key { get; set; }
		}
	}
}
