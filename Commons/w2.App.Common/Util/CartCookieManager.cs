/*
=========================================================================================================
  Module      : カートクッキー管理クラス(CartCookieManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common.Order;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.User.Helper;

namespace w2.App.Common.Util
{
	/// <summary>
	/// CartCookieManager の概要の説明です
	/// </summary>
	public class CartCookieManager
	{
		/// <summary>
		/// クッキーからカートリスト情報復元
		/// </summary>
		/// <param name="loginId">ログインID（生成の際に利用。未ログインであればnull）</param>
		/// <param name="orderKbn">注文区分</param>
		/// <returns>カートリスト</returns>
		public static CartObjectList GetCartListFromCookie(string loginId, string orderKbn)
		{
			string cartIdList = GetCartIdFromCookie();
			if ((cartIdList != "") && (Validator.IsHalfwidthNumber(cartIdList.Replace(",", ""))))
			{
				return CartObjectList.GetUserCartList(StringUtility.ToEmpty(loginId), orderKbn, cartIdList);
			}
			return CartObjectList.GetUserCartList(StringUtility.ToEmpty(loginId), orderKbn);
		}

		/// <summary>
		/// クッキーからカート情報を復元（モバイル向け）
		/// </summary>
		/// <returns>カートリスト</returns>
		public static CartObject GetCartFromCookie()
		{
			string cartIdList = GetCartIdFromCookie();
			if ((cartIdList != "") && (Validator.IsHalfwidthNumber(cartIdList.Replace(",", ""))))
			{
				var cartList = CartObjectList.GetUserCartList("", Constants.FLG_ORDER_ORDER_KBN_MOBILE, cartIdList);
				if (cartList.Items.Count != 0)
				{
					return cartList.Items[0];
				}
			}
			return null;
		}

		/// <summary>
		/// カートIDをCookieから取得
		/// </summary>
		/// <returns>カートID一覧(カンマ区切りの平文)</returns>
		public static string GetCartIdFromCookie()
		{
			var cookie = CookieManager.Get(Constants.COOKIE_KEY_CART_ID);
			return (cookie != null) ? UserPassowordCryptor.PasswordDecrypt(cookie.Value) : "";
		}

		/// <summary>
		/// カートIDをクッキーへ保存（カートが空ならクッキー削除）
		/// </summary>
		/// <param name="cartList">カート一覧</param>
		public static void RefreshCookieCartId(CartObjectList cartList)
		{
			if (cartList.Items.Count == 0)
			{
				RemoveCartCookie();
				return;
			}

			var cartIdList = string.Join(",", cartList.Items.Select(cart => cart.CartId));
			UpdateCookieCartId(cartIdList, Constants.CART_ID_COOKIE_LIMIT_DAYS);
		}

		/// <summary>
		/// カートクッキー削除
		/// </summary>
		public static void RemoveCartCookie()
		{
			CookieManager.RemoveCookie(Constants.COOKIE_KEY_CART_ID, Constants.PATH_ROOT);
		}

		/// <summary>
		/// カートIDをクッキーに更新する
		/// </summary>
		/// <param name="cartIdList">カートID一覧(カンマ区切りの平文)</param>
		/// <param name="limitDays">Cookie有効期限（-1指定でクッキー削除）</param>
		/// <remarks>レスポンス管理のクッキーリストに設定したクッキーを追加</remarks>
		public static void UpdateCookieCartId(string cartIdList, int limitDays)
		{
			CookieManager.SetCookie(
				Constants.COOKIE_KEY_CART_ID,
				UserPassowordCryptor.PasswordEncrypt(cartIdList),
				Constants.PATH_ROOT,
				DateTime.Now.AddDays(limitDays));
		}
	}
}