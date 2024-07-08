/*
=========================================================================================================
  Module      : 海外配送エリア構成サービス (GlobalShippingAreaComponentService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Sql;
using w2.Domain.GlobalShipping.Helper;

namespace w2.Domain.GlobalShipping
{
	/// <summary>
	/// 海外配送周りのサービスクラス
	/// </summary>
	public class GlobalShippingService : ServiceBase, IGlobalShippingService
	{
		/// <summary>
		/// 海外配送エリア一覧検索
		/// </summary>
		/// <param name="cond">条件</param>
		/// <returns>検索結果</returns>
		public GlobalShippingAreaListSearchResult[] SearchGlobalShippingAreaList(GlobalShippingAreaListSearchCondition cond)
		{
			var helper = new GlobalShippingAreaListSearch();
			return helper.Search(cond);
		}

		/// <summary>
		/// 海外配送エリア検索件数取得
		/// </summary>
		/// <param name="cond">条件</param>
		/// <returns>Hit件数</returns>
		public int GetSearchGlobalShippingAreaListCount(GlobalShippingAreaListSearchCondition cond)
		{
			var helper = new GlobalShippingAreaListSearch();
			return helper.GetSearchCount(cond);
		}

		/// <summary>
		/// IDをもとに海外配送エリア取得
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>結果</returns>
		public GlobalShippingAreaModel GetGlobalShippingAreaById(string id)
		{
			using (var repository = new GlobalShippingRepository())
			{
				var model = repository.GetGlobalShippingAreaById(id);
				return model;
			}
		}

		/// <summary>
		/// 海外配送エリア登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		public int RegisterGlobalShippingArea(GlobalShippingAreaModel model)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.RegisterGlobalShippingArea(model);
				return res;
			}
		}

		/// <summary>
		/// 海外配送エリア更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		public int UpdateGlobalShippingArea(GlobalShippingAreaModel model)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.UpdateGlobalShippingArea(model);
				return res;
			}
		}

		/// <summary>
		/// 海外配送エリア構成登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		public int RegisterGlobalShippingAreaComponent(GlobalShippingAreaComponentModel model)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.RegisterGlobalShippingAreaComponent(model);
				return res;
			}
		}

		/// <summary>
		/// 海外配送エリア構成削除
		/// </summary>
		/// <param name="seq">シーケンス</param>
		/// <returns>削除件数</returns>
		public int DeleteGlobalShippingAreaComponent(int seq)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.DeleteGlobalShippingAreaComponent(seq);
				return res;
			}
		}

		/// <summary>
		/// 海外配送エリアIDを指定して構成情報取得
		/// </summary>
		/// <param name="areaId">ID</param>
		/// <returns>結果</returns>
		public GlobalShippingAreaComponentModel[] GetAreaComponentByAreaId(string areaId)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.GetAreaComponentByAreaId(areaId);
				return res;
			}
		}

		/// <summary>
		/// 有効な海外配送エリアを取得
		/// </summary>
		/// <returns>結果</returns>
		public GlobalShippingAreaModel[] GetValidGlobalShippingArea()
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.GetValidGlobalShippingArea();
				return res;
			}
		}

		/// <summary>
		/// 海外配送エリアの送料を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別</param>
		/// <param name="areaId">エリア</param>
		/// <returns>結果</returns>
		public GlobalShippingPostageModel[] GetAreaPostage(string shopId, string shippingId, string areaId)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.GetAreaPostage(shopId, shippingId, areaId);
				return res;
			}
		}

		/// <summary>
		/// 配送種別と配送会社で海外配送エリアの送料を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別</param>
		/// <param name="areaId">エリア</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>結果</returns>
		public GlobalShippingPostageModel[] GetAreaPostageByShippingDeliveryCompany(
			string shopId,
			string shippingId,
			string areaId,
			string deliveryCompanyId)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.GetAreaPostageByShippingDeliveryCompany(shopId, shippingId, areaId, deliveryCompanyId);
				return res;
			}
		}

		/// <summary>
		/// 送料登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>登録件数</returns>
		public int RegisterGlobalShippingPostage(GlobalShippingPostageModel model, SqlAccessor accessor = null)
		{
			using (var rep = new GlobalShippingRepository(accessor))
			{
				var res = rep.RegisterGlobalShippingPostage(model);
				return res;
			}
		}

		/// <summary>
		/// 送料削除
		/// </summary>
		/// <param name="seq">シーケンス</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteGlobalShippingPostage(int seq, SqlAccessor accessor = null)
		{
			using (var rep = new GlobalShippingRepository(accessor))
			{
				var res = rep.DeleteGlobalShippingPostage(seq);
				return res;
			}
		}

		/// <summary>
		/// 送料変更
		/// </summary>
		/// <param name="seq">シーケンス</param>
		/// <param name="postage">送料</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int ChangeGlobalShippingPostage(int seq, decimal postage, string lastChanged, SqlAccessor accessor = null)
		{
			using (var rep = new GlobalShippingRepository(accessor))
			{
				var res = rep.ChangeGlobalShippingPostage(seq, postage, lastChanged);
				return res;
			}
		}

		/// <summary>
		/// 住所の情報をもとに海外配送エリアを振り分ける
		/// </summary>
		/// <param name="countryIsoCode">ISO</param>
		/// <param name="addr5">住所5</param>
		/// <param name="addr4">住所4</param>
		/// <param name="addr3">住所3</param>
		/// <param name="addr2">住所2</param>
		/// <param name="zip">郵便番号</param>
		/// <returns>該当するエリア</returns>
		public GlobalShippingAreaComponentModel DistributesShippingArea(string countryIsoCode, string addr5, string addr4, string addr3, string addr2, string zip)
		{
			// まずは国でとってくる
			// 国で取れなければ該当なし、null返す
			var component = this.GetAreaComponentByCountry(countryIsoCode);
			if (component.Any() == false) { return null; }

			// 条件にHitするのもを抽出
			var matched = component
				.Where(c =>
					(string.IsNullOrEmpty(c.ConditionAddr5) || (c.ConditionAddr5.ToLower().Trim().Replace(" ", "") == addr5.ToLower().Trim().Replace(" ", "")))
					&& (string.IsNullOrEmpty(c.ConditionAddr4) || (c.ConditionAddr4.ToLower().Trim().Replace(" ", "") == addr4.ToLower().Trim().Replace(" ", "")))
					&& (string.IsNullOrEmpty(c.ConditionAddr3) || (c.ConditionAddr3.ToLower().Trim().Replace(" ", "") == addr3.ToLower().Trim().Replace(" ", "")))
					&& (string.IsNullOrEmpty(c.ConditionAddr2) || (c.ConditionAddr2.ToLower().Trim().Replace(" ", "") == addr2.ToLower().Trim().Replace(" ", "")))
					&& (string.IsNullOrEmpty(c.ConditionZip) || (c.ConditionZip.ToLower().Trim().Replace(" ", "") == zip.ToLower().Trim().Replace(" ", "")))
				);

			// 該当したものが1件ならそのまま返す
			if (matched.Count() == 1) { return matched.FirstOrDefault(); }

			// 住所5の条件が空ではないものが1つある場合はそれを返却
			if (matched.Count(c => string.IsNullOrEmpty(c.ConditionAddr5) == false) == 1) { return matched.FirstOrDefault(c => string.IsNullOrEmpty(c.ConditionAddr5) == false); }
			// 住所4の条件が空ではないものが1つある場合はそれを返却
			else if (matched.Count(c => string.IsNullOrEmpty(c.ConditionAddr4) == false) == 1) { return matched.FirstOrDefault(c => string.IsNullOrEmpty(c.ConditionAddr4) == false); }
			// 住所3の条件が空ではないものが1つある場合はそれを返却
			else if (matched.Count(c => string.IsNullOrEmpty(c.ConditionAddr3) == false) == 1) { return matched.FirstOrDefault(c => string.IsNullOrEmpty(c.ConditionAddr3) == false); }
			// 住所2の条件が空ではないものが1つある場合はそれを返却
			else if (matched.Count(c => string.IsNullOrEmpty(c.ConditionAddr2) == false) == 1) { return matched.FirstOrDefault(c => string.IsNullOrEmpty(c.ConditionAddr2) == false); }
			// 郵便番号の条件が空ではないものが1つある場合はそれを返却
			else if (matched.Count(c => string.IsNullOrEmpty(c.ConditionZip) == false) == 1) { return matched.FirstOrDefault(c => string.IsNullOrEmpty(c.ConditionZip) == false); }

			// 特定しきれない場合はシーケンスが大きいものを優先
			return matched.OrderByDescending(c => c.Seq).FirstOrDefault();

		}

		/// <summary>
		/// 国のエリア構成取得
		/// </summary>
		/// <param name="countryIsoCode">ISO</param>
		/// <returns>結果</returns>
		public GlobalShippingAreaComponentModel[] GetAreaComponentByCountry(string countryIsoCode)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.GetAreaComponentByCountry(countryIsoCode);
				return res;
			}
		}

		/// <summary>
		/// 配送種別を指定して送料削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>削除件数</returns>
		public int DeleteGlobalShippingPostageByShipping(string shopId, string shippingId)
		{
			using (var rep = new GlobalShippingRepository())
			{
				var res = rep.DeleteGlobalShippingPostageByShipping(shopId, shippingId);
				return res;
			}
		}
		/// <summary>
		/// 配送種別を指定して送料削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">アクセサ（トランザクション含む）</param>
		/// <returns>削除件数</returns>
		public int DeleteGlobalShippingPostageByShipping(string shopId, string shippingId, SqlAccessor accessor)
		{
			using (var rep = new GlobalShippingRepository(accessor))
			{
				var res = rep.DeleteGlobalShippingPostageByShipping(shopId, shippingId);
				return res;
			}
		}

		/// <summary>
		/// 配送種別と配送会社を指定して送料削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteGlobalShippingPostageByShippingDeliveryCompany(
			string shopId,
			string shippingId,
			string deliveryCompanyId,
			SqlAccessor accessor = null)
		{
			using (var rep = new GlobalShippingRepository(accessor))
			{
				var res = rep.DeleteGlobalShippingPostageByShippingDeliveryCompany(
					shopId,
					shippingId,
					deliveryCompanyId);
				return res;
			}
		}

		/// <summary>
		/// 配送種別を指定してまるっとコピー
		/// コピー前にいったんクリーニングします
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fromShippingId">コピー元配送種別</param>
		/// <param name="fromDeliveryCompanyId">コピー元配送会社</param>
		/// <param name="toShippingId">コピー先配送種別</param>
		/// <param name="toDeliveryCompanyId">コピー先配送会社</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>結果件数</returns>
		public int CopyGlobalShippingPostageByShippingDeliveryCompany(
			string shopId,
			string fromShippingId,
			string fromDeliveryCompanyId,
			string toShippingId,
			string toDeliveryCompanyId,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var rep = new GlobalShippingRepository(accessor))
			{
				rep.DeleteGlobalShippingPostageByShippingDeliveryCompany(shopId, toShippingId, toDeliveryCompanyId);
				var res = rep.CopyGlobalShippingPostageByShippingDeliveryCompany(
					shopId,
					fromShippingId,
					fromDeliveryCompanyId,
					toShippingId,
					toDeliveryCompanyId,
					lastChanged);
				return res;
			}
		}

		/// <summary>
		/// 重量の範囲が重複しているかどうか
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>
		/// True：重複している
		/// False：重複していない
		/// </returns>
		public bool DuplicationWeightRange(GlobalShippingPostageModel model)
		{
			var areaPostage = this.GetAreaPostageByShippingDeliveryCompany(
				model.ShopId,
				model.ShippingId,
				model.GlobalShippingAreaId,
				model.DeliveryCompanyId);

			var isHit = Enumerable
				.Range((int)model.WeightGramGreaterThanOrEqualTo, (int)(model.WeightGramLessThan - model.WeightGramGreaterThanOrEqualTo + 1))
				.Any(i => areaPostage.Any(p => p.WeightGramGreaterThanOrEqualTo <= i && i <= p.WeightGramLessThan));

			return isHit;
		}
	}
}
