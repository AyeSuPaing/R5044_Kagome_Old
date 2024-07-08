/*
=========================================================================================================
  Module      : 注文拡張項目 Inputクラス(OrderExtendInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.DataCacheController.CacheData;
using w2.App.Common.OrderExtend;

/// <summary>
/// 注文拡張項目 Inputクラス
/// </summary>
[Serializable]
public class OrderExtendInput
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public OrderExtendInput()
	{
		this.OrderExtendItems = new OrderExtendItemInput[] { };
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="orderExtend">注文拡張項目 入力内容</param>
	public OrderExtendInput(
		IReadOnlyDictionary<string, string> orderExtend)
	{
		this.OrderExtendItems = DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData
			.SettingModelsForEcManager.Where(m => m.CanUseEc).Select(m => new OrderExtendItemInput(m)
			{
				InputValue = orderExtend[m.SettingId],
				InputText = OrderExtendCommon.GetValueDisplayName(m.InputType, m.InputDefault, orderExtend[m.SettingId])
			}).ToArray();
	}
	/// <summary>
	/// バリデーション
	/// </summary>
	/// <returns>エラー内容</returns>
	public string Validate()
	{
		var values = this.OrderExtendItems.ToDictionary(i => i.SettingModel.SettingId, i => i.InputValue);
		var result = Validator.Validate(
			OrderExtendSettingCacheData.VALIDATE_NAME,
			values,
			DataCacheControllerFacade.GetOrderExtendSettingCacheController().CacheData.ValidaterMasterForEcManager);
		return result;
	}

	/// <summary>注文拡張項目 項目後の入力内容</summary>
	public OrderExtendItemInput[] OrderExtendItems { get; set; }
}