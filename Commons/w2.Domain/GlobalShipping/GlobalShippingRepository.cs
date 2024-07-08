/*
=========================================================================================================
  Module      : 海外配送エリアリポジトリ (GlobalShippingAreaRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.GlobalShipping.Helper;

namespace w2.Domain.GlobalShipping
{
	/// <summary>
	/// 海外配送エリアリポジトリ
	/// </summary>
	internal class GlobalShippingRepository : RepositoryBase
	{
		/// <summary>QUERY用XMLキー</summary>
		private const string XML_KEY_NAME = "GlobalShipping";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal GlobalShippingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal GlobalShippingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		/// <summary>
		/// 海外配送エリア一覧検索
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>検索結果</returns>
		internal GlobalShippingAreaListSearchResult[] SearchGlobalShippingAreaList(GlobalShippingAreaListSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "SearchGlobalShippingAreaList", cond.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new GlobalShippingAreaListSearchResult(drv)).ToArray();
		}

		/// <summary>
		/// 検索件数取得
		/// </summary>
		/// <param name="cond">検索条件</param>
		/// <returns>検索結果</returns>
		internal int GetSearchGlobalShippingAreaListCount(GlobalShippingAreaListSearchCondition cond)
		{
			var dv = base.Get(XML_KEY_NAME, "GetSearchGlobalShippingAreaListCount", cond.CreateHashtableParams());
			if (dv.Count == 0) { return 0; }
			return (int)dv[0][0];
		}

		/// <summary>
		/// IDをもとに海外配送エリア取得
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>結果</returns>
		internal GlobalShippingAreaModel GetGlobalShippingAreaById(string id)
		{
			var ht = new Hashtable { { Constants.FIELD_GLOBALSHIPPINGAREA_GLOBAL_SHIPPING_AREA_ID, id }, };
			var dv = Get(XML_KEY_NAME, "GetGlobalShippingAreaById", ht);
			if (dv.Count == 0) { return null; }
			return new GlobalShippingAreaModel(dv[0]);
		}

		/// <summary>
		/// 海外配送エリア登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int RegisterGlobalShippingArea(GlobalShippingAreaModel model)
		{
			return base.Exec(XML_KEY_NAME, "RegisterGlobalShippingArea", model.DataSource);
		}

		/// <summary>
		/// 海外配送エリア更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>更新件数</returns>
		internal int UpdateGlobalShippingArea(GlobalShippingAreaModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateGlobalShippingArea", model.DataSource);
		}

		/// <summary>
		/// 海外配送エリア構成登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int RegisterGlobalShippingAreaComponent(GlobalShippingAreaComponentModel model)
		{
			return base.Exec(XML_KEY_NAME, "RegisterGlobalShippingAreaComponent", model.DataSource);
		}

		/// <summary>
		/// 海外配送エリア構成削除
		/// </summary>
		/// <param name="seq">シーケンス</param>
		/// <returns>削除件数</returns>
		internal int DeleteGlobalShippingAreaComponent(int seq)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_SEQ, seq);
			return base.Exec(XML_KEY_NAME, "DeleteGlobalShippingAreaComponent", input);
		}

		/// <summary>
		/// 海外配送エリアIDを指定して構成情報取得
		/// </summary>
		/// <param name="areaId">ID</param>
		/// <returns>結果</returns>
		internal GlobalShippingAreaComponentModel[] GetAreaComponentByAreaId(string areaId)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_GLOBAL_SHIPPING_AREA_ID, areaId);
			var dv = base.Get(XML_KEY_NAME, "GetAreaComponentByAreaId", input);
			return dv.Cast<DataRowView>().Select(drv => new GlobalShippingAreaComponentModel(drv)).ToArray();
		}

		/// <summary>
		/// 有効な海外配送エリアを取得
		/// </summary>
		/// <returns>結果</returns>
		internal GlobalShippingAreaModel[] GetValidGlobalShippingArea()
		{
			var dv = base.Get(XML_KEY_NAME, "GetValidGlobalShippingArea");
			return dv.Cast<DataRowView>().Select(drv => new GlobalShippingAreaModel(drv)).ToArray();
		}

		/// <summary>
		/// 海外配送エリアの送料を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別</param>
		/// <param name="areaId">エリア</param>
		/// <returns>結果</returns>
		internal GlobalShippingPostageModel[] GetAreaPostage(string shopId, string shippingId, string areaId)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID, shopId);
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID, shippingId);
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_AREA_ID, areaId);
			var dv = base.Get(XML_KEY_NAME, "GetAreaPostage", input);
			return dv.Cast<DataRowView>().Select(drv => new GlobalShippingPostageModel(drv)).ToArray();
		}

		/// <summary>
		/// 配送種別と配送会社で海外配送エリアの送料を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別</param>
		/// <param name="areaId">エリア</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>結果</returns>
		internal GlobalShippingPostageModel[] GetAreaPostageByShippingDeliveryCompany(
			string shopId,
			string shippingId,
			string areaId,
			string deliveryCompanyId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID, shopId },
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID, shippingId },
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_AREA_ID, areaId },
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DELIVERY_COMPANY_ID, deliveryCompanyId },
			};
			var dv = base.Get(XML_KEY_NAME, "GetAreaPostageByShippingDeliveryCompany", input);
			return dv.Cast<DataRowView>().Select(drv => new GlobalShippingPostageModel(drv)).ToArray();
		}

		/// <summary>
		/// 国のエリア構成取得
		/// </summary>
		/// <param name="countryIsoCode">ISO</param>
		/// <returns>結果</returns>
		internal GlobalShippingAreaComponentModel[] GetAreaComponentByCountry(string countryIsoCode)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGAREACOMPONENT_COUNTRY_ISO_CODE, countryIsoCode);
			var dv = base.Get(XML_KEY_NAME, "GetAreaComponentByCountry", input);
			return dv.Cast<DataRowView>().Select(drv => new GlobalShippingAreaComponentModel(drv)).ToArray();
		}

		/// <summary>
		/// 送料登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int RegisterGlobalShippingPostage(GlobalShippingPostageModel model)
		{
			return base.Exec(XML_KEY_NAME, "RegisterGlobalShippingPostage", model.DataSource);
		}

		/// <summary>
		/// 送料削除
		/// </summary>
		/// <param name="seq">シーケンス</param>
		/// <returns>削除件数</returns>
		internal int DeleteGlobalShippingPostage(int seq)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SEQ, seq);
			return base.Exec(XML_KEY_NAME, "DeleteGlobalShippingPostage", input);
		}

		/// <summary>
		/// 送料変更
		/// </summary>
		/// <param name="seq">シーケンス</param>
		/// <param name="postage">送料</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>変更件数</returns>
		internal int ChangeGlobalShippingPostage(int seq, decimal postage, string lastChanged)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SEQ, seq);
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_GLOBAL_SHIPPING_POSTAGE, postage);
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_LAST_CHANGED, lastChanged);
			return base.Exec(XML_KEY_NAME, "ChangeGlobalShippingPostage", input);
		}

		/// <summary>
		/// 配送種別を指定して送料削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteGlobalShippingPostageByShipping(string shopId, string shippingId)
		{
			var input = new Hashtable();
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID, shopId);
			input.Add(Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID, shippingId);
			return base.Exec(XML_KEY_NAME, "DeleteGlobalShippingPostageByShipping", input);
		}

		/// <summary>
		/// 配送種別と配送会社を指定して送料削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteGlobalShippingPostageByShippingDeliveryCompany(
			string shopId,
			string shippingId,
			string deliveryCompanyId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID, shopId },
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHIPPING_ID, shippingId },
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_DELIVERY_COMPANY_ID, deliveryCompanyId },
			};
			return Exec(XML_KEY_NAME, "DeleteGlobalShippingPostageByShippingDeliveryCompany", input);
		}

		/// <summary>
		/// 配送種別と配送会社を指定してまるっとコピー
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="fromShippingId">コピー元配送種別</param>
		/// <param name="fromDeliveryCompanyId">コピー元配送会社</param>
		/// <param name="toShippingId">コピー先配送種別</param>
		/// <param name="toDeliveryCompanyId">コピー先配送会社</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>結果件数</returns>
		internal int CopyGlobalShippingPostageByShippingDeliveryCompany(
			string shopId,
			string fromShippingId,
			string fromDeliveryCompanyId,
			string toShippingId,
			string toDeliveryCompanyId,
			string lastChanged)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_SHOP_ID, shopId },
				{ "from_shipping_id", fromShippingId },
				{ "to_shipping_id", toShippingId },
				{ "from_delivery_company_id", fromDeliveryCompanyId },
				{ "to_delivery_company_id", toDeliveryCompanyId },
				{ Constants.FIELD_GLOBALSHIPPINGPOSTAGE_LAST_CHANGED, lastChanged },
			};
			return base.Exec(XML_KEY_NAME, "CopyGlobalShippingPostageByShippingDeliveryCompany", input);
		}
	}
}
