/*
=========================================================================================================
  Module      : 配送リードタイム入力クラス (DeliveryLeadTimeInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Input;
using w2.Domain.DeliveryLeadTime;

/// <summary>
/// 配送リードタイムマスタ入力クラス
/// </summary>
[Serializable]
public class DeliveryLeadTimeInput : InputBase<DeliveryLeadTimeModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public DeliveryLeadTimeInput()
	{
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public DeliveryLeadTimeInput(DeliveryLeadTimeModel model)
		: this()
	{
		this.ShopId = StringUtility.ToEmpty(model.ShopId);
		this.DeliveryCompanyId = StringUtility.ToEmpty(model.DeliveryCompanyId);
		this.LeadTimeZoneNo = StringUtility.ToEmpty(model.LeadTimeZoneNo);
		this.LeadTimeZoneName = StringUtility.ToEmpty(model.LeadTimeZoneName);
		this.Zip = StringUtility.ToEmpty(model.Zip);
		this.ShippingLeadTime = StringUtility.ToEmpty(model.ShippingLeadTime);
		this.LastChanged = StringUtility.ToEmpty(model.LastChanged);
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override DeliveryLeadTimeModel CreateModel()
	{
		var model = new DeliveryLeadTimeModel
		{
			ShopId = this.ShopId,
			DeliveryCompanyId = this.DeliveryCompanyId,
			LeadTimeZoneNo = int.Parse(this.LeadTimeZoneNo),
			LeadTimeZoneName = this.LeadTimeZoneName,
			Zip = this.Zip,
			ShippingLeadTime = int.Parse(this.ShippingLeadTime),
			LastChanged = this.LastChanged,
		};

		return model;
	}

	/// <summary>
	/// 検証
	/// </summary>
	/// <param name="actionStatus">Action status</param>
	/// <param name="deliveryLeadTimeZoneSpecial">Delivery lead time zone special</param>
	/// <returns>エラーメッセージ</returns>
	public string Validate(string actionStatus, bool deliveryLeadTimeZoneSpecial = false)
	{
		var errorMessage = string.Empty;

		if (deliveryLeadTimeZoneSpecial)
		{
			var inputForCheck = (Hashtable)this.DataSource.Clone();
			if (Constants.TW_COUNTRY_SHIPPING_ENABLE)
			{
				inputForCheck[Constants.FIELD_DELIVERYLEADTIME_ZIP + "_tw"] = this.Zip;
				inputForCheck.Remove(Constants.FIELD_DELIVERYLEADTIME_ZIP);
			}
			errorMessage = Validator.Validate("DeliveryLeadTimeZoneSpecial", inputForCheck);
		}
		else
		{
			switch (actionStatus)
			{
				case Constants.ACTION_STATUS_INSERT:
				case Constants.ACTION_STATUS_COPY_INSERT:
					errorMessage = Validator.Validate("DeliveryLeadTimeRegist", this.DataSource);
					break;

				case Constants.ACTION_STATUS_UPDATE:
					errorMessage = Validator.Validate("DeliveryLeadTimeModify", this.DataSource);
					break;
			}
		}

		return errorMessage;
	}
	#endregion

	#region プロパティ
	/// <summary>店舗ID</summary>
	public string ShopId
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHOP_ID]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHOP_ID] = value; }
	}

	/// <summary>配送会社ID</summary>
	public string DeliveryCompanyId
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DELIVERY_COMPANY_ID] = value; }
	}

	/// <summary>リードタイム地帯区分</summary>
	public string LeadTimeZoneNo
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NO]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NO] = value; }
	}

	/// <summary>地帯名</summary>
	public string LeadTimeZoneName
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NAME]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LEAD_TIME_ZONE_NAME] = value; }
	}

	/// <summary>郵便番号</summary>
	public string Zip
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_ZIP]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_ZIP] = value; }
	}

	/// <summary>追加配送リードタイム</summary>
	public string ShippingLeadTime
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHIPPING_LEAD_TIME]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_SHIPPING_LEAD_TIME] = value; }
	}

	/// <summary>作成日</summary>
	public string DateCreated
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CREATED] = value; }
	}

	/// <summary>更新日</summary>
	public string DateChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_DATE_CHANGED] = value; }
	}

	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_DELIVERYLEADTIME_LAST_CHANGED] = value; }
	}
	#endregion
}