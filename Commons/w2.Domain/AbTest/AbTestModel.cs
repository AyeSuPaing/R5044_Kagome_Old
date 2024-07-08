/*
=========================================================================================================
  Module      : ABテストモデル (AbTestModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.AbTest
{
	/// <summary>
	/// ABテストモデル
	/// </summary>
	[Serializable]
	public partial class AbTestModel : ModelBase<AbTestModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public AbTestModel()
		{
			this.AbTestId = "";
			this.AbTestTitle = "";
			this.PublicStatus = Constants.FLG_ABTEST_PUBLISH_PRIVATE;
			this.PublicEndDatetime = null;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AbTestModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public AbTestModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ABテストID</summary>
		public string AbTestId
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_AB_TEST_ID]; }
			set { this.DataSource[Constants.FIELD_ABTEST_AB_TEST_ID] = value; }
		}
		/// <summary>ABテストタイトル</summary>
		public string AbTestTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_AB_TEST_TITLE]; }
			set { this.DataSource[Constants.FIELD_ABTEST_AB_TEST_TITLE] = value; }
		}
		/// <summary>公開状態</summary>
		public string PublicStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_PUBLIC_STATUS]; }
			set { this.DataSource[Constants.FIELD_ABTEST_PUBLIC_STATUS] = value; }
		}
		/// <summary>公開開始日時</summary>
		public DateTime PublicStartDatetime
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ABTEST_PUBLIC_START_DATETIME]; }
			set { this.DataSource[Constants.FIELD_ABTEST_PUBLIC_START_DATETIME] = value; }
		}
		/// <summary>公開終了日時</summary>
		public DateTime? PublicEndDatetime
		{
			get
			{
				if (this.DataSource[Constants.FIELD_ABTEST_PUBLIC_END_DATETIME] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_ABTEST_PUBLIC_END_DATETIME];
			}
			set { this.DataSource[Constants.FIELD_ABTEST_PUBLIC_END_DATETIME] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ABTEST_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ABTEST_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ABTEST_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ABTEST_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ABTEST_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ABTEST_LAST_CHANGED] = value; }
		}
		#endregion
	}
}