/*
=========================================================================================================
  Module      : 注文拡張項目 Inputクラス(OrderExtendInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using w2.App.Common.DataCacheController;
using w2.App.Common.DataCacheController.CacheData;
using w2.App.Common.Order.Cart;
using w2.App.Common.OrderExtend;
using w2.Domain.OrderExtendSetting;

/// <summary>
/// 注文拡張項目 Inputクラス
/// </summary>
public class OrderExtendInput
{
	/// <summary>
	/// 登録・編集画面
	/// </summary>
	public enum UseType
	{
		Register,
		Modify
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="useType">登録・編集画面</param>
	/// <param name="orderExtend">注文拡張項目 入力内容</param>
	public OrderExtendInput(UseType useType, IReadOnlyDictionary<string, CartOrderExtendItem> orderExtend)
	{
		if ((Constants.ORDER_EXTEND_OPTION_ENABLED == false)
			|| Constants.GIFTORDER_OPTION_WITH_SHORTENING_GIFT_OPTION_ENABLED)
		{
			this.OrderExtendItems = new OrderExtendItemInput[] { };
			return;
		}

		this.ValidaterMaster = new XmlDocument();
		switch (useType)
		{
			case UseType.Register:
				this.ValidaterMaster = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
					.ValidaterMasterForFrontRegister;
				break;

			case UseType.Modify:
				this.ValidaterMaster = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
					.ValidaterMasterForFrontModify;
				break;

			default:
				break;
		}

		this.OrderExtendItems = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
			.SettingModelsForFront.Select(
			m => new OrderExtendItemInput(m)
			{
				InputValue = orderExtend[m.SettingId].Value,
				InputText = OrderExtendCommon.GetValueDisplayName(
					m.InputType,
					m.InputDefault,
					orderExtend[m.SettingId].Value),
				OrderExtendFixedPUrchaseNextTakeOver = orderExtend[m.SettingId].IsFixedPurchaseTakeOverNext,
			}).ToArray();
	}

	/// <summary>
	/// バリデーション
	/// </summary>
	/// <returns>エラー内容</returns>
	public Dictionary<string, string> Validate()
	{
		if (Constants.ORDER_EXTEND_OPTION_ENABLED == false)
		{
			return new Dictionary<string, string>();
		}

		var values = this.OrderExtendItems.ToDictionary(i => i.SettingModel.SettingId, i => i.InputValue);
		var result = Validator.ValidateAndGetErrorContainer(
			OrderExtendSettingCacheData.VALIDATE_NAME,
			values,
			this.ValidaterMaster);
		return result;
	}

	/// <summary>
	/// 注文拡張項目 項目後の入力内容
	/// </summary>
	public OrderExtendItemInput[] OrderExtendItems { get; set; }

	/// <summary>
	/// フォーム作成時点のバリデーション内容
	/// </summary>
	private XmlDocument ValidaterMaster { get; set; }
}