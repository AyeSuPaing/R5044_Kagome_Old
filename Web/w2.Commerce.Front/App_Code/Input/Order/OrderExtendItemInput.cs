/*
=========================================================================================================
  Module      : 注文拡張項目 項目ごと Inputクラス(OrderExtendItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.OrderExtendSetting;

/// <summary>
/// 注文拡張項目 項目ごと Inputクラス
/// </summary>
[Serializable]
public class OrderExtendItemInput
{
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public OrderExtendItemInput()
	{
		this.InputValue = string.Empty;
		this.InputText = string.Empty;
		this.OrderExtendFixedPUrchaseNextTakeOver = true;
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">注文拡張項目 設定</param>
	public OrderExtendItemInput(OrderExtendSettingModel model)
	{
		this.SettingModel = model;
		this.InputValue = string.Empty;
		this.InputText = string.Empty;
	}

	/// <summary>入力内容</summary>
	public string InputValue { get; set; }
	/// <summary>表示内容</summary>
	public string InputText { get; set; }
	/// <summary>次回定期注文に引き継ぐかどうか</summary>
	public bool OrderExtendFixedPUrchaseNextTakeOver { get; set; }
	/// <summary>注文拡張項目 設定</summary>
	public OrderExtendSettingModel SettingModel { get; private set; }
}