/*
=========================================================================================================
  Module      : Facebook Conversion Utility(FacebookConversionUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.App.Common.FacebookConversion;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.User;

namespace w2.App.Common.Util
{
	/// <summary>
	/// Facebook Conversion Utility
	/// </summary>
	public class FacebookConversionUtility
	{
		/// <summary>Facebook Sex value: Male</summary>
		private const string FLG_USER_FACEBOOK_SEX_MALE = "m";
		/// <summary>Facebook Sex value: Female</summary>
		private const string FLG_USER_FACEBOOK_SEX_FEMALE = "f";
		/// <summary>Facebook Action source: website</summary>
		private const string FACEBOOK_ACTION_SOURCE_DEFAULT = "website";
		/// <summary>Facebook Action source: chat</summary>
		private const string FACEBOOK_ACTION_SOURCE_BOTCHAN = "chat";
		/// <summary>Facebook contents type add to cart</summary>
		private const string FACEBOOK_CONTENTS_TYPE_ADD_TO_CART = "product";
		/// <summary>Japan international telephone code</summary>
		private const string JAPAN_INTERNATIONAL_TELEPHONE_CODE = "81";
		/// <summary>Japan country iso code</summary>
		private const string JAPAN_COUNTRY_ISO_CODE = "jp";

		/// <summary>
		/// Create hash sha256
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>result</returns>
		private string CreateHashSha256(string data)
		{
			var keyBytesForHash = Encoding.UTF8.GetBytes(data.ToLower().Trim());
			using (var sha256 = new SHA256CryptoServiceProvider())
			{
				var hashBytes = sha256.ComputeHash(keyBytesForHash);
				var result = string.Join(string.Empty, hashBytes.Select(item => item.ToString("x2")));
				return result;
			}
		}

		/// <summary>
		/// Create hash for birthday
		/// </summary>
		/// <param name="date">Birthday</param>
		/// <returns>Hash string for birth</returns>
		private string CreateHashForBirthDay(DateTime? date)
		{
			if (date.HasValue == false) return string.Empty;

			var birthdayHash = CreateHashSha256(date.Value.ToString(Constants.DATE_FORMAT_SHORT));
			return birthdayHash;
		}

		/// <summary>
		/// Create hash for string
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hash string</returns>
		private string CreateHashForString(string data)
		{
			var stringHash = CreateHashSha256(data.ToLower().Trim());
			return stringHash;
		}

		/// <summary>
		/// Create hash for gender
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hash string for gender</returns>
		private string CreateHashForGender(string data)
		{
			var gender = (data.ToUpper().Trim() == Constants.FLG_USER_SEX_MALE)
				? FLG_USER_FACEBOOK_SEX_MALE
				: FLG_USER_FACEBOOK_SEX_FEMALE;
			var genderHash = CreateHashSha256(gender.ToLower().Trim());
			return genderHash;
		}

		/// <summary>
		/// Create hash for zip code
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hash string for zip code</returns>
		private string CreateHashForZipCode(string data)
		{
			var zipCodeHash = CreateHashSha256(data.ToLower().Trim().Replace(".", string.Empty));
			return zipCodeHash;
		}

		/// <summary>
		/// Create hash for phone
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="countryIsoCode">Country iso code</param>
		/// <returns>Hash string for phone</returns>
		private string CreateHashForPhone(string data, string countryIsoCode)
		{
			string nationNumber;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				var code = GlobalConfigs.GetInstance().GlobalSettings.InternationalTelephoneCode
					.FirstOrDefault(item => (item.Iso == countryIsoCode));
				nationNumber = (code != null) ? code.Number : JAPAN_INTERNATIONAL_TELEPHONE_CODE;
			}
			else
			{
				// Default: Using Japan international telephone code
				nationNumber = JAPAN_INTERNATIONAL_TELEPHONE_CODE;
			}

			var tmpTelNo = data
				.Replace("-", string.Empty)
				.Replace("(", string.Empty)
				.Replace(")", string.Empty);
			if (tmpTelNo.StartsWith("0"))
			{
				nationNumber += string.Join(string.Empty, tmpTelNo.Skip(1));
			}
			else
			{
				nationNumber += tmpTelNo;
			}

			var phoneHash = CreateHashSha256(nationNumber);
			return phoneHash;
		}

		/// <summary>
		/// Create hash for country
		/// </summary>
		/// <param name="data">Data</param>
		/// <returns>Hash string for country</returns>
		private string CreateHashForCountry(string data)
		{
			string code;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				code = GlobalConfigs.GetInstance().GlobalSettings.CountryIsoCodes
					.FirstOrDefault(item => (item == data));

				code = (code != null) ? code.ToLower().Trim() : JAPAN_COUNTRY_ISO_CODE;
			}
			else
			{
				code = JAPAN_COUNTRY_ISO_CODE;
			}

			var countryHash = CreateHashSha256(code);
			return countryHash;
		}

		/// <summary>
		/// Create convert facebook request
		/// </summary>
		/// <param name="eventName">Event name</param>
		/// <param name="userId">User ID</param>
		/// <param name="eventSourceUrl">Event source url</param>
		/// <param name="eventId">Event id</param>
		/// <param name="isOrderBotChan">Is order bot chan</param>
		/// <param name="customData">Custom data</param>
		/// <returns>Convert Facebook Object</returns>
		public FacebookConversionRequest CreateConvertFaceBookRequest(
			string eventName,
			string userId,
			string eventSourceUrl,
			string eventId,
			bool isOrderBotChan,
			FacebookConversionDataRequest.CustomData customData)
		{
			var userData = CreateUserDataRequest(userId);
			var unixTime = int.Parse(((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds().ToString());

			var actionSource = isOrderBotChan
				? FACEBOOK_ACTION_SOURCE_BOTCHAN
				: FACEBOOK_ACTION_SOURCE_DEFAULT;
			var facebookConversionDataRequest = new FacebookConversionDataRequest
			{
				EventName = eventName,
				EventTime = unixTime,
				UserDataFacebook = userData,
				EventSourceUrl = eventSourceUrl,
				OptOut = true,
				EventId = eventId,
				ActionSource = actionSource,
				CustomDataFacebook = customData,
			};

			var request = new FacebookConversionRequest
			{
				Data = new FacebookConversionDataRequest[] { facebookConversionDataRequest },
				TestEventCode = Constants.MARKETING_FACEBOOK_CAPI_TEST_EVENT_CODE,
			};
			return request;
		}

		/// <summary>
		/// Create user data request
		/// </summary>
		/// <param name="userId">User id</param>
		/// <returns>User data</returns>
		private FacebookConversionDataRequest.UserData CreateUserDataRequest(string userId)
		{
			var clientUserAgent = StringUtility.ToEmpty(HttpContext.Current.Request.Headers["User-Agent"]);
			var user = new UserService().Get(userId);
			if (user == null)
			{
				var guestRequest = new FacebookConversionDataRequest.UserData
				{
					ClientIpAddress = HttpContext.Current.Request.UserHostAddress,
					ClientUserAgent = clientUserAgent,
				};
				return guestRequest;
			}

			var request = new FacebookConversionDataRequest.UserData
			{
				MailAddress = CreateHashSha256(user.MailAddr),
				Phone = CreateHashForPhone(user.Tel1, user.AddrCountryIsoCode),
				Gender = CreateHashForGender(user.Sex),
				BirthDay = CreateHashForBirthDay(user.Birth),
				LastName = CreateHashForString(user.Name1),
				FirstName = CreateHashForString(user.Name2),
				City = CreateHashForString(user.Addr2),
				State = CreateHashForString(user.Addr1),
				Zip = CreateHashForZipCode(user.Zip),
				Country = CreateHashForCountry(user.AddrCountryIsoCode),
				ClientIpAddress = user.RemoteAddr,
				ClientUserAgent = clientUserAgent,
			};
			return request;
		}

		/// <summary>
		/// Create custom data add to cart
		/// </summary>
		/// <param name="cartList">Cart list</param>
		/// <returns>Custom data</returns>
		public FacebookConversionDataRequest.CustomData CreateCustomDataAddToCart(CartObjectList cartList)
		{
			if (cartList == null)
			{
				FileLogger.WriteError(
					string.Format(
						"{0}\r\n{1}",
						Constants.FACEBOOK_CONVERSION_API,
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_CARTLIST_NOT_EXISTS)));
				return null;
			}

			var contentIds = string.Empty;
			var contentCategory = string.Empty;
			var contentsName = string.Empty;
			var contents = new List<FacebookConversionDataRequest.Contents>();
			foreach (var cartObject in cartList.Items)
			{
				foreach (var cartProduct in cartObject.Items)
				{
					var contentsObject = new FacebookConversionDataRequest.Contents
					{
						ProductId = cartProduct.ProductId,
						ItemPrice = cartProduct.Price,
						Quantity = cartProduct.Count.ToString(),
					};
					contents.Add(contentsObject);

					contentIds = GetUpdatedContentIds(cartProduct, contentIds);
					contentsName = GetUpdatedContentNames(cartProduct, contentsName);
					contentCategory = GetUpdatedContentCategories(cartProduct, contentsName);
				}
			}

			var currency = Constants.GLOBAL_OPTION_ENABLE
				? RegionManager.GetInstance().Region.CurrencyCode
				: Constants.CONST_KEY_CURRENCY_CODE;
			var request = new FacebookConversionDataRequest.CustomData
			{
				Currency = currency,
				Value = cartList.PriceCartListTotal,
				ContentName = contentsName,
				ContentIds = contentIds,
				ContentCategory = contentCategory,
				Contents = contents,
				ContentType = FACEBOOK_CONTENTS_TYPE_ADD_TO_CART,
			};
			return request;
		}

		/// <summary>
		/// Create custom data purchase
		/// </summary>
		/// <param name="orderId">order ID</param>
		/// <returns>Custom data</returns>
		public FacebookConversionDataRequest.CustomData CreateCustomDataPurchase(string orderId)
		{
			var order = new OrderService().Get(orderId);
			if (order == null)
			{
				FileLogger.WriteError(
					string.Format(
						"{0}\r\n{1}",
						Constants.FACEBOOK_CONVERSION_API,
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_ORDER_NOT_EXISTS).Replace("@@ 1 @@", orderId)));
				return null;
			}

			var request = new FacebookConversionDataRequest.CustomData
			{
				Currency = order.SettlementCurrency,
				Value = order.OrderPriceTotal,
				OrderId = orderId,
			};
			return request;
		}

		/// <summary>
		/// Create custom data view contents
		/// </summary>
		/// <param name="contentName">Content name</param>
		/// <param name="value">Value</param>
		/// <param name="currency">Currency</param>
		/// <param name="contentType">Content type</param>
		/// <param name="contentIds">Content ids</param>
		/// <param name="contentCategory">Content category</param>
		/// <returns>Custom data</returns>
		public FacebookConversionDataRequest.CustomData CreateCustomDataViewContents(
			string contentName,
			decimal value,
			string currency,
			string contentType,
			string contentIds,
			string contentCategory)
		{
			var request = new FacebookConversionDataRequest.CustomData
			{
				Currency = currency,
				Value = value,
				ContentType = contentType,
				ContentIds = contentIds,
				ContentCategory = contentCategory,
				ContentName = contentName,
			};
			return request;
		}

		/// <summary>
		/// Create custom data complete registration
		/// </summary>
		/// <param name="contentName">Content name</param>
		/// <param name="value">Value</param>
		/// <param name="currency">Currency</param>
		/// <param name="status">Status</param>
		/// <returns>Custom data</returns>
		public FacebookConversionDataRequest.CustomData CreateCustomDataCompleteRegistration(
			string contentName,
			decimal value,
			string currency,
			string status)
		{
			var request = new FacebookConversionDataRequest.CustomData
			{
				ContentName = contentName,
				Value = value,
				Status = status,
				Currency = currency,
			};
			return request;
		}

		/// <summary>
		/// Get updated content IDs
		/// </summary>
		/// <param name="cartProduct">Cart product</param>
		/// <param name="contentCategories">Content IDs</param>
		/// <returns>Updated Content IDs</returns>
		private string GetUpdatedContentIds(CartProduct cartProduct, string contentIds)
		{
			var result = string.IsNullOrEmpty(contentIds)
				? cartProduct.ProductId
				: string.Format(
					"{0},{1}",
					contentIds,
					cartProduct.ProductId);
			return result;
		}

		/// <summary>
		/// Get updated content names
		/// </summary>
		/// <param name="cartProduct">Cart product</param>
		/// <param name="contentCategories">Content names</param>
		/// <returns>Updated content names</returns>
		private string GetUpdatedContentNames(CartProduct cartProduct, string contentNames)
		{
			var result = (string.IsNullOrEmpty(contentNames))
				? cartProduct.ProductJointName
				: string.Format(
					"{0},{1}",
					contentNames,
					cartProduct.ProductJointName);
			return result;
		}

		/// <summary>
		/// Get updated content categories
		/// </summary>
		/// <param name="cartProduct">Cart product</param>
		/// <param name="contentCategories">Content categories</param>
		/// <returns>Updated content categories</returns>
		private string GetUpdatedContentCategories(CartProduct cartProduct, string contentCategories)
		{
			var result = (string.IsNullOrEmpty(contentCategories))
				? cartProduct.CategoryId1
				: string.Format(
					"{0},{1}",
					contentCategories,
					cartProduct.CategoryId1);
			return result;
		}
	}
}
