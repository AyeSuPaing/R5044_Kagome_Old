/*
=========================================================================================================
  Module      : メールコンテンツ名ヘルパ(MailContentsNameHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order;
using w2.App.Common.Web.Page;
using w2.Domain.Order;

namespace w2.App.Common.Mail
{
	/// <summary>
	/// メールコンテンツ名ヘルパ
	/// </summary>
	public class MailContentsNameHelper
	{
		/// <summary>コンテンツ表示名タイプ</summary>
		public enum ContentsDispNameType
		{
			/// <summary>商品ID</summary>
			ProductId,
			/// <summary>商品名</summary>
			ProductName,
			/// <summary>商品単価</summary>
			ProductPrice,
			/// <summary>数量</summary>
			Quantity,
		}

		/// <summary>コンテンツ表示名リスト（グローバル）</summary>
		private static readonly Dictionary<ContentsDispNameType, string> m_contentsTag =
			new Dictionary<ContentsDispNameType, string>
			{
				{ ContentsDispNameType.ProductId, "@@MailTemplate.product_id.name@@" },
				{ ContentsDispNameType.ProductName, "@@MailTemplate.product_name.name@@" },
				{ ContentsDispNameType.ProductPrice, "@@MailTemplate.product_price.name@@" },
				{ ContentsDispNameType.Quantity, "@@MailTemplate.product_quantity.name@@" },
			};

		/// <summary>
		/// コンテンツ名取得
		/// </summary>
		/// <param name="contentsDispNameType">コンテンツ名タイプ</param>
		/// <param name="cart">カート</param>
		/// <returns>コンテンツ名</returns>
		public static string GetContentsName(
			ContentsDispNameType contentsDispNameType,
			CartObject cart)
		{
			var result = GetContentsName(contentsDispNameType, cart.Owner.DispLanguageLocaleId);
			return result;
		}
		/// <summary>
		/// コンテンツ名取得
		/// </summary>
		/// <param name="contentsDispNameType">コンテンツ名タイプ</param>
		/// <param name="orderForMail">注文情報</param>
		/// <returns>コンテンツ名</returns>
		public static string GetContentsName(
			ContentsDispNameType contentsDispNameType,
			OrderModel orderForMail)
		{
			var owner = new OrderOwnerModel(orderForMail.DataSource);
			var result = GetContentsName(contentsDispNameType, owner.DispLanguageLocaleId);
			return result;
		}
		/// <summary>
		/// コンテンツ名取得
		/// </summary>
		/// <param name="contentsDispNameType">コンテンツ名タイプ</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>コンテンツ名</returns>
		public static string GetContentsName(
			ContentsDispNameType contentsDispNameType,
			string languageLocaleId)
		{
			var result = CommonPage.ReplaceTagByLocaleId(
				m_contentsTag[contentsDispNameType],
				languageLocaleId);
			return result;
		}
	}
}
