/*
=========================================================================================================
  Module      : 為替レートマスタモデル (ExchangeRateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ExchangeRate
{
	/// <summary>
	/// 為替レートマスタモデル
	/// </summary>
	[Serializable]
	public partial class ExchangeRateModel : ModelBase<ExchangeRateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ExchangeRateModel()
		{
			this.ExchangeRate = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ExchangeRateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ExchangeRateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
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
		public decimal ExchangeRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_EXCHANGERATE_EXCHANGE_RATE]; }
			set { this.DataSource[Constants.FIELD_EXCHANGERATE_EXCHANGE_RATE] = value; }
		}
		#endregion
	}
}
