/*
=========================================================================================================
  Module      : CSVマップ名属性クラス (CsvMapName.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Util.Attribute
{
	/// <summary>
	/// CSVマップ名属性クラス
	/// </summary>
	public class CsvMapName : System.Attribute
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="columnIndex">列番号</param>
		/// <param name="mapName">マップ名</param>
		public CsvMapName(int columnIndex, string mapName)
		{
			this.ColumnIndex = columnIndex;
			this.MapName = mapName;
		}

		#region プロパティ
		/// <summary>列番号</summary>
		public int ColumnIndex { get; set; }
		/// <summary>マップ名</summary>
		public string MapName { get; set; }
		#endregion
	}
}
