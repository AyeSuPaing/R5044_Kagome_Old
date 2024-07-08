/*
=========================================================================================================
  Module      : 会員ランク共通処理クラス(MemberRankOptionUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.MemberRank;
using w2.Domain.MemberRankRule;

namespace w2.App.Common.Option
{
	/// <summary>
	/// 会員ランクエラーコード
	/// </summary>
	public enum MemberRankErrorcode
	{
		/// <summary>エラーなし</summary>
		NoError,
		/// <summary>定期会員割引無効エラー</summary>
		FixedPurchaseMemberDiscountInvaildError,
	}

	///*********************************************************************************************
	/// <summary>
	/// 会員ランクオプションユーティリティ
	/// </summary>
	///*********************************************************************************************
	public class MemberRankOptionUtility
	{
		/// <summary>
		/// 会員ランク一覧情報取得
		/// </summary>
		/// <returns>会員ランク情報データビュー</returns>
		public static MemberRankModel[] GetMemberRankList()
		{
			return DataCacheControllerFacade.GetMemberRankCacheController().CacheData;
		}

		/// <summary>
		/// 会員ランク詳細取得
		/// </summary>
		/// <param name="strRankId">会員ランクID</param>
		/// <returns>会員ランク情報データビュー</returns>
		private static MemberRankModel GetMemberRankDetail(string strRankId)
		{
			var memberRankDetail = GetMemberRankList().FirstOrDefault(memberRank => memberRank.MemberRankId == strRankId);
			return memberRankDetail;
		}

		/// <summary>
		/// 会員ランク名取得
		/// </summary>
		/// <param name="strRankId">会員ランクID</param>
		/// <returns>会員ランク名</returns>
		public static string GetMemberRankName(string strRankId)
		{
			var memberRankDetail = GetMemberRankDetail(strRankId);
			var memberRankName = (memberRankDetail != null)
				? memberRankDetail.MemberRankName
				: string.Empty;
			return memberRankName;
		}

		/// <summary>
		/// 会員ランク順取得
		/// </summary>
		/// <param name="strRankId">会員ランクID</param>
		/// <returns>会員ランク順</returns>
		public static int GetMemberRankNo(string strRankId)
		{
			var memberRankDetail = GetMemberRankDetail(strRankId);
			var memberRankNo = (memberRankDetail != null)
				? memberRankDetail.MemberRankOrder
				: 0;
			return memberRankNo;
		}

		/// <summary>
		/// 会員ランクポイント付与加算数取得
		/// </summary>
		/// <param name="strRankId">会員ランクID</param>
		/// <returns>会員ランクポイント付与加算数</returns>
		public static decimal GetMemberRankPointAdd(string strRankId)
		{
			// メンバーランク詳細取得（無ければ0）
			var memberRankDetail = GetMemberRankDetail(strRankId);
			var memberRankPointAdd = (memberRankDetail != null)
				? (memberRankDetail.PointAddType == Constants.FLG_MEMBERRANK_POINT_ADD_TYPE_RATE)
					? memberRankDetail.PointAddValue.GetValueOrDefault(0)
					: 0
				: 0;
			return memberRankPointAdd;
		}

		/// <summary>
		/// 会員ランク比較
		/// </summary>
		/// <param name="strRankId">比較元会員ランクID</param>
		/// <param name="strProductRankId">商品の会員ランクID</param>
		/// <returns>会員ランクを満たしていればTRUE、満たしていなければFALSE</returns>
		public static bool CheckMemberRankPermission(string strRankId, string strProductRankId)
		{
			// 会員ランクオプション使用時のみ
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				// 会員ランク順取得
				var productRankNo = GetMemberRankNo(strProductRankId);
				var orderRankNo = GetMemberRankNo(strRankId);

				var isValid = ((productRankNo == 0)
					|| ((orderRankNo != 0) && (productRankNo >= orderRankNo)));
				return isValid;
			}

			return true;
		}

		/// <summary>
		/// 会員ランクID取得
		/// </summary>
		/// <param name="strUserId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>会員ランク</returns>
		public static string GetMemberRankId(string strUserId, SqlAccessor accessor = null)
		{
			// HACK:ユーザクラスから取得するのが正しい。これがなければw2.App.Common.Userをusingしなくてすむ。依存関係が複雑すぎ。。

			// 会員ランクオプション使用時のみ
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				var memberRankId = DomainFacade.Instance.UserService.GetMemberRankId(strUserId, accessor);
				if (memberRankId != null)
				{
					return memberRankId;
				}
			}
			else
			{
				return "nothing";
			}

			return "";
		}

		/// <summary>
		/// 会員ランク名取得
		/// </summary>
		/// <param name="strUserId">ユーザーID</param>
		/// <returns>会員ランク</returns>
		public static string GetMemberRankNameFromUserId(string strUserId)
		{
			return GetMemberRankName(GetMemberRankId(strUserId));
		}

		/// <summary>
		/// 会員ランク割引対象商品チェック
		/// </summary>
		/// <param name="strShopId">ショップID</param>
		/// <param name="strProductId">商品ID</param>
		/// <returns>会員ランク割引対象フラグ</returns>
		public static bool IsDiscountTarget(string strShopId, string strProductId)
		{
			DataView dvProduct = ProductCommon.GetProductInfoUnuseMemberRankPrice(strShopId, strProductId);

			if (dvProduct.Count != 0)
			{
				return ((string)dvProduct[0][Constants.FIELD_PRODUCT_MEMBER_RANK_DISCOUNT_FLG] == Constants.FLG_PRODUCT_MEMBER_RANK_DISCOUNT_FLG_VALID);
			}

			return false;
		}

		/// <summary>
		/// 会員ランク割引金額取得
		/// </summary>
		/// <param name="rankId">会員ランクID</param>
		/// <param name="priceSubtotal">計算対象となる商品小計</param>
		/// <returns>会員ランク割引額</returns>
		public static decimal GetDiscountPrice(string rankId, decimal priceSubtotal)
		{
			var discountPrice = 0m;

			//------------------------------------------------------
			// 会員ランク詳細取得
			//------------------------------------------------------
			var memberRankDetail = GetMemberRankDetail(rankId);
			if (memberRankDetail != null)
			{
				// 割引値取得
				var discountValue = memberRankDetail.OrderDiscountValue.GetValueOrDefault(0);

				//------------------------------------------------------
				// 割引タイプで分岐
				//------------------------------------------------------
				switch (memberRankDetail.OrderDiscountType)
				{
					// 割引しない
					case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE:
						discountPrice = 0;
						break;

					// 割引率指定
					case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE:
						discountPrice = RoundingCalculationUtility.GetRoundPercentDiscountFraction(priceSubtotal, discountValue);

						break;

					// 割引金額指定
					case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED:
						if (IsOverThreshold(GetThresholdPrice(memberRankDetail), priceSubtotal))
						{
							// 割引額が小計を超えていた場合は小計を限度額とする
							discountPrice = (discountValue > priceSubtotal) ? priceSubtotal : discountValue;
						}
						break;
				}
			}

			return discountPrice;
		}

		/// <summary>
		/// 按分後の会員ランク割引金額の取得
		/// </summary>
		/// <param name="rankId">会員ランクID</param>
		/// <param name="priceSubtotal">計算対象となる商品小計</param>
		/// <param name="dPriceTotal">計算対象となる商品小計の合計</param>
		/// <returns>按分後の会員ランク割引金額</returns>
		public static decimal GetDiscountPriceAfterDistribution(string rankId, decimal priceSubtotal, decimal priceSubTotalAll)
		{
			var discountPriceAfterDistribution = 0m;
			//------------------------------------------------------
			// 会員ランク詳細取得
			//------------------------------------------------------
			var memberRankDetail = GetMemberRankDetail(rankId);
			if (memberRankDetail != null)
			{
				// 割引値取得
				var discountValue = memberRankDetail.OrderDiscountValue.GetValueOrDefault(0);

				//------------------------------------------------------
				// 割引タイプで分岐
				//------------------------------------------------------
				switch (memberRankDetail.OrderDiscountType)
				{
					// 割引しない
					case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_NONE:
						discountPriceAfterDistribution = 0m;
						break;

					// 割引率指定
					case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_RATE:
						// 計算方法：商品小計 * 会員ランク割引率　※端数切捨て
						discountPriceAfterDistribution = RoundingCalculationUtility.GetRoundPercentDiscountFraction(priceSubtotal, discountValue);
						break;

					// 割引金額指定
					case Constants.FLG_MEMBERRANK_ORDER_DISCOUNT_TYPE_FIXED:
						if (IsOverThreshold(GetThresholdPrice(memberRankDetail), priceSubtotal))
						{
							// 割引額が合計を超えていた場合は合計を限度額とする
							var discountPrice = (discountValue > priceSubTotalAll) ? priceSubTotalAll : discountValue;

							// 計算方法：(商品小計 / 商品合計 * 会員ランク割引額)　※端数切捨て
							var priceTotalAllTemp = ((priceSubTotalAll != 0) ? priceSubTotalAll : 1);
							discountPriceAfterDistribution = (priceSubtotal / priceTotalAllTemp * discountPrice).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
						}
						break;
				}
			}
			return discountPriceAfterDistribution;
		}

		/// <summary>
		/// 割引金額閾値取得
		/// </summary>
		/// <param name="rank">ランク情報</param>
		/// <returns>割引閾値</returns>
		private static decimal GetThresholdPrice(MemberRankModel rank)
		{
			var thresholdPrice = rank.OrderDiscountThresholdPrice.GetValueOrDefault(0);
			return thresholdPrice;
		}

		/// <summary>
		/// 割引閾値を超えている事を確認
		/// </summary>
		/// <param name="dThresholdPrice">閾値（価格）</param>
		/// <param name="dPriceSubTotal">小計</param>
		/// <returns>成否</returns>
		protected static bool IsOverThreshold(decimal dThresholdPrice, decimal dPriceSubTotal)
		{
			return (dPriceSubTotal >= dThresholdPrice);
		}

		/// <summary>
		/// 会員ランク割引適用後の配送料の取得
		/// </summary>
		/// <param name="rankId">会員ランクID</param>
		/// <param name="shippingPriceTotal">計算対象となる配送料</param>
		/// <returns>会員ランク割引適用後の配送料</returns>
		public static decimal GetMemberRankPriceShipping(string rankId, decimal shippingPriceTotal)
		{
			var memberRankShippingPrice = shippingPriceTotal;
			var memberRankDetail = GetMemberRankDetail(rankId);
			if (memberRankDetail != null)
			{
				//------------------------------------------------------
				// 割引タイプで分岐
				//------------------------------------------------------
				switch (memberRankDetail.ShippingDiscountType)
				{
					// 配送手数料割引しない
					case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_NONE:
						memberRankShippingPrice = shippingPriceTotal;
						break;

					// 割引金額指定
					case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FIXED:
						memberRankShippingPrice = shippingPriceTotal - memberRankDetail.ShippingDiscountValue.GetValueOrDefault(0);

						// 0以下にはしない
						if (memberRankShippingPrice < 0)
						{
							memberRankShippingPrice = 0;
						}
						break;

					// 配送手数料無料
					case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FREE:
						memberRankShippingPrice = 0;
						break;

					// 配送料無料最低金額設定
					// ここではなく、配送先ごとに計算する（ここに入る時点は計算済み）
					case Constants.FLG_MEMBERRANK_SHIPPING_DISCOUNT_TYPE_FRESHP_TSD:
						memberRankShippingPrice = shippingPriceTotal;
						break;
				}
			}

			return memberRankShippingPrice;
		}

		/// <summary>
		/// デフォルト会員ランクの取得
		/// </summary>
		/// <returns>デフォルト会員ランク</returns>
		public static string GetDefaultMemberRank()
		{
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				var memberRank = MemberRankService.GetDefaultMemberRank();
				return (memberRank != null) ? memberRank.MemberRankId : string.Empty;
			}

			return "";
		}

		/// <summary>
		/// 会員ランク付与結果を、会員ランク更新履歴に格納
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="strBeforeRankId">更新前ランクID</param>
		/// <param name="strAfterRankId">更新後ランクID</param>
		/// <param name="strMaiId">メールテンプレートID</param>
		/// <param name="strLastChanged">変更者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public static void InsertUserMemberRankHistory(
			string strUserId,
			string strBeforeRankId,
			string strAfterRankId,
			string strMaiId,
			string strLastChanged,
			SqlAccessor accessor)
		{
			using (var statement = new SqlStatement("UserMemberRank", "InsertUserMemberRankHistory"))
			{
				var ht = new Hashtable
				{
					{ Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID, strUserId },
					{ Constants.FIELD_USERMEMBERRANKHISTORY_BEFORE_RANK_ID, strBeforeRankId },
					{ Constants.FIELD_USERMEMBERRANKHISTORY_AFTER_RANK_ID, strAfterRankId },
					{ Constants.FIELD_USERMEMBERRANKHISTORY_MAIL_ID, strMaiId },
					{ Constants.FIELD_USERMEMBERRANKHISTORY_CHANGED_BY, strLastChanged }
				};
				statement.ExecStatement(accessor, ht);
			}
		}

		/// <summary>
		/// 定期会員割引率を取得
		/// </summary>
		/// <param name="memberRankId">会員ランクID</param>
		/// <returns>定期会員割引率</returns>
		public static decimal GetFixedPurchaseMemberDiscountRate(string memberRankId)
		{
			var memberRankDetail = GetMemberRankDetail(memberRankId);
			var fixedPurchaseMemberDiscountRate
				= ((memberRankDetail != null) ? memberRankDetail.FixedPurchaseDiscountRate : 0M);
			return fixedPurchaseMemberDiscountRate;
		}

		/// <summary>
		/// 会員ランク変動ルール一覧情報取得
		/// </summary>
		/// <returns>会員ランク情報データビュー</returns>
		public static MemberRankRuleModel[] GetMemberRankRuleList()
		{
			return DataCacheControllerFacade.GetMemberRankRuleCacheController().CacheData;
		}

		/// <summary>
		/// 指定されたエラーコードからエラーメッセージ取得
		/// </summary>
		/// <param name="errorCode">会員ランクエラーコード</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(MemberRankErrorcode errorCode)
		{
			switch (errorCode)
			{
				// 定期会員割引無効エラー
				case MemberRankErrorcode.FixedPurchaseMemberDiscountInvaildError:
					return CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_FIXED_PURCHASE_MEMBER_DISCOUNT_INVAILD);

				default:
					return "";
			}
		}
	}
}
