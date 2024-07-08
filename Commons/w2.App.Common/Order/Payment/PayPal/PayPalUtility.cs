/*
=========================================================================================================
  Module      : PayPalUtility(PayPalUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using Braintree;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Global.Config;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Payment.PayPal
{
	/// <summary>
	/// PayPalUtility
	/// </summary>
	public class PayPalUtility
	{
		/// <summary>
		/// アカウント処理
		/// </summary>
		public class Account
		{
			/// <summary>ロックオブジェクト</summary>
			private static readonly object m_lock = new object();
			/// <summary>クライアントトークン（有効期限、トークン）</summary>
			private static KeyValuePair<DateTime, string> m_clientTokenInfo =
				new KeyValuePair<DateTime, string>(DateTime.MinValue, "");

			/// <summary>
			/// クライアントトークン作成
			/// </summary>
			/// <returns>クライアントトークン</returns>
			public static string CreateClientToken()
			{
				lock (m_lock)
				{
					try
					{
						var token = CreateClientTokenInner();
						return token;
					}
					catch (Exception ex)
					{
						FileLogger.WriteError("PayPal ClientToken取得失敗", ex);
						return "";
					}
				}
			}

			/// <summary>
			/// クライアントトークンをキャッシュから取得（キャッシングを行う）
			/// </summary>
			/// <returns>クライアントトークン情報（有効期限、トークン）</returns>
			private static string CreateClientTokenInner()
			{
				if (m_clientTokenInfo.Key < DateTime.Now)
				{
					var expire = DateTime.Now.AddHours(23);	// 24時間は有効なので23時間で期限を切る
					var token = Constants.PAYMENT_PAYPAL_GATEWAY_FOR_GET_CLIENTTOKEN.ClientToken.Generate();
					m_clientTokenInfo = new KeyValuePair<DateTime, string>(expire, token);
				}
				return m_clientTokenInfo.Value;
			}

			/// <summary>
			/// CustomerIdからPayPal Customer情報を取得
			/// </summary>
			/// <param name="paypalCustomerId">PayPalCustomerId</param>
			/// <returns>ユーザー</returns>
			public static Customer GetPayPalCustomerById(string paypalCustomerId)
			{
				var token = Constants.PAYMENT_PAYPAL_GATEWAY.Customer.Find(paypalCustomerId);
				return token;
			}

			/// <summary>
			/// PayPalCustomerIdからユーザー情報を取得
			/// </summary>
			/// <param name="paypalCustomerId">PayPalCustomerId</param>
			/// <returns>ユーザー</returns>
			public static UserModel GetUserByPayPalCustomerId(string paypalCustomerId)
			{
				var user = new UserService().GetByExtendColumn(
					Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID,
					paypalCustomerId);
				return user;
			}

			/// <summary>
			/// ユーザーIDから紐づいているPayPalアカウントメールを取得
			/// </summary>
			/// <param name="userId">ユーザーID</param>
			/// <returns>紐づいているPayPalアカウントメール</returns>
			public static string GetCooperateAccountEmail(string userId)
			{
				if (string.IsNullOrEmpty(userId)) return null;
				if (string.IsNullOrEmpty(Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID)) return null;
				if (string.IsNullOrEmpty(Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS)) return null;

				var userExtend = new UserService().GetUserExtend(userId);
				if (userExtend == null) return null;

				var payPalCooperationInfo = new PayPalCooperationInfo(userExtend);
				return payPalCooperationInfo.AccountEMail;
			}

			/// <summary>
			/// PayPal情報をユーザー拡張項目に更新
			/// </summary>
			/// <param name="userId">ユーザーID</param>
			/// <param name="paypalLoginResult">ペイパルログイン情報（nullの場合は値をクリア）</param>
			/// <param name="updateHistoryAction">更新履歴アクション</param>
			/// <returns>ユーザー</returns>
			public static void UpdateUserExtendForPayPal(
				string userId,
				PayPalLoginResult paypalLoginResult,
				UpdateHistoryAction updateHistoryAction)
			{
				if (string.IsNullOrEmpty(Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID)) return;
				if (string.IsNullOrEmpty(Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS)) return;

				new UserService().ModifyUserExtend(
					userId,
					model =>
					{
						model.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_CUSTOMER_ID] =
							(paypalLoginResult != null) ? paypalLoginResult.CustomerId : "";
						model.UserExtendDataValue[Constants.PAYPAL_USEREXTEND_COLUMNNAME_COOPERATION_INFOS] =
							(paypalLoginResult != null)
								? new PayPalCooperationInfo(paypalLoginResult).CooperationInfo
								: "";
						model.LastChanged = Constants.FLG_LASTCHANGED_USER;
					},
					updateHistoryAction);
			}
		}

		/// <summary>
		/// 決済処理
		/// </summary>
		public class Payment
		{
			/// <summary>ユーザークレジットカードカード表示名（PayPalカスタマー）</summary>
			public const string USERCREDITCARD_CARDDISPNAME_PAYPALCUSTOMER = "PalPalCustomer";

			/// <summary>
			/// 決済実行（Valut利用）
			/// </summary>
			/// <param name="paymentOrderId">決済注文ID</param>
			/// <param name="customerId">PayPal CustomerId</param>
			/// <param name="deviceData">PayPal deviceData</param>
			/// <param name="amountTmp">与信金額（小数点含む）</param>
			/// <param name="submitForSettlement">売上確定するか？</param>
			/// <param name="addressRequestWrapper">アドレスリクエストラッパー</param>
			/// <returns>クライアントトークン</returns>
			public static Result<Transaction> PayWithCustomerId(
				string paymentOrderId,
				string customerId,
				string deviceData,
				decimal amountTmp,
				bool submitForSettlement,
				AddressRequestWrapper addressRequestWrapper)
			{
				var amount = decimal.Parse(amountTmp.ToPriceString());	// 不要な小数点を削除
				var currencyCode = Constants.GLOBAL_OPTION_ENABLE
					? GlobalConfigs.GetInstance().GlobalSettings.KeyCurrency.Code
					: Constants.CONST_KEY_CURRENCY_CODE;
				var logInfo = string.Format(
					"paymentOrderId:{0} customerId:{1} amount:{2}, currencyCode:{3}",
					paymentOrderId,
					customerId,
					amount,
					currencyCode);
				try
				{
					var result = Constants.PAYMENT_PAYPAL_GATEWAY.Transaction.Sale(
						new TransactionRequest
						{
							OrderId = paymentOrderId,
							CustomerId = customerId,
							DeviceData = deviceData,
							Amount = amount,	// 不要な小数点を削除
							MerchantAccountId = currencyCode,
							ShippingAddress = addressRequestWrapper.AddressRequest,
							Options = new TransactionOptionsRequest
							{
								SubmitForSettlement = submitForSettlement,
								StoreInVaultOnSuccess = true, // customerIdが含まれていない場合、新しい顧客が作成される
							},
							Channel = Constants.PAYPAL_PAYMENT_BNCODE,
						});
					WritePayPalLog(result, PaymentFileLogger.PaymentProcessingType.Sale, logInfo, result.Message);
					return result;
				}
				catch (Exception ex)
				{
					WritePayPalLog(
						null,
						PaymentFileLogger.PaymentProcessingType.Sale,
						logInfo + string.Format(
							" deviceData:{0} shippingAddress{1}",
							deviceData,
							addressRequestWrapper.AddressRequest.ToXml()),
						ex.Message);
					throw;
				}
			}

			/// <summary>
			/// 売上確定
			/// </summary>
			/// <param name="transactionId">トランザクションID</param>
			/// <param name="amount">売り上げ確定金額</param>
			/// <returns>結果</returns>
			public static Result<Transaction> Sales(string transactionId, decimal amount)
			{
				var result = Constants.PAYMENT_PAYPAL_GATEWAY.Transaction.SubmitForSettlement(
					transactionId,
					new TransactionRequest
					{
						Amount = amount,
					});
				WritePayPalLog(
					result,
					PaymentFileLogger.PaymentProcessingType.SubmitForSettlement,
					"transactionId:" + transactionId,
					result.Message);
				return result;
			}

			/// <summary>
			/// 取り消しor返金処理
			/// </summary>
			/// <param name="transactionId">トランザクションID</param>
			/// <param name="doRefund">返金実行するか</param>
			/// <returns>結果</returns>
			public static Result<Transaction> VoidOrRefund(string transactionId, bool doRefund)
			{
				if (doRefund)
				{
					var resultRefund = Constants.PAYMENT_PAYPAL_GATEWAY.Transaction.Refund(transactionId);
					WritePayPalLog(
						resultRefund,
						PaymentFileLogger.PaymentProcessingType.Refund,
						"transactionId:" + transactionId,
						resultRefund.Message);
					return resultRefund;
				}
				else
				{
					var resultVoid = Constants.PAYMENT_PAYPAL_GATEWAY.Transaction.Void(transactionId);
					WritePayPalLog(
						resultVoid,
						PaymentFileLogger.PaymentProcessingType.Void,
						"transactionId:" + transactionId,
						resultVoid.Message);
					return resultVoid;
				}
			}

			/// <summary>
			/// ユーザークレジットカードとしてPayPal情報登録
			/// </summary>
			/// <param name="userId">ユーザーID</param>
			/// <param name="payPalCooperationInfo">ペイパル連携情報</param>
			/// <param name="lastChanged">最終更新者</param>
			/// <param name="updateHistoryAction">更新履歴アクション</param>
			/// <param name="accessor">SQLアクセサ</param>
			/// <returns>ユーザークレジットカード</returns>
			public static UserCreditCardModel RegisterAsUserCreditCard(
				string userId,
				PayPalCooperationInfo payPalCooperationInfo,
				string lastChanged,
				UpdateHistoryAction updateHistoryAction,
				SqlAccessor accessor = null)
			{
				var userCreditCard = new UserCreditCardModel
				{
					UserId = userId,
					//BranchNo	// 採番
					CooperationId = payPalCooperationInfo.CustomerId,
					CardDispName = USERCREDITCARD_CARDDISPNAME_PAYPALCUSTOMER,
					LastFourDigit = "",
					ExpirationMonth = "",
					ExpirationYear = "",
					AuthorName = payPalCooperationInfo.AccountEMail,
					DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
					LastChanged = lastChanged,
					CompanyCode = "",
					CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAYPAL,
				};
				new UserCreditCardService().Insert(userCreditCard, updateHistoryAction, accessor);
				return userCreditCard;
			}
		}

		/// <summary>
		/// PayPalログ書き込み
		/// </summary>
		/// <param name="result">Transaction実行結果（例外の場合はnull）</param>
		/// <param name="kbn">区分</param>
		/// <param name="info">注文情報</param>
		/// <param name="message">メッセージ</param>
		private static void WritePayPalLog(Result<Transaction> result, PaymentFileLogger.PaymentProcessingType kbn, string info, string message)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL,
				PaymentFileLogger.PaymentType.PayPal,
				kbn,
				string.Format(
					"[{0}] {1} ({2}) - {3}",
					(result != null) ? result.IsSuccess() ? "成功" : "失敗" : "例外",
					kbn,
					info,
					message));
		}

		/// <summary>
		/// アドレスリクエストラッパー
		/// </summary>
		public class AddressRequestWrapper
		{
			/// <summary>US州コード（離島など州選択のプルダウンに存在しないものはコメントアウトしている）</summary>
			private readonly Dictionary<string, string> m_usStateCodes = new Dictionary<string, string>
			{
				{"Alabama", "AL"},
				{"Alaska", "AK"},
				//{"American Samoa", "AS"},
				{"Arizona", "AZ"},
				{"Arkansas", "AR"},
				{"California", "CA"},
				{"Colorado", "CO"},
				{"Connecticut", "CT"},
				{"Delaware", "DE"},
				//{"Dist. of Columbia", "DC"},
				{"Florida", "FL"},
				{"Georgia", "GA"},
				//{"Guam", "GU"},
				{"Hawaii", "HI"},
				{"Idaho", "ID"},
				{"Illinois", "IL"},
				{"Indiana", "IN"},
				{"Iowa", "IA"},
				{"Kansas", "KS"},
				{"Kentucky", "KY"},
				{"Louisiana", "LA"},
				{"Maine", "ME"},
				{"Maryland", "MD"},
				//{"Marshall Islands", "MH"},
				{"Massachusetts", "MA"},
				{"Michigan", "MI"},
				//{"Micronesia", "FM"},
				{"Minnesota", "MN"},
				{"Mississippi", "MS"},
				{"Missouri", "MO"},
				{"Montana", "MT"},
				{"Nebraska", "NE"},
				{"Nevada", "NV"},
				{"New Hampshire", "NH"},
				{"New Jersey", "NJ"},
				{"New Mexico", "NM"},
				{"New York", "NY"},
				{"North Carolina", "NC"},
				{"North Dakota", "ND"},
				//{"Northern Marianas", "MP"},
				{"Ohio", "OH"},
				{"Oklahoma", "OK"},
				{"Oregon", "OR"},
				//{"Palau", "PW"},
				{"Pennsylvania", "PA"},
				//{"Puerto Rico", "PR"},
				{"Rhode Island", "RI"},
				{"South Carolina", "SC"},
				{"South Dakota", "SD"},
				{"Tennessee", "TN"},
				{"Texas", "TX"},
				{"Utah", "UT"},
				{"Vermont", "VT"},
				{"Virginia", "VA"},
				//{"Virgin Islands", "VI"},
				{"Washington", "WA"},
				{"West Virginia", "WV"},
				{"Wisconsin", "WI"},
				{"Wyoming", "WY"},
			};

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="cart">カート</param>
			public AddressRequestWrapper(CartObject cart)
			{
				this.AddressRequest = CreateAddressRequest(
					cart.Shippings[0].Name1,
					cart.Shippings[0].Name2,
					cart.Shippings[0].CompanyName,
					cart.Shippings[0].Zip,
					cart.Shippings[0].Addr1,
					cart.Shippings[0].Addr2,
					cart.Shippings[0].Addr3,
					cart.Shippings[0].Addr4,
					cart.Shippings[0].Addr5,
					cart.Shippings[0].ShippingCountryIsoCode);
			}
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文モデル</param>
			public AddressRequestWrapper(OrderModel order)
			{
				this.AddressRequest = this.AddressRequest = CreateAddressRequest(
					order.Shippings[0].ShippingName1,
					order.Shippings[0].ShippingName2,
					order.Shippings[0].ShippingCompanyName,
					order.Shippings[0].ShippingZip,
					order.Shippings[0].ShippingAddr1,
					order.Shippings[0].ShippingAddr2,
					order.Shippings[0].ShippingAddr3,
					order.Shippings[0].ShippingAddr4,
					order.Shippings[0].ShippingAddr5,
					order.Shippings[0].ShippingCountryIsoCode);
			}

			/// <summary>
			/// AddressRequest作成
			/// </summary>
			/// <param name="shippingName1">姓</param>
			/// <param name="shippingName2">名</param>
			/// <param name="shippingCompanyName">会社名</param>
			/// <param name="shippingZip">郵便番号</param>
			/// <param name="shippingAddr1">住所1</param>
			/// <param name="shippingAddr2">住所2</param>
			/// <param name="shippingAddr3">住所3</param>
			/// <param name="shippingAddr4">住所4</param>
			/// <param name="shippingAddr5">住所5</param>
			/// <param name="shippingCountryIsoCode">国コード</param>
			/// <returns>AddressRequest</returns>
			private AddressRequest CreateAddressRequest(
				string shippingName1,
				string shippingName2,
				string shippingCompanyName,
				string shippingZip,
				string shippingAddr1,
				string shippingAddr2,
				string shippingAddr3,
				string shippingAddr4,
				string shippingAddr5,
				string shippingCountryIsoCode)
			{
				var addressRequest = new AddressRequest
				{
					FirstName = shippingName2,
					LastName = shippingName1,
					Company = shippingCompanyName,
					PostalCode = shippingZip,
					CountryCodeAlpha2 = Constants.GLOBAL_OPTION_ENABLE ? shippingCountryIsoCode : Constants.COUNTRY_ISO_CODE_JP,
				};

				// 国による住所の切り替え
				if ((Constants.GLOBAL_OPTION_ENABLE == false)
					|| (shippingCountryIsoCode == Constants.COUNTRY_ISO_CODE_JP)
					|| (shippingCountryIsoCode == Constants.COUNTRY_ISO_CODE_TW))
				{
					addressRequest.Region = (string.IsNullOrEmpty(shippingAddr5)) ? shippingAddr1 : shippingAddr5;
					addressRequest.Locality = shippingAddr2;
					addressRequest.StreetAddress = shippingAddr3;
					addressRequest.ExtendedAddress = shippingAddr4;
				}
				else
				{
					// USの場合はregion（州）を2文字のコードに切り替える必要がある。
					// また、・Region（州）＆Locality（市）＆PostalCode（郵便番号）の整合性があっていないといけない。
					// 例：IL、Bartlett、60103
					// 例：NY、Queens、11359
					// ※ニューヨーク州ニューヨーク市の場合はNewYorkが2つ続くので変な感じではあるが・・・

					var region = ((shippingCountryIsoCode == Constants.COUNTRY_ISO_CODE_US)
						&& m_usStateCodes.ContainsKey(shippingAddr5))
						? m_usStateCodes[shippingAddr5]
						: shippingAddr5;
					addressRequest.Region = region;
					addressRequest.Locality = shippingAddr4;
					addressRequest.StreetAddress = shippingAddr3;
					addressRequest.ExtendedAddress = shippingAddr2;
				}
				return addressRequest;
			}

			/// <summary>アドレスリクエスト</summary>
			public AddressRequest AddressRequest { get; set; }
		}
	}
}
