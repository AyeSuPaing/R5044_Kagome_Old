/*
=========================================================================================================
  Module      : 商品コンバータ共通ページ(ProductConverterPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using System.Collections;
using System.Web;

///**************************************************************************************
/// <summary>
/// 商品コンバータ共通ページ
/// </summary>
///**************************************************************************************
public class ProductConverterPage : BasePage
{
	/// <summary>
	/// 商品コンバータ基本設定を１件分取得する
	/// </summary>
	/// <param name="strAdtoId">商品コンバータID</param>
	protected DataView GetProductConverterOnce(string strAdtoId)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MallProductConverter", "GetProductConverterOnce"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MALLPRDCNV_ADTO_ID, strAdtoId);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}
	}

	/// <summary>
	/// 行データハッシュテーブル取得
	/// </summary>
	/// <param name="dvParam">データビュー</param>
	/// <param name="iRow">行番号</param>
	/// <returns>行データ</returns>
	protected Hashtable GetRowInfoHashtable(DataView dvParam, int iRow)
	{
		Hashtable htResult = new Hashtable();

		// データが存在する場合
		if (dvParam.Count != 0)
		{
			DataRow drParam = dvParam[iRow].Row;	// 指定行データ取得
			foreach (DataColumn dcParam in drParam.Table.Columns)
			{
				htResult.Add(dcParam.ColumnName, drParam[dcParam.ColumnName]);
			}
		}

		return htResult;
	}

	/// <summary>
	/// パラメタ取得
	/// </summary>
	/// <param name="hrRequest">パラメタが格納されたHttpRequest</param>
	/// <returns>パラメタが格納されたHashtable</returns>
	protected Hashtable GetParameters(HttpRequest hrRequest)
	{
		Hashtable htResult = new Hashtable();
		bool blParamError = false;

		// ページ番号（ページャ動作時のみもちまわる）
		int iCurrentPageNumber;
		if (hrRequest[Constants.REQUEST_KEY_PAGE_NO] == null)
		{
			iCurrentPageNumber = 1;	// 初期遷移（パラメタなし）のときは1ページ目とする
		}
		else if (int.TryParse(hrRequest[Constants.REQUEST_KEY_PAGE_NO], out iCurrentPageNumber) == false)
		{
			blParamError = true;
		}

		htResult.Add(Constants.REQUEST_KEY_PAGE_NO, iCurrentPageNumber);
		htResult.Add(Constants.ERROR_REQUEST_PRAMETER, blParamError);

		return htResult;
	}
}
