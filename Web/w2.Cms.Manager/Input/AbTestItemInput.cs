/*
=========================================================================================================
  Module      : ABテストアイテム入力クラス (AbTestItemInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Input;
using w2.Database.Common;
using w2.Domain.AbTest;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// ABテストアイテム入力クラス
	/// </summary>
	public class AbTestItemInput : InputBase<AbTestItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AbTestItemInput()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AbTestItemInput(AbTestItemModel model)
			: this()
		{
			this.AbTestId = model.AbTestId;
			this.ItemNo = model.ItemNo;
			this.PageId = model.PageId;
			this.DistributionRate = model.DistributionRate.ToString();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override AbTestItemModel CreateModel()
		{
			var model = new AbTestItemModel
			{
				AbTestId = this.AbTestId,
				ItemNo = this.ItemNo,
				PageId = this.PageId,
				DistributionRate = int.Parse(this.DistributionRate),
			};
			return model;
		}
		#endregion

		#region プロパティ
		/// <summary>ABテストID</summary>
		public string AbTestId
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_AB_TEST_ID]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_AB_TEST_ID] = value; }
		}
		/// <summary>アイテムNO</summary>
		public string ItemNo
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_ITEM_NO] = value; }
		}
		/// <summary>ランディングページID</summary>
		public string PageId
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_PAGE_ID]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_PAGE_ID] = value; }
		}
		/// <summary>振り分け比率</summary>
		public string DistributionRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTESTITEM_DISTRIBUTION_RATE]; }
			set { this.DataSource[Constants.FIELD_ABTESTITEM_DISTRIBUTION_RATE] = value; }
		}
		#endregion
	}
}
