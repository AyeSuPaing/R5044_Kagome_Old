/*
=========================================================================================================
  Module      : 配送種別情報基底ページ(ShopShippingPage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Web;
using w2.Domain.DeliveryCompany;
using w2.Domain.GlobalShipping;
using w2.Domain.ShopShipping;

/// <summary>
/// 配送種別情報基底ページ
/// </summary>
public class ShopShippingPage : BasePage
{
	/// <summary>配送サービスごとの配送料地帯の定義 </summary>
	protected const string DELIVERY_ZONE_PRICES = "delivery_zone_prices";
	/// <summary>The shop shipping zone zip Taiwan key</summary>
	protected const string CONST_KEY_SHOPSHIPPINGZONE_ZIP_TW = Constants.FIELD_SHOPSHIPPINGZONE_ZIP + "_tw";
	/// <summary>The shop shipping zone shipping zone no Taiwan key</summary>
	protected const string CONST_KEY_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO_TW = Constants.FIELD_SHOPSHIPPINGZONE_SHIPPING_ZONE_NO + "_tw";

	/// <summary>
	/// 配送種別情報系ページURL作成
	/// </summary>
	/// <param name="pageManager">ページ</param>
	/// <param name="actionStatus">アクション</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>Url</returns>
	protected string CreateShippingPageUrl(string pageManager, string actionStatus = null, string shippingId = null)
	{
		var creator = new UrlCreator(Constants.PATH_ROOT + pageManager);
		// アクションステータスパラメータ付与
		if (string.IsNullOrEmpty(actionStatus) == false)
		{
			creator.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);
		}
		// 配送種別IDパラメータ付与
		if (string.IsNullOrEmpty(shippingId) == false)
		{
			creator.AddParam(Constants.REQUEST_KEY_SHIPPING_ID, shippingId);
		}
		var url = creator.CreateUrl();
		return url;
	}

	/// <summary>
	/// 配送会社名取得
	/// </summary>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>配送会社名</returns>
	protected string GetDeliveryCompanyName(string deliveryCompanyId)
	{
		var info = this.DeliveryCompanyList.FirstOrDefault(item => (item.DeliveryCompanyId == deliveryCompanyId));
		return (info != null) ? info.DeliveryCompanyName : string.Empty;
	}

	/// <summary>
	/// 配送会社に紐づく配送料地帯情報取得
	/// </summary>
	/// <param name="shippingZones">配送種別に紐づく配送料地帯情報</param>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <returns>配送種別と配送会社に紐づく配送料地帯情報</returns>
	protected ShopShippingZoneModel[] GetShippingZonesByDeliveryCompany(
		ShopShippingZoneModel[] shippingZones,
		string deliveryCompanyId)
	{
		var result = shippingZones.Where(zone => (zone.DeliveryCompanyId == deliveryCompanyId)).ToArray();
		return result;
	}

	/// <summary>
	/// 配送サービスに紐づく海外配送料情報取得
	/// </summary>
	/// <param name="globalPostages">配送種別に紐づく配送料地帯情報</param>
	/// <param name="deliveryCompanyId">配送サービスID</param>
	/// <returns>配送種別と配送会社に紐づく海外配送料情報</returns>
	protected GlobalShippingPostageModel[] GetGlobalShippingPostageByDeliveryCompany(
		GlobalShippingPostageModel[] globalPostages,
		string deliveryCompanyId)
	{
		var result = globalPostages.Where(p => (p.DeliveryCompanyId == deliveryCompanyId)).ToArray();
		return result;
	}

	/// <summary>
	/// 配送サービスに紐づくエラーメッセージ取得
	/// </summary>
	/// <param name="errorMessages">エラーメッセージ</param>
	/// <param name="deliveryCompanyId">配送サービスID</param>
	/// <returns>配送サービスに紐づくエラーメッセージ</returns>
	protected string GetErrorMsgByDeliveryCompany(
		List<KeyValuePair<string, string>> errorMessages,
		string deliveryCompanyId)
	{
		var result = string.Join(
			"",
			errorMessages
				.Where(p => (p.Key == deliveryCompanyId))
				.Select(p => p.Value).ToArray());
		return result;
	}

	/// <summary>
	/// デフォルート値での配送料地帯情報作成
	/// </summary>
	/// <param name="shippingId">配送種別ID</param>
	/// <param name="deliveryCompanyId">配送会社ID</param>
	/// <param name="zoneNo">地帯番号</param>
	/// <param name="zoneName">地帯名</param>
	/// <param name="zip">郵便番号</param>
	/// <returns>デフォルート値での配送料地帯情報</returns>
	protected ShopShippingZoneInput CreateShippingZonePriceDefault(
		string shippingId,
		string deliveryCompanyId,
		string zoneNo,
		string zoneName,
		string zip)
	{
		var input = new ShopShippingZoneInput
		{
			ShopId = this.LoginOperatorShopId,
			ShippingId = shippingId,
			DeliveryCompanyId = deliveryCompanyId,
			ShippingZoneNo = zoneNo,
			ShippingZoneName = zoneName,
			Zip = zip,
			SizeMailShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeXxsShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeXsShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeSShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeMShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeLShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeXlShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			SizeXxlShippingPrice = Constants.DEFAULT_SHIPPING_PRICE,
			LastChanged = this.LoginOperatorName,
		};
		return input;
	}

	/// <summary>
	/// 新規の配送料マスタ作成（デフォールト値）
	/// </summary>
	/// <param name="deliveryCompanies">配送種別に紐づく配送会社情報</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>配送料マスタ</returns>
	protected ShippingDeliveryPostageModel[] CreateNewShippingDeliveryPostages(
		ShopShippingCompanyModel[] deliveryCompanies,
		string shippingId)
	{
		var postages = deliveryCompanies.Select(
			dc =>
			{
				var postage = new ShippingDeliveryPostageModel
				{
					ShopId = this.LoginOperatorShopId,
					ShippingId = shippingId,
					DeliveryCompanyId = dc.DeliveryCompanyId,
					LastChanged = this.LoginOperatorName,
				};
				return postage;
			}).OrderBy(p => p.DeliveryCompanyId).ToArray();
		return postages;
	}

	/// <summary>
	/// 配送料マスタ作成
	/// </summary>
	/// <param name="shippingPostages">既存の配送料マスタ</param>
	/// <param name="afterDeliveryCompanies">変更後の配送会社情報</param>
	/// <param name="beforeDeliveryCompanies">変更前の配送会社情報</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns>配送料マスタ</returns>
	protected ShippingDeliveryPostageModel[] CreateShippingDeliveryPostages(
		ShippingDeliveryPostageModel[] shippingPostages,
		ShopShippingCompanyModel[] afterDeliveryCompanies,
		ShopShippingCompanyModel[] beforeDeliveryCompanies,
		string shippingId)
	{
		if ((shippingPostages == null) || (shippingPostages.Length == 0))
		{
			var result = CreateNewShippingDeliveryPostages(afterDeliveryCompanies, shippingId);
			return result;
		}

		var postages = new List<ShippingDeliveryPostageModel>();
		// 既存の配送料情報を取得
		postages.AddRange(shippingPostages
			.Where(p => afterDeliveryCompanies.Any(dc => (dc.DeliveryCompanyId == p.DeliveryCompanyId))));
		postages.ForEach(
			postage =>
			{
				postage.ShippingId = shippingId;
				postage.LastChanged = this.LoginOperatorName;
			});

		// 変更後の配送会社の差分に応じて、デフォルート配送料情報を設定
		var deliveryCompanyDiffList = afterDeliveryCompanies.Select(dc => dc.DeliveryCompanyId).ToArray()
			.Except(beforeDeliveryCompanies.Select(dc => dc.DeliveryCompanyId).ToArray())
			.ToArray();
		foreach (var companyId in deliveryCompanyDiffList)
		{
			if(postages.Any() && postages.Any(p => (p.DeliveryCompanyId == companyId))) continue;

			var postage = new ShippingDeliveryPostageModel
			{
				ShopId = this.LoginOperatorShopId,
				ShippingId = shippingId,
				DeliveryCompanyId = companyId,
				LastChanged = this.LoginOperatorName,
			};
			postages.Add(postage);
		}

		var sortedPostages = postages.OrderBy(model => model.DeliveryCompanyId).ToArray();
		return sortedPostages;
	}

	/// <summary>
	/// 新規の配送料地帯情報作成（デフォールト値）
	/// </summary>
	/// <param name="deliveryCompanies">選択した配送会社情報</param>
	/// <param name="shippingId">配送種別</param>
	/// <returns>配送料地帯情報</returns>
	protected ShopShippingZoneModel[] CreateNewShippingZones(
		ShopShippingCompanyModel[] deliveryCompanies,
		string shippingId)
	{
		var shippingZones = new List<ShopShippingZoneModel>();
		foreach (var dc in deliveryCompanies)
		{
			for (var idx = 0; idx < this.PrefecturesList.Length; idx++)
			{
				var input = CreateShippingZonePriceDefault(
					shippingId,
					dc.DeliveryCompanyId,
					(idx + 1).ToString(),
					this.PrefecturesList[idx],
					string.Empty);
				shippingZones.Add(input.CreateModel());
			}
		}

		var sortedZones = shippingZones
			.OrderBy(model => model.DeliveryCompanyId)
			.ThenBy(model => model.ShippingZoneNo)
			.ToArray();
		return sortedZones;
	}

	/// <summary>
	/// 配送料地帯情報作成
	/// </summary>
	/// <param name="beforeShippingZones">既存の配送料地帯情報</param>
	/// <param name="afterDeliveryCompanies">変更後の配送会社情報</param>
	/// <param name="beforeDeliveryCompanies">変更前の配送会社情報</param>
	/// <param name="shippingId">配送種別ID</param>
	/// <returns></returns>
	protected ShopShippingZoneModel[] CreateShippingZones(
		ShopShippingZoneModel[] beforeShippingZones,
		ShopShippingCompanyModel[] afterDeliveryCompanies,
		ShopShippingCompanyModel[] beforeDeliveryCompanies,
		string shippingId)
	{
		if ((beforeShippingZones == null) || (beforeShippingZones.Length == 0))
		{
			var result = CreateNewShippingZones(afterDeliveryCompanies, shippingId);
			return result;
		}

		var shippingZones = new List<ShopShippingZoneModel>();

		// 既存の配送料地帯情報を取得
		shippingZones.AddRange(beforeShippingZones
			.Where(zone => afterDeliveryCompanies.Any(dc => (dc.DeliveryCompanyId == zone.DeliveryCompanyId)))
			.ToArray());
		shippingZones.ForEach(
			model =>
			{
				model.ShippingId = shippingId;
				model.LastChanged = this.LoginOperatorName;
			});

		// 変更後の配送会社の差分に応じて、デフォルート配送料地帯情報を設定
		var deliveryCompanyDiffList = afterDeliveryCompanies.Select(dc => dc.DeliveryCompanyId).ToArray()
			.Except(beforeDeliveryCompanies.Select(dc => dc.DeliveryCompanyId).ToArray())
			.ToArray();
		var zoneList = beforeShippingZones.Where(
			zone => (zone.DeliveryCompanyId == beforeShippingZones.First().DeliveryCompanyId))
			.ToList();
		foreach (var companyId in deliveryCompanyDiffList)
		{
			if (shippingZones.Any() && shippingZones.Any(m => (m.DeliveryCompanyId == companyId))) continue;

			foreach (var zone in zoneList)
			{
				var zoneInput = CreateShippingZonePriceDefault(
					shippingId,
					companyId,
					zone.ShippingZoneNo.ToString(),
					zone.ShippingZoneName,
					zone.Zip);
				shippingZones.Add(zoneInput.CreateModel());
			}
		}

		var sortedShippingZones = shippingZones
			.OrderBy(zone => zone.DeliveryCompanyId)
			.ThenBy(zone => zone.ShippingZoneNo)
			.ToArray();
		return sortedShippingZones;
	}

	/// <summary>
	/// 重複なしの配送会社情報取得
	/// </summary>
	/// <param name="companies">配送会社情報配列</param>
	/// <returns>重複なしの配送会社情報配列</returns>
	protected ShopShippingCompanyModel[] GetDistinctCompany(ShopShippingCompanyModel[] companies)
	{
		if ((companies == null) || (companies.Length == 0)) return new ShopShippingCompanyModel[0];
		var distinctModels = companies.GroupBy(c => c.DeliveryCompanyId)
			.Select(grp => grp.First())
			.ToArray();
		return distinctModels;
	}

	#region プロパティ
	/// <summary>配送会社リスト</summary>
	protected DeliveryCompanyModel[] DeliveryCompanyList
	{
		get
		{
			if (m_deliveryComapnyList == null)
			{
				var service = new DeliveryCompanyService();
				m_deliveryComapnyList = service.GetAll();
			}
			return m_deliveryComapnyList;
		}
	}
	private DeliveryCompanyModel[] m_deliveryComapnyList;
	/// <summary>詳細アクションか</summary>
	protected bool IsActionDetail
	{
		get { return this.ActionStatus == Constants.ACTION_STATUS_DETAIL; }
	}
	/// <summary>新規登録アクションか</summary>
	protected bool IsActionInsert
	{
		get { return this.ActionStatus == Constants.ACTION_STATUS_INSERT; }
	}
	/// <summary>コピーして新規登録アクションか</summary>
	protected bool IsActionCopyInsert
	{
		get { return this.ActionStatus == Constants.ACTION_STATUS_COPY_INSERT; }
	}
	/// <summary>編集アクションか</summary>
	protected bool IsActionUpdate
	{
		get { return this.ActionStatus == Constants.ACTION_STATUS_UPDATE; }
	}
	/// <summary>セッションに保持する配送種別情報</summary>
	protected Hashtable ShippingInfoInSession
	{
		get { return (Hashtable)Session[Constants.SESSIONPARAM_KEY_SHIPPING_INFO]; }
		set { Session[Constants.SESSIONPARAM_KEY_SHIPPING_INFO] = value; }
	}
	/// <summary>ViewStateに保持する配送種別情報</summary>
	protected Hashtable ShippingInfoInViewState
	{
		get { return (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_SHIPPING_INFO]; }
		set { ViewState[Constants.SESSIONPARAM_KEY_SHIPPING_INFO] = value; }
	}
	/// <summary>セッションに保持する配送料地帯情報</summary>
	protected Hashtable ShippingZoneInSession
	{
		get { return (Hashtable)Session[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO]; }
		set { Session[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO] = value; }
	}
	/// <summary>ViewStateに保持する配送料地帯情報</summary>
	protected Hashtable ShippingZoneInViewState
	{
		get { return (Hashtable)ViewState[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO]; }
		set { ViewState[Constants.SESSIONPARAM_KEY_SHIPPINGZONE_INFO] = value; }
	}
	/// <summary>元の配送種別ID</summary>
	protected string OriginShippingId
	{
		get { return (string)Session[Constants.REQUEST_KEY_SHIPPING_ID]; }
		set { Session[Constants.REQUEST_KEY_SHIPPING_ID] = value; }
	}
	#endregion

	/// <summary>
	/// 配送料地帯情報
	/// </summary>
	[Serializable]
	public class ShippingZonePrices
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShippingZonePrices()
		{
			this.DeliveryZonePriceList = new List<ShopShippingZoneModel>();
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>配送種別ID</summary>
		public string ShippingId { get; set; }
		/// <summary>配送種別名</summary>
		public string ShopShippingName { get; set; }
		/// <summary>配送地帯区分</summary>
		public int ShippingZoneNo { get; set; }
		/// <summary>地帯名</summary>
		public string ShippingZoneName { get; set; }
		/// <summary>郵便番号</summary>
		public string Zip { get; set; }
		/// <summary>配送サービスごとの配送料</summary>
		public List<ShopShippingZoneModel> DeliveryZonePriceList { get; set; }
	}
}
