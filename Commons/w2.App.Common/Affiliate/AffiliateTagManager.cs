/*
=========================================================================================================
  Module      : アフィリエイトタグマネージャークラス(AffiliateTagManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Global.Region;
using w2.App.Common.Order;
using w2.Domain.Affiliate;
using w2.Domain.LandingPage;
using w2.Domain.User;

namespace w2.App.Common.Affiliate
{
	/// <summary>
	/// アフィリエイトタグマネージャークラス
	/// </summary>
	public class AffiliateTagManager
	{
		/// <summary>
		/// 出力可能なアフィリエイトタグの取得
		/// </summary>
		/// <param name="url">アクセスページURL</param>
		/// <param name="location">出力位置(haed, body下, body上)</param>
		/// <param name="device">デバイスタイプ</param>
		/// <param name="allTag">全ページ出力用タグかどうか</param>
		/// <param name="date">タグ外部からの取得データ</param>
		/// <param name="affiliateCooperationSessionDate">セッション取得データ</param>
		/// <returns>アフィリエイトタグ出力内容リスト</returns>
		public List<AffiliateTagResult> GetAffiliateTag(
			string url,
			string location,
			string device,
			bool allTag,
			object date,
			AffiliateCooperationSessionDate affiliateCooperationSessionDate)
		{
			// データ指定があるページ以外はセッションから情報取得
			var targetPage = TagSetting.GetInstance().Setting.TargetPages
				.FirstOrDefault(i => url.Contains(i.Path));
			var actionType = (targetPage != null) ? targetPage.ActionType : TagSetting.ACTION_TYPE_SESSION_ONLY;

			var logging = (targetPage != null) && targetPage.Logging;

			// ページ内のプロパティを利用する際はmasterにある全ページ共通出力タグではなくページに配置したタグより出力する
			if ((actionType != TagSetting.ACTION_TYPE_SESSION_ONLY) && (date == null) && allTag)
				return new List<AffiliateTagResult>();

			var models = DataCacheControllerFacade.GetAffiliateTagSettingCacheController()
				.GetAffiliateTagSettingModels();
			var tags = models
				// 条件判定
				.Where(m => ConditionCheck(m, url, location, device, actionType, date, affiliateCooperationSessionDate))
				// ソート
				.OrderBy(m => m.DisplayOrder)
				// 置換処理
				.Select(m => ReplaceAction(m, actionType, date, affiliateCooperationSessionDate, logging)).ToList();
			return tags;
		}

		/// <summary>
		/// 条件判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="url">アクセスページURL</param>
		/// <param name="location">出力位置(haed, body下, body上)</param>
		/// <param name="device">デバイスタイプ</param>
		/// <param name="actionType">アクションタイプ</param>
		/// <param name="date">タグ外部からの取得データ</param>
		/// <param name="affiliateCooperationSessionDate">セッション取得データ</param>
		/// <returns>結果</returns>
		private bool ConditionCheck(
			AffiliateTagSettingModel model,
			string url,
			string location,
			string device,
			string actionType,
			object date,
			AffiliateCooperationSessionDate affiliateCooperationSessionDate)
		{
			var pageCheck = PageCheck(model, url);

			if (pageCheck == false) return false;

			if (LandingCartCheck(model, url, actionType) == false) return false;

			if (LocationCheck(model, location) == false) return false;

			if (DeviceCheck(model, device) == false) return false;

			if (AdvCodeCheck(model, affiliateCooperationSessionDate.AdvCodeNow) == false) return false;

			if (AdvMediaTypeCheck(model, affiliateCooperationSessionDate.AdvCodeNow) == false) return false;

			if (ProductCheck(model, actionType, date, affiliateCooperationSessionDate.CartList) == false) return false;

			return true;
		}

		/// <summary>
		/// ページ判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="url">アクセスページURL</param>
		/// <returns>結果</returns>
		private bool PageCheck(AffiliateTagSettingModel model, string url)
		{
			var result = model.AffiliateTagConditionList.Any(
				c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE)
					&& ((url.Contains(c.ConditionValue) || (c.ConditionValue == ""))
					|| (url.Contains(Constants.CARTLIST_LP_FILE_NAME) && c.ConditionValue.Contains(Constants.CARTLIST_TAG_NAME))));

			return result;
		}

		/// <summary>
		/// ランディングカート用判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="url">アクセスページURL</param>
		/// <param name="actionType">アクションタイプ</param>
		/// <returns>結果</returns>
		private bool LandingCartCheck(AffiliateTagSettingModel model, string url, string actionType)
		{
			// 全ページ出力または、新LPカートリストかつカートリスト出力は判定スルー
			var isThroughPage = model.AffiliateTagConditionList.Any(
				c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PAGE) && ((c.ConditionValue == "")
					|| (url.Contains(Constants.CARTLIST_LP_FILE_NAME) && c.ConditionValue.Contains(Constants.CARTLIST_TAG_NAME))));
			if (isThroughPage) return true;

			// CMS 新LP
			if (url.ToLower().Contains(Constants.PATH_LANDING_FORMLP.ToLower()))
			{
				var cash = DataCacheControllerFacade
					.GetLandingPageDeCacheController()
					.CacheData
					.FirstOrDefault(s => (s.PageFileName == Path.GetFileNameWithoutExtension(url))) ?? new LandingPageDesignModel();
				var lp = cash.Clone();
				if ((actionType != TagSetting.ACTION_TYPE_LANDING_CART) && string.IsNullOrEmpty(lp.TagSettingList)) return true;

				foreach (var tag in lp.TagSettingList.Split(','))
				{
					var result = model.AffiliateTagConditionList.Any(condition => condition.AffiliateId.ToString() == tag); 
					if (result) return true;
				}

				return false;
			}

			// 既存LP
			if (url.ToLower().Contains(Constants.PATH_LANDING.ToLower()))
			{
				return (actionType == TagSetting.ACTION_TYPE_LANDING_CART);
			}

			// その他ページは判定スルー
			return true;
		}

		/// <summary>
		/// 出力位置判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="location">出力位置(haed, body下, body上)</param>
		/// <returns>結果</returns>
		private bool LocationCheck(AffiliateTagSettingModel model, string location)
		{
			var result = (model.OutputLocation == location);
			return result;
		}

		/// <summary>
		/// デバイス判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="device">デバイスタイプ</param>
		/// <returns>結果</returns>
		private bool DeviceCheck(AffiliateTagSettingModel model, string device)
		{
			if (model.AffiliateKbn == Constants.FLG_AFFILIATETAGSETTING_AFFILIATE_KBN_PC_SP) return true;

			var result = (model.AffiliateKbn == device);
			return result;
		}

		/// <summary>
		/// 広告コード判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="advCode">広告コード</param>
		/// <returns>結果</returns>
		private bool AdvCodeCheck(AffiliateTagSettingModel model, string advCode)
		{
			var result = model.AffiliateTagConditionList.Any(
				c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADVERTISEMENT_CODE)
					&& (ConditionMatchCheck(advCode, c.ConditionValue, c.MatchType) || (c.ConditionValue == "")));
			return result;
		}

		/// <summary>
		/// メディアタイプ判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="advCode">広告コード</param>
		/// <returns>結果</returns>
		private bool AdvMediaTypeCheck(AffiliateTagSettingModel model, string advCode)
		{
			var advCodeModel = DataCacheControllerFacade.GetAdvCodeCacheController().GetAdvCodemodel(advCode);

			if (advCodeModel == null)
				return model.AffiliateTagConditionList.Any(
					c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE)
						&& (c.ConditionValue == ""));

			var result = model.AffiliateTagConditionList.Any(
				c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_ADCODE_MEDIA_TYPE)
					&& (ConditionMatchCheck(advCodeModel.AdvcodeMediaTypeId, c.ConditionValue, c.MatchType)
						|| (c.ConditionValue == "")));

			return result;
		}

		/// <summary>
		/// 商品判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="actionType">アクションタイプ</param>
		/// <param name="date">タグ外部からの取得データ</param>
		/// <param name="cart">カートデータ</param>
		/// <returns>結果</returns>
		private bool ProductCheck(AffiliateTagSettingModel model, string actionType, object date, CartObjectList cart)
		{
			switch (actionType)
			{
				case TagSetting.ACTION_TYPE_ORDER:
					return ((date != null) && (date.GetType() == typeof(List<DataView>)))
						&& OrderProductCheck(model, (List<DataView>)date);

				case TagSetting.ACTION_TYPE_SESSION_ONLY:
					return CartProductCheck(model, cart);

				case TagSetting.ACTION_TYPE_LANDING_CART:
					return ((date != null) && (date.GetType() == typeof(CartObjectList)))
						&& CartProductCheck(model, (CartObjectList)date);

				default:
					return false;
			}
		}

		/// <summary>
		/// 注文データ用 商品判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="order">注文データ</param>
		/// <returns>結果</returns>
		private bool OrderProductCheck(AffiliateTagSettingModel model, List<DataView> order)
		{
			var result = order
				.SelectMany(o => o.Cast<DataRowView>().Select(p => (string)p[Constants.FIELD_ORDERITEM_PRODUCT_ID]))
				.Distinct().Any(
					productId => model.AffiliateTagConditionList.Any(
						c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID)
							&& (ConditionMatchCheck(productId, c.ConditionValue, c.MatchType)
								|| (c.ConditionValue == ""))));
			return result;
		}

		/// <summary>
		/// カートデータ用 商品判定
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="cart">カートデータ</param>
		/// <returns>結果</returns>
		private bool CartProductCheck(AffiliateTagSettingModel model, CartObjectList cart)
		{
			if (((cart == null) || (cart.Items.Count == 0)) && model.AffiliateTagConditionList.Any(
				m => (m.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID)
					&& (m.ConditionValue == ""))) return true;

			if (cart == null) return false;

			var result = cart.Items.SelectMany(c => c.Items.Select(cp => cp.ProductId)).Distinct().Any(
				productId => model.AffiliateTagConditionList.Any(
					c => (c.ConditionType == Constants.FLG_AFFILIATETAGCONDITION_CONDITION_TYPE_PRODUCT_ID)
						&& (ConditionMatchCheck(productId, c.ConditionValue, c.MatchType)
							|| (c.ConditionValue == ""))));
			return result;
		}

		/// <summary>
		/// 置換処理
		/// </summary>
		/// <param name="model">アフィリエイトタグモデル</param>
		/// <param name="actionType">アクションタイプ</param>
		/// <param name="date">タグ外部からの取得データ</param>
		/// <param name="affiliateCooperationSessionDate">セッションデータ</param>
		/// <param name="logging">ロギング</param>
		/// <returns>アフィリエイトタグ内容</returns>
		private AffiliateTagResult ReplaceAction(
			AffiliateTagSettingModel model,
			string actionType,
			object date,
			AffiliateCooperationSessionDate affiliateCooperationSessionDate,
			bool logging)
		{
			var affiliateTagKey = string.Empty;
			var affiliateTag = model.AffiliateTag1;
			var affiliateName = "";
			var affiliateId = 0;
			switch (actionType)
			{
				case TagSetting.ACTION_TYPE_ORDER:
					if (date.GetType() == typeof(List<DataView>))
					{
						affiliateTag = new OrderReplaceTagManager().ReplaceTag(
							model,
							(List<DataView>)date,
							affiliateTag,
							affiliateCooperationSessionDate.RegionModel);
						affiliateTag = new LoginUserReplaceTagManager().ReplaceTag(
							affiliateCooperationSessionDate.LoginUser,
							affiliateTag);
						affiliateName = model.AffiliateName;
						affiliateId = model.AffiliateId;
						foreach (var d in (List<DataView>)date)
						{
							affiliateTagKey = (string)d[0][Constants.FIELD_ORDER_ORDER_ID];
						}
					}
					else
					{
						affiliateTag = string.Empty;
					}

					break;

				case TagSetting.ACTION_TYPE_SESSION_ONLY:

					affiliateTag = new LoginUserReplaceTagManager().ReplaceTag(
						affiliateCooperationSessionDate.LoginUser,
						affiliateTag);
					affiliateTag = new CartReplaceTagManager().ReplaceTag(
						model,
						affiliateCooperationSessionDate.CartList,
						affiliateTag,
						affiliateCooperationSessionDate.RegionModel);
					affiliateName = model.AffiliateName;
					affiliateId = model.AffiliateId;
					affiliateTagKey = (affiliateCooperationSessionDate.LoginUser != null)
						? affiliateCooperationSessionDate.LoginUser.UserId
						: string.Empty;
					break;

				case TagSetting.ACTION_TYPE_LANDING_CART:
					if (date.GetType() == typeof(CartObjectList))
					{
						affiliateTag = new LoginUserReplaceTagManager().ReplaceTag(
							affiliateCooperationSessionDate.LoginUser,
							affiliateTag);
						affiliateTag = new CartReplaceTagManager().ReplaceTag(
							model,
							(CartObjectList)date,
							affiliateTag,
							affiliateCooperationSessionDate.RegionModel);
						affiliateName = model.AffiliateName;
						affiliateId = model.AffiliateId;
						affiliateTagKey = (affiliateCooperationSessionDate.LoginUser != null)
							? affiliateCooperationSessionDate.LoginUser.UserId
							: string.Empty;
					}
					else
					{
						affiliateTag = string.Empty;
					}

					break;
			}

			var result = new AffiliateTagResult(affiliateTagKey, affiliateTag, affiliateName, logging, affiliateId);
			return result;
		}

		/// <summary>
		/// 条件の一致判定
		/// </summary>
		/// <param name="value1">データ1</param>
		/// <param name="value2">データ2</param>
		/// <param name="type">一致タイプ 完全一致　前方一致</param>
		/// <returns></returns>
		private bool ConditionMatchCheck(string value1, string value2, string type)
		{
			var result = (type == Constants.FLG_AFFILIATETAGCONDITION_MATCH_TYPE_PERFECT)
				? (value1 == value2)
				: ((value1 != null) && value1.StartsWith(value2));

			return result;
		}
	}

	/// <summary>
	/// アフィリエイトタグ処理結果クラス
	/// </summary>
	public class AffiliateTagResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="logKey">ログキー名</param>
		/// <param name="content">置換処理結果</param>
		/// <param name="affiliateName">アフィリエイト名</param>
		/// <param name="logging">ロギング</param>
		/// <param name="affiliateId">アフィリエイトID</param>
		public AffiliateTagResult(string logKey, string content, string affiliateName, bool logging, int affiliateId)
		{
			this.LogKey = logKey;
			this.Content = content;
			this.AffiliateName = affiliateName;
			this.Logging = logging;
			this.AffiliateId = affiliateId;
		}

		/// <summary>ログキー名</summary>
		public string LogKey { get; set; }
		/// <summary>内容</summary>
		public string Content { get; set; }
		/// <summary>アフィリエイト名</summary>
		public string AffiliateName { get; set; }
		/// <summary>ロギング</summary>
		public bool Logging { get; set; }
		/// <summary>アフィリエイトID</summary>
		public int AffiliateId { get; set; }
	}

	/// <summary>
	/// アフィリエイトタグ表示用セッションデータの一時保管クラス
	/// </summary>
	public class AffiliateCooperationSessionDate
	{
		/// <summary>広告コード</summary>
		public string AdvCodeNow { get; set; }
		/// <summary>カートリスト</summary>
		public CartObjectList CartList { get; set; }
		/// <summary>ログインユーザ</summary>
		public UserModel LoginUser { get; set; }
		/// <summary>ユーザ リージョンモデル</summary>
		public RegionModel RegionModel { get; set; }
	}
}