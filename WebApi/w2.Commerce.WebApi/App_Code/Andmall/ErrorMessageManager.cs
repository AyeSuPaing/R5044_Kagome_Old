/*
=========================================================================================================
  Module      : エラーメッセージマネージャー(ErrorMessageManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

/// <summary>
/// エラーメッセージマネージャー
/// </summary>
public class ErrorMessageManager
{
	/// <summary>処理結果コード</summary>
	public enum ResultCode : int
	{
		S0000,
		E0001,
		E0002,
		E0003,
		E0004,
		E0030,
		E0031,
		E9999
	}

	/// <summary>明細処理結果コード</summary>
	public enum DetailsResultCode : int
	{
		S0000,
		E0003,
		E0004,
		E0010,
		E0012,
		E0013,
		E0014,
		E0015,
		E0020
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ErrorMessageManager()
	{
		this.ResultErrorMessages = new Dictionary<ResultCode, string>
		{
			{ResultCode.S0000, ""},
			{ResultCode.E0001, "パラメータが設定されておりません"},
			{ResultCode.E0002, "パラメータが正しくありません"},
			{ResultCode.E0003, "JSONデータのフォーマットが不正です"},
			{ResultCode.E0004, "サイトコードが不正です"},
			{ResultCode.E0030, "在庫引当に失敗しました"},
			{ResultCode.E0031, "引当解放に失敗しました"},
			{ResultCode.E9999, "システム内部エラー発生中です"}
		};

		this.DetailsResultErrorMessages = new Dictionary<DetailsResultCode, string>
		{
			{DetailsResultCode.S0000, ""},
			{DetailsResultCode.E0003, "JSONデータのフォーマットが不正です"},
			{DetailsResultCode.E0004, "サイトコードまたはテナントコードが不正です"},
			{DetailsResultCode.E0010, "ショップ詳細情報が取得できません"},
			{DetailsResultCode.E0012, "倉庫情報が取得できません"},
			{DetailsResultCode.E0013, "商品情報が取得できません"},
			{DetailsResultCode.E0014, "在庫引当可能数情報が取得できません"},
			{DetailsResultCode.E0015, "引当情報の取得に失敗しました"},
			{DetailsResultCode.E0020, "引当可能在庫がありません"},
		};
	}

	/// <summary>
	/// コード名称の取得
	/// </summary>
	/// <param name="resultCode">コード</param>
	/// <returns>コード名称</returns>
	public static string CodeKeyName(object resultCode)
	{
		var result = Enum.GetName(resultCode.GetType(), resultCode);
		return result;
	}

	/// <summary>処理結果エラーメッセージリスト</summary>
	public Dictionary<ResultCode, string> ResultErrorMessages { get; private set; }
	/// <summary>明細処理結果エラーメッセージリスト</summary>
	public Dictionary<DetailsResultCode, string> DetailsResultErrorMessages { get; private set; }
}