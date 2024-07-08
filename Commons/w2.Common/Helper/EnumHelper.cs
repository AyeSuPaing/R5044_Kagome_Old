/*
=========================================================================================================
  Module      : 列挙体ヘルパークラス (EnumHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Common.Helper.Attribute;

namespace w2.Common.Helper
{
	public static class EnumHelper
	{
		/// <summary>
		/// 属性で指定されたテキスト名を取得
		/// </summary>
		/// <param name="value">列挙型</param>
		/// <returns>テキスト名</returns>
		public static string ToText(this Enum value)
		{
			var enumType = value.GetType();
			var name = Enum.GetName(enumType, value);
			EnumTextName[] attributes = (EnumTextName[])enumType.GetField(name).GetCustomAttributes(typeof(EnumTextName), false);

			return attributes.FirstOrDefault().TextName;
		}

		/// <summary>
		/// EnumのEnumTextName属性文字から列挙型取得
		/// ・EnumTextNameが設定されていない列挙子の挙動
		/// もしもEnumTextNameが設定されていない列挙子があった場合、そこのEnumTextNameは空文字と見なされる
		/// </summary>
		/// <remarks>
		/// 性能がいまいちかもしれない...
		/// これを使う場合、列挙型クラスを検討してみてもいい
		/// https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/enumeration-classes-over-enum-types
		/// </remarks>
		/// <param name="value">Attributeテキスト</param>
		/// <param name="result">結果</param>
		/// <returns>成功失敗</returns>
		public static bool TryParseToEnum<T>(string value, out T result) where T : Enum
		{
			Func<T, string> parseString = (enumAttribute => enumAttribute.ToText());
			var success = EnumHelper.TryParseAttributeToEnum<T>(value, parseString, out result);
			return success;
		}

		/// <summary>
		/// Enumの属性文字から列挙型取得
		/// </summary>
		/// <typeparam name="TEnum">Enumの型</typeparam>
		/// <param name="value">値</param>
		/// <param name="parseStringFunc">文字列変換処理</param>
		/// <param name="result">変換結果</param>
		/// <returns>成功失敗</returns>
		private static bool TryParseAttributeToEnum<TEnum>(
			string value,
			Func<TEnum, string> parseStringFunc,
			out TEnum result)
			where TEnum : Enum
		{
			foreach (TEnum enumItem in typeof(TEnum).GetEnumValues())
			{
				var enumFieldText = parseStringFunc(enumItem);
				if (enumFieldText == value)
				{
					result = enumItem;
					return true;
				}
			}

			result = default;
			return false;
		}
	}
}
