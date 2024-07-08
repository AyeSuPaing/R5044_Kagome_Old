/*
=========================================================================================================
  Module      : 為替レート情報入力クラス (ExchangeRateInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text.RegularExpressions;
using w2.App.Common.Input;
using w2.Domain.ExchangeRate;

/// <summary>
/// 為替レート情報入力クラス
/// </summary>
[Serializable]
public class ExchangeRateInput : InputBase<ExchangeRateModel>
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ExchangeRateInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public ExchangeRateInput(ExchangeRateModel model)
		: this()
	{
		this.SrcCurrencyCode = model.SrcCurrencyCode;
		this.DstCurrencyCode = model.DstCurrencyCode;
		this.ExchangeRate = model.ExchangeRate.ToString();
	}
	#endregion

	#region バリデーション
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override ExchangeRateModel CreateModel()
	{
		var model = new ExchangeRateModel
		{
			SrcCurrencyCode = this.SrcCurrencyCode,
			DstCurrencyCode = this.DstCurrencyCode,
			ExchangeRate = (string.IsNullOrEmpty(this.ExchangeRate))
				? 1m
				: decimal.Parse(this.ExchangeRate)
		};
		return model;
	}

	/// <summary>
	/// バリデーション
	/// </summary>
	/// <returns>エラーの際はエラーメッセージを返す</returns>
	public string Validate()
	{
		var currentExchangeRateMessage = string.Format(
			"{0} (縦) - {1} (横):",
			this.SrcCurrencyCode,
			this.DstCurrencyCode);
		var errorMessage = string.Empty;
		errorMessage = Validator.Validate("ExchangeRate", this.DataSource);

		if ((string.IsNullOrEmpty(errorMessage)
			&& (IsValidExchangeRate() == false)))
		{
			errorMessage += WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_EXCHANGE_RATE_EXCHANGE_RATE_NOT_VALID);
		}

		errorMessage = string.IsNullOrEmpty(errorMessage)
			? string.Empty
			: string.Format(
				"{0} {1}",
				currentExchangeRateMessage,
				errorMessage);

		return errorMessage;
	}

	/// <summary>
	/// Is valid exchange rate
	/// </summary>
	/// <returns>True if exchange rate valid, otherwise false</returns>
	public bool IsValidExchangeRate()
	{
		var isValidExchangeRate = Regex.IsMatch(this.ExchangeRate, @"^\d{1,12}(\.\d{1,12})?$");
		return isValidExchangeRate;
	}
	#endregion

	#region プロパティ
	/// <summary>通貨コード（元）</summary>
	public string SrcCurrencyCode
	{
		get { return (string)this.DataSource[Constants.FIELD_EXCHANGERATE_SRC_CURRENCY_CODE]; }
		set { this.DataSource[Constants.FIELD_EXCHANGERATE_SRC_CURRENCY_CODE] = value; }
	}
	/// <summary>通貨コード（先）</summary>
	public string DstCurrencyCode
	{
		get { return (string)this.DataSource[Constants.FIELD_EXCHANGERATE_DST_CURRENCY_CODE]; }
		set { this.DataSource[Constants.FIELD_EXCHANGERATE_DST_CURRENCY_CODE] = value; }
	}
	/// <summary>為替レート</summary>
	public string ExchangeRate
	{
		get { return (string)this.DataSource[Constants.FIELD_EXCHANGERATE_EXCHANGE_RATE]; }
		set { this.DataSource[Constants.FIELD_EXCHANGERATE_EXCHANGE_RATE] = value; }
	}
	#endregion
}