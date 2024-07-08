/*
=========================================================================================================
  Module      : テンポラリデータモデル (TempDatasModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using w2.Common.Extensions;

namespace w2.Domain.TempDatas
{
	/// <summary>
	/// テンポラリデータモデル
	/// </summary>
	public partial class TempDatasModel : ModelBase<TempDatasModel>
	{
		/// <summary>
		/// テンポラリデータデシリアライズ
		/// </summary>
		public void DeserializeTempData()
		{
			using (var stream = new MemoryStream(this.TempData))
			{
				var formatter = new BinaryFormatter();
				this.TempDataDeserialized = formatter.Deserialize(stream);
			}
		}

		#region プロパティ
		/// <summary>デシリアライズ済テンポラリデータ</summary>
		public object TempDataDeserialized
		{
			get { return this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_DATA + "_Deserialized"]; }
			set { this.DataSource[Constants.FIELD_TEMPDATAS_TEMP_DATA + "_Deserialized"] = value; }
		}
		#endregion
	}
}
