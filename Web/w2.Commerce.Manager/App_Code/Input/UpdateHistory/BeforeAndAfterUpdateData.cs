/*
=========================================================================================================
  Module      : 更新データ（前後）クラス (BeforeAndAfterUpdateData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.Domain.UpdateHistory.Setting;

/// <summary>
/// 更新データ（前後）クラス
/// </summary>
[Serializable]
public class BeforeAndAfterUpdateData
{
	#region コンストラクタ
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BeforeAndAfterUpdateData()
	{
		this.DataSource = new Hashtable();
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public BeforeAndAfterUpdateData(Field field, string before, string after)
		: this()
	{
		this.FieldJName = field.JName;
		this.FieldName = field.Name;
		this.Before = before;
		this.After = after;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// 更新データ（前後）が異なっているか？
	/// </summary>
	/// <returns>更新データ（前後）が異なっている：true、異なっていない：false</returns>
	public bool IsDifferent()
	{
		// 最終ログイン日時は更新の度に更新されるため除外とする
		if (this.FieldName == Constants.FIELD_USER_DATE_LAST_LOGGEDIN) return false;

		return (this.Before != this.After);
	}
	#endregion

	#region プロパティ
	/// <summary>ソース</summary>
	public Hashtable DataSource { get; set; }
	/// <summary>項目名（論理名）</summary>
	public string FieldJName
	{
		get { return (string)this.DataSource["FieldJName"]; }
		set { this.DataSource["FieldJName"] = value; }
	}
	/// <summary>項目名（物理名）</summary>
	public string FieldName
	{
		get { return (string)this.DataSource["FieldName"]; }
		set { this.DataSource["FieldName"] = value; }
	}
	/// <summary>前データ</summary>
	public string Before
	{
		get { return (string)this.DataSource["Before"]; }
		set { this.DataSource["Before"] = value; }
	}
	/// <summary>後データ</summary>
	public string After
	{
		get { return (string)this.DataSource["After"]; }
		set { this.DataSource["After"] = value; }
	}
	#endregion
}