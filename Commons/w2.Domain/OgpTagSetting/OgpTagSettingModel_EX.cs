/*
=========================================================================================================
  Module      : OGPタグ設定テーブルモデル (OgpTagSettingModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.OgpTagSetting
{
	/// <summary>
	/// OGPタグ設定テーブルモデル
	/// </summary>
	public partial class OgpTagSettingModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>タイプ</summary>
		public string Type
		{
			get { return (string)this.DataSource["type"]; }
			set { this.DataSource["type"] = value; }
		}
		/// <summary>サイトURL</summary>
		public string SiteUrl
		{
			get { return (string)this.DataSource["site_url"]; }
			set { this.DataSource["site_url"] = value; }
		}
		#endregion
	}
}
