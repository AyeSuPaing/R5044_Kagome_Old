/*
=========================================================================================================
  Module      : 分析ユーティリティモジュール(AnalysisUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Xml;

public class AnalysisUtility
{
	/// <summary>
	/// 割合取得(文字列)
	/// </summary>
	/// <param name="objCurrent">対象データ</param>
	/// <param name="objTarget">比較データ</param>
	/// <param name="iDecimalPoints">小数点以下の桁数</param>
	/// <returns>比較データ÷比較対象データの割合（百分率)、∞の場合は null</returns>
	public static string GetRateString(object objCurrent, object objTarget, int iDecimalPoints)
	{
		string strResult = "";

		try
		{
			double dCurrent = double.Parse(objCurrent.ToString());
			double dTarget = double.Parse(objTarget.ToString());

			if (dTarget == 0)
			{
				if (dCurrent == 0)
				{
					strResult = RegulateDecimalPoint(0,iDecimalPoints).ToString();
				}
				else
				{
					// ∞
					strResult = null;
				}
			}
			else
			{
				double dIncRate = GetRate(dCurrent, dTarget);
				strResult = RegulateDecimalPoint(dIncRate, iDecimalPoints);
			}
		}
		catch
		{
			// 数値変換エラー（空文字が返る）
		}

		return strResult; 
	}

	/// <summary>
	/// 割合取得(double型)
	/// </summary>
	/// <param name="dCurrent">対象データ</param>
	/// <param name="objTarget">比較データ(0以外)</param>
	/// <param name="iDecimalPoints">小数点以下の桁数</param>
	/// <returns>比較データ÷比較対象データの割合（百分率)</returns>
	public static double GetRate(double dCurrent, double dTarget)
	{
		double dResult = 0;

		try
		{
			dResult = ((dCurrent * 100) / dTarget);
		}
		catch
		{
			// 0割（0が返る）
		}

		return dResult; 
	}

	/// <summary>
	/// 上昇率取得(double型)
	/// </summary>
	/// <param name="objCurrent">比較データ</param>
	/// <param name="objTarget">比較対象データ(0以外)</param>
	/// <returns>比較対象データ→比較データの上昇率（百分率)、∞の場合は「-」を返す。（</returns>
	public static double GetIncreasingRate(double dCurrent, double dTarget)
	{
		double dResult = 0;

		try
		{
			dResult = ((dCurrent - dTarget) * 100) / dTarget;
		}
		catch
		{
			// 0割など（0が返る）
		}

		return dResult; 
	}

	/// <summary>
	/// 小数点桁揃え
	/// </summary>
	/// <param name="dCurrent">対象データ</param>
	/// <param name="iDecimalPoints">小数点以下の桁数</param>
	/// <returns>小数点桁揃えされた文字列</returns>
	public static string RegulateDecimalPoint(object oValue, int iDecimalPoints)
	{
		string strResult = null;
		double dUnderDecimalPoint = Math.Pow(10, iDecimalPoints);	// 小数点以下の桁数

		try
		{
			double dValue = double.Parse( oValue.ToString() );
			strResult = (((int)((dValue * dUnderDecimalPoint) + 0.5)) / dUnderDecimalPoint).ToString();
		}
		catch
		{
			// 0割（0が返る）
		}

		return strResult; 
	}
}

