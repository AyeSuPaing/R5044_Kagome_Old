/*
=========================================================================================================
  Module      : モール出品設定ファイル取込インタフェース(IImportExhibits.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.IO;

public interface IImportExhibits
{
	/// <summary>
	/// インポート
	/// </summary>
	bool ImportExhibits(StreamReader sr, string strShopId, string strOpertorName);

	/// プロパティ
	/// <summary>エラーメッセージ</summary>
	string ErrorMessage { get; }
	/// <summary>更新件数</summary>
	int UpdatedCount { get; }
	/// <summary>処理行数</summary>
	int LinesCount { get; }
}