/*
=========================================================================================================
  Module      : モール出品設定マスタモデル (MallExhibitsConfigModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.MallExhibitsConfig
{
	/// <summary>
	/// モール出品設定マスタモデル
	/// </summary>
	public class MallExhibitsConfigModel : ModelBase<MallExhibitsConfigModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MallExhibitsConfigModel()
		{
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.ExhibitsFlg1 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg2 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg3 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg4 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg5 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg6 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg7 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg8 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg9 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg10 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg11 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg12 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg13 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg14 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg15 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg16 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg17 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg18 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg19 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.ExhibitsFlg20 = Constants.FLG_MALLEXHIBITSCONFIG_EXHIBITS_FLG_OFF;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MallExhibitsConfigModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public MallExhibitsConfigModel(Hashtable source)
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID] = value; }
		}
		/// <summary>出品FLG1</summary>
		public string ExhibitsFlg1
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG1] = value; }
		}
		/// <summary>出品FLG2</summary>
		public string ExhibitsFlg2
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG2] = value; }
		}
		/// <summary>出品FLG3</summary>
		public string ExhibitsFlg3
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG3] = value; }
		}
		/// <summary>出品FLG4</summary>
		public string ExhibitsFlg4
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG4] = value; }
		}
		/// <summary>出品FLG5</summary>
		public string ExhibitsFlg5
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG5] = value; }
		}
		/// <summary>出品FLG6</summary>
		public string ExhibitsFlg6
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG6] = value; }
		}
		/// <summary>出品FLG7</summary>
		public string ExhibitsFlg7
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG7] = value; }
		}
		/// <summary>出品FLG8</summary>
		public string ExhibitsFlg8
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG8] = value; }
		}
		/// <summary>出品FLG9</summary>
		public string ExhibitsFlg9
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG9] = value; }
		}
		/// <summary>出品FLG10</summary>
		public string ExhibitsFlg10
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG10] = value; }
		}
		/// <summary>出品FLG11</summary>
		public string ExhibitsFlg11
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG11] = value; }
		}
		/// <summary>出品FLG12</summary>
		public string ExhibitsFlg12
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG12] = value; }
		}
		/// <summary>出品FLG13</summary>
		public string ExhibitsFlg13
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG13] = value; }
		}
		/// <summary>出品FLG14</summary>
		public string ExhibitsFlg14
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG14] = value; }
		}
		/// <summary>出品FLG15</summary>
		public string ExhibitsFlg15
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG15] = value; }
		}
		/// <summary>出品FLG16</summary>
		public string ExhibitsFlg16
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG16] = value; }
		}
		/// <summary>出品FLG17</summary>
		public string ExhibitsFlg17
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG17] = value; }
		}
		/// <summary>出品FLG18</summary>
		public string ExhibitsFlg18
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG18] = value; }
		}
		/// <summary>出品FLG19</summary>
		public string ExhibitsFlg19
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG19] = value; }
		}
		/// <summary>出品FLG20</summary>
		public string ExhibitsFlg20
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_EXHIBITS_FLG20] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_MALLEXHIBITSCONFIG_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
