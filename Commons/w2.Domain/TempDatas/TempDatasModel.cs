/*
=========================================================================================================
  Module      : テンポラリデータモデル (TempDatasModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.TempDatas
{
	/// <summary>
	/// テンポラリデータモデル
	/// </summary>
	[Serializable]
	public partial class TempDatasModel : ModelBase<TempDatasModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TempDatasModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TempDatasModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public TempDatasModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>テンポラリタイプ</summary>
		public string TempType
		{
			get { return (string)this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_TYPE]; }
			set { this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_TYPE] = value; }
		}
		/// <summary>テンポラリキー</summary>
		public string TempKey
		{
			get { return (string)this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_KEY]; }
			set { this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_KEY] = value; }
		}
		/// <summary>テンポラリデータ</summary>
		public byte[] TempData
		{
			get { return (byte[])this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_DATA]; }
			set { this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_DATA] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_TEMPDATAS_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_TEMPDATAS_DATE_CREATED] = value; }
		}
		#endregion
	}
}
