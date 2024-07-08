/*
=========================================================================================================
  Module      : つくーるAPI連携データインポート用基底クラス (UreruAdImportBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Sql;
using w2.Commerce.Batch.ExternalOrderImport.Entity;

namespace w2.Commerce.Batch.ExternalOrderImport.Import.UreruAd
{
	/// <summary>
	/// つくーるAPI連携データインポート用基底クラス
	/// </summary>
	public abstract class UreruAdImportBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		protected UreruAdImportBase(UreruAdResponseDataItem responseData, SqlAccessor accessor)
		{
			this.ResponseData = responseData;
			this.Accessor = accessor;
		}

		/// <summary>
		/// 登録・更新
		/// </summary>
		public abstract void Import();

		#region プロパティ
		/// <summary>SQLアクセサ</summary>
		protected SqlAccessor Accessor { get; set; }
		/// <summary>レスポンスデータ</summary>
		public UreruAdResponseDataItem ResponseData { get; set; }
		#endregion
	}
}
