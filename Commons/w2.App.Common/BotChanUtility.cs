/*
=========================================================================================================
  Module      : BotChan共有メソッド(BotChanUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Botchan;
using w2.App.Common.Order;
using w2.App.Common.Product;
using w2.Common.Logger;
using w2.Domain.ShopShipping;

namespace w2.App.Common
{
	/// <summary>
	///  BotChan共有メソッド
	/// </summary>
	public class BotChanUtility
	{
		/// <summary>
		/// 共通バリエーション
		/// </summary>
		/// <param name="validate">バリエーション</param>
		/// <param name="apiName">API名</param>
		/// <returns>エラーリスト</returns>
		public static List<BotchanMessageManager.MessagesCode> ValidateRequest(Hashtable validate, string apiName)
		{
			var errorTypeList = new List<BotchanMessageManager.MessagesCode>();

			if (Constants.BOTCHAN_OPTION == false)
			{
				errorTypeList.Add(BotchanMessageManager.MessagesCode.BOTCHAN_OPTION_OFF);
			}

			if ((apiName != Constants.BOTCHAN_API_NAME_LOGIN) && (CheckAuthText((string)validate["AuthText"]) == false))
			{
				errorTypeList.Add(BotchanMessageManager.MessagesCode.INVALID_AUTHENTICATION_KEY);
			}

			return errorTypeList;
		}

		/// <summary>
		/// Create hash sha256
		/// </summary>
		/// <param name="data">data</param>
		/// <returns>Hash SHA256</returns>
		private static string CreateHashSha256(string data)
		{
			var keyBytesForHash = Encoding.UTF8.GetBytes(data);
			using (var sha256 = new System.Security.Cryptography.SHA256CryptoServiceProvider())
			{
				var hash = sha256.ComputeHash(keyBytesForHash);
				var result = BitConverter.ToString(hash).ToLower().Replace("-", "");
				return result;
			}
		}

		/// <summary>
		/// BotChan共有メソッド
		/// </summary>
		/// <param name="authText">認証キー</param>
		/// <returns>認証結果</returns>
		public static bool CheckAuthText(string authText)
		{
			var authKey = CreateHashSha256(Constants.SECRET_KEY_API_BOTCHAN);
			var result = ((string.IsNullOrEmpty(authText) == false) && (authText == authKey));
			return result;
		}

		/// <summary>
		/// Create Novelty Items
		/// </summary>
		/// <param name="cartId">Cart Id</param>
		/// <param name="cartNoveltyList">Cart Novelty List</param>
		/// <param name="cartList">Cart List</param>
		public static CartProduct[] CreateNoveltyItems(
			string cartId,
			CartNoveltyList cartNoveltyList,
			CartObjectList cartList)
		{
			var cartProducts = new List<CartProduct>();
			CartProduct cartProduct = null;
			var cartNovelty = cartNoveltyList.GetCartNovelty(cartId).FirstOrDefault();

			if (cartNovelty == null) return cartProducts.ToArray();

			var noveltyGrantItems = cartNovelty.GrantItemList;

			foreach (var noveltyGrantItem in noveltyGrantItems)
			{
				// カートに追加していない？
				if (cartList.Items.SelectMany(cart
					=> cart.Items.Where(item
						=> (item.NoveltyId == noveltyGrantItem.NoveltyId))).Any() == false)
				{
					// 付与アイテムが商品マスタに存在する?
					var product = ProductCommon.GetProductVariationInfo(
						noveltyGrantItem.ShopId,
						noveltyGrantItem.ProductId,
						noveltyGrantItem.VariationId,
						cartList.MemberRankId);
					if (product.Count != 0)
					{
						// カート商品（ノベルティID含む）を作成し、カートに追加
						cartProduct = new CartProduct(
							product[0],
							Constants.AddCartKbn.Normal,
							string.Empty,
							1,
							true,
							new ProductOptionSettingList());
						cartProduct.NoveltyId = noveltyGrantItem.NoveltyId;
					}
				}
				cartProducts.Add(cartProduct);
			}
			return cartProducts.ToArray();
		}

		/// <summary>
		/// アクセスユーザのIpv4アドレスを取得
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>IP address</returns>
		public static string GetIpAddress(HttpRequest request)
		{
			var result = request.ServerVariables["REMOTE_ADDR"];

			if (string.IsNullOrEmpty(result))
			{
				result = request.ServerVariables["REMOTE_ADDR"];
			}

			if (string.IsNullOrEmpty(result))
			{
				result = request.UserHostAddress;
			}

			if ((result == "::1") || string.IsNullOrEmpty(result))
			{
				result = "127.0.0.1";
			}
			
			return result;
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailData">メールデータ</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		public void SendMail(string mailId, string userId, Hashtable mailData, string languageCode = null, string languageLocaleId = null)
		{
			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
			{
				if ((string)mailData[Constants.FIELD_USER_MAIL_ADDR] != "")
				{
					using (var msMailSend = new MailSendUtility(
						Constants.CONST_DEFAULT_SHOP_ID,
						mailId,
						userId,
						mailData,
						true,
						Constants.MailSendMethod.Auto,
						languageCode,
						languageLocaleId,
						(string)mailData[Constants.FIELD_USER_MAIL_ADDR]))
					{
						msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());

						if (msMailSend.SendMail() == false)
						{
							AppLogger.WriteError(this.GetType().BaseType + " : " + msMailSend.MailSendException.Message);
						}
					}
				}

				if ((string)mailData[Constants.FIELD_USER_MAIL_ADDR2] != "")
				{
					using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, false, Constants.MailSendMethod.Auto, userMailAddress: (string)mailData[Constants.FIELD_USER_MAIL_ADDR]))
					{
						msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR2].ToString());

						if (msMailSend.SendMail() == false)
						{
							AppLogger.WriteError(this.GetType().BaseType + " : " + msMailSend.MailSendException.Message);
						}
					}
				}
			}
			else
			{
				using (var msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, mailId, userId, mailData, true, Constants.MailSendMethod.Auto, languageCode, languageLocaleId, (string)mailData[Constants.FIELD_USER_MAIL_ADDR]))
				{
					msMailSend.AddTo(mailData[Constants.FIELD_USER_MAIL_ADDR].ToString());

					if (msMailSend.SendMail() == false)
					{
						AppLogger.WriteError(this.GetType().BaseType + " : " + msMailSend.MailSendException.Message);
					}
				}
			}
		}

		/// <summary>
		/// 配送不可エリアチェック
		/// </summary>
		/// <param name="cartObject">カート</param>
		/// <returns>配送不可エリアならtrue</returns>
		public static bool CheckUnavailableShippingAreaForBotChat(CartObject cartObject)
		{
			var unavailableShippingZip = new ShopShippingService().GetUnavailableShippingZipFromShippingDelivery(
				cartObject.ShippingType,
				cartObject.Shippings[0].DeliveryCompanyId);
			var shippingZip = cartObject.Shippings[0].HyphenlessZip;

			return OrderCommon.CheckUnavailableShippingArea(unavailableShippingZip, shippingZip);
		}
	}
}
