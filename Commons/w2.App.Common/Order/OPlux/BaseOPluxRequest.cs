/*
=========================================================================================================
  Module      : Base O-Plux Request(BaseOPluxRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.App.Common.Order.OPlux.Requests.Helper;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// Base O-PLUX request
	/// </summary>
	public abstract class BaseOPluxRequest : IHttpApiRequestData
	{
		/// <summary>Request</summary>
		protected IDictionary<string, string> _request = new Dictionary<string, string>();
		/// <summary>Currency code JPY</summary>
		private const string CURRENCY_CODE_JPY = "JPY";

		/// <summary>
		/// Create post string
		/// </summary>
		/// <returns>Post string</returns>
		public string CreatePostString()
		{
			SetParameterByClassFloorToRequest();
			var request = _request
				.Select(item => string.Concat(
					item.Key,
					"=",
					HttpUtility.UrlEncode(item.Value)))
				.ToArray();
			var postString = string.Join("&", request);
			return postString;
		}

		/// <summary>
		/// Set parameter by class floor to request
		/// </summary>
		public void SetParameterByClassFloorToRequest()
		{
			if (_request.Count > 0) return;

			var properties = this.GetType().GetProperties();
			var requestName = GetRequestName(this.GetType());

			foreach (var property in properties)
			{
				RecursiveSetParameterByClassFloor(property, this, requestName);
			}
		}

		/// <summary>
		/// Recursive set parameter by class floor
		/// </summary>
		/// <param name="property">Property</param>
		/// <param name="parent">Parent</param>
		/// <param name="parentName">Parent name</param>
		public void RecursiveSetParameterByClassFloor(
			PropertyInfo propertyInfo,
			object parent,
			string parentName)
		{
			var values = propertyInfo.GetValue(parent, null);

			if (values == null) return;

			var requestName = GetRequestName(propertyInfo);
			var requestFullName = string.Format(
				"{0}.{1}",
				parentName,
				requestName);

			// Check property is object
			if ((typeof(ValueType).IsAssignableFrom(propertyInfo.PropertyType) == false)
				&& (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType) == false))
			{
				var childProperties = values.GetType().GetProperties();

				foreach (var childProperty in childProperties)
				{
					RecursiveSetParameterByClassFloor(childProperty, values, requestFullName);
				}
			}
			else
			{
				// Check if a value has multiple values
				if (values.GetType().IsGenericType
					|| values.GetType().IsArray)
				{
					var index = 0;

					foreach (var value in (IEnumerable<object>)values)
					{
						var requestNameItem = GetRequestName(value.GetType());
						var properties = value.GetType().GetProperties();
						var items = properties.ToDictionary(
							property => string.Format(
								"{0}.{1}{2}.{3}",
								requestFullName,
								requestNameItem,
								index,
								GetRequestName(property)),
							property => ConvertValue(property.GetValue(value, null), property));

						foreach (var item in items)
						{
							_request[item.Key] = item.Value;
						}

						index++;
					}
				}
				else
				{
					_request[requestFullName] = ConvertValue(values, propertyInfo);
				}
			}
		}

		/// <summary>
		/// Get request name
		/// </summary>
		/// <param name="memberInfo">Member information</param>
		/// <returns>Request name</returns>
		private string GetRequestName(MemberInfo memberInfo)
		{
			var requestName = Attribute.IsDefined(memberInfo, typeof(RequestKey))
				? ((RequestKey)Attribute.GetCustomAttribute(memberInfo, typeof(RequestKey))).RequestName
				: memberInfo.Name.ToLower();
			return requestName;
		}

		/// <summary>
		/// Convert value
		/// </summary>
		/// <param name="value">Value</param>
		/// <param name="memberInfo">Member info</param>
		/// <returns>Value</returns>
		private static string ConvertValue(object value, MemberInfo memberInfo = null)
		{
			var formatValue = ((memberInfo != null)
					&& Attribute.IsDefined(memberInfo, typeof(RequestKey)))
				? ((RequestKey)Attribute.GetCustomAttribute(memberInfo, typeof(RequestKey))).FormatValue
				: string.Empty;

			if (value is DateTime)
			{
				formatValue = string.IsNullOrEmpty(formatValue)
					? "yyyy/MM/dd HH:mm:ss"
					: formatValue;
				return StringUtility.ToDateString(value, formatValue);
			}

			if ((value is decimal)
				&& (Constants.CONST_KEY_CURRENCY_CODE == CURRENCY_CODE_JPY))
			{
				var amount = DecimalUtility.DecimalRound((decimal)value, DecimalUtility.Format.RoundDown);
				return StringUtility.ToEmpty(amount);
			}

			return StringUtility.ToEmpty(value);
		}

		/// <summary>
		/// Create hash SHA256
		/// </summary>
		/// <param name="targetInput">Target input</param>
		/// <returns>Hash SHA256</returns>
		public static string CreateHashSha256(string targetInput)
		{
			var keyBytesForHash = Encoding.UTF8.GetBytes(targetInput);
			using (var cryptoService = new SHA256CryptoServiceProvider())
			{
				var computeHash = cryptoService.ComputeHash(keyBytesForHash);
				var result = string.Join(string.Empty, computeHash.Select(item => item.ToString("X2")));
				return result;
			}
		}

		/// <summary>
		/// Create hash SHA1
		/// </summary>
		/// <param name="targetInput">Target input</param>
		/// <returns>Hash SHA1</returns>
		public static string CreateHashSha1(string targetInput)
		{
			var keyBytesForHash = Encoding.UTF8.GetBytes(targetInput);
			using (var cryptoService = new SHA1CryptoServiceProvider())
			{
				var computeHash = cryptoService.ComputeHash(keyBytesForHash);
				var result = string.Join(string.Empty, computeHash.Select(item => item.ToString("X2")));
				return result;
			}
		}

		/// <summary>
		/// Create hash for signature
		/// </summary>
		/// <param name="requestDatetime">Request datetime</param>
		/// <returns>Signature hash</returns>
		public static string CreateHashForSignature(DateTime requestDatetime)
		{
			var signature = string.Format(
				"{0}{1}{2}{3}",
				Constants.OPLUX_CONNECTION_ID,
				Constants.OPLUX_REQUEST_SHOP_ID,
				ConvertValue(requestDatetime),
				Constants.OPLUX_SECRET_ACCESS_KEY);

			var signatureHash = CreateHashSha256(signature);
			return signatureHash;
		}
	}
}
