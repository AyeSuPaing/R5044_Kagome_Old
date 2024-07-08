/*
=========================================================================================================
  Module      : 定期商品変更設定表示のためのヘルパクラス (FixedPurchaseProductChangeSettingContainer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;

namespace w2.Domain.FixedPurchaseProductChangeSetting.Helper
{
	/// <summary>
	/// 表示用定期商品変更設定クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseProductChangeSettingContainer : FixedPurchaseProductChangeSettingModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseProductChangeSettingContainer()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseProductChangeSettingContainer(DataRowView source)
			: base(source)
		{
		}

		/// <summary>変更元商品コンテナ</summary>
		public FixedPurchaseBeforeChangeItemContainer[] BeforeChangeItemContainers { get; set; }
		/// <summary>変更後商品コンテナ</summary>
		public FixedPurchaseAfterChangeItemContainer[] AfterChangeItemContainers { get; set; }
	}

	/// <summary>
	/// 表示用定期変更元商品クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseBeforeChangeItemContainer : FixedPurchaseBeforeChangeItemModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseBeforeChangeItemContainer()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseBeforeChangeItemContainer(DataRowView source)
			: base(source)
		{
		}

		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource["product_name"]; }
			set { this.DataSource["product_name"] = value; }
		}
		/// <summary>配送種別</summary>
		public string ShippingType
		{
			get { return (string)this.DataSource["shipping_type"]; }
			set { this.DataSource["shipping_type"] = value; }
		}
	}

	/// <summary>
	/// 表示用定期変更元商品クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseAfterChangeItemContainer : FixedPurchaseAfterChangeItemModel
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseAfterChangeItemContainer()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FixedPurchaseAfterChangeItemContainer(DataRowView source)
			: base(source)
		{
		}

		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource["product_name"]; }
			set { this.DataSource["product_name"] = value; }
		}
		/// <summary>配送種別</summary>
		public string ShippingType
		{
			get { return (string)this.DataSource["shipping_type"]; }
			set { this.DataSource["shipping_type"] = value; }
		}
	}
}
