/*
=========================================================================================================
  Module      : チャートＸＭＬ出力モジュール(ChartXmlWriter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

public class ChartXmlWriter
{
	public const string XMLELM_BASE = "Chart";
	public const string XMLELM_TYPE = "Type";
	public const string XMLELM_DEFAULTVALUEVIEW = "DefaultValueView";

	// 追加
	public const string XMLELM_TOP_MARGIN = "TopMargin";
	public const string XMLELM_BOTTOM_MARGIN = "BottomMargin";
	public const string XMLELM_LEFT_MARGIN = "LeftMargin";
	public const string XMLELM_RIGHT_MARGIN = "RightMargin";
	public const string XMLELM_HORIZONAL_LABEL = "HorizontalLabel";
	public const string XMLELM_VERTICAL_LABEL = "VerticalLabel";

	public const string XMLELM_YMIN = "YMinValue";
	public const string XMLELM_YMAX = "YMaxValue";
	public const string XMLELM_YSCALE_INT = "YScaleInterval";
	public const string XMLELM_YSCALETEXT_INT = "YScaleTextInterval";
	public const string XMLELM_XFIELD_NAME = "XFieldName";
	public const string XMLELM_YFIELD_NAME = "YFieldName";
	public const string XMLELM_ANIMATION = "Animation";
	public const string XMLELM_ANIMATION_SPEED = "AnimationSpeed";


	public const string XMLELM_DATA = "Data";
	public const string XMLELM_OBJECT = "Object";
	public const string XMLATR_DATA_OBJECT_SCALEMARK = "ScaleMark";
	public const string XMLATR_DATA_OBJECT_SCALETEXT = "ScaleText";
	public const string XMLATR_DATA_OBJECT_VALUE = "Value";
	public const string XMLATR_DATA_OBJECT_TIPTEXT = "TipText";

	public const string VALUE_TYPE_LINE = "line";
	public const string VALUE_TYPE_COLUMN = "column";
	public const string VALUE_TOPMARGIN = "50";
	public const string VALUE_BOTTOMMARGIN = "50";
	public const string VALUE_LEFTMARGIN = "50";
	public const string VALUE_RIGHTMARGIN = "30";
	//public const string VALUE_HORIZONALLABEL = "日付";
	//public const string VALUE_VERTICALLABEL = "人";
	public const string VALUE_ANIMATION = "true";

	/// <summary>横軸見出し</summary>
	public string m_strHorizonalLabel = null;

	/// <summary>縦軸見出し</summary>
	public string m_strVerticalLabel = null;

	/// <summary>マージン</summary>
	public string m_strTopMargin = VALUE_TOPMARGIN;
	public string m_strBottomMargin = VALUE_BOTTOMMARGIN;
	public string m_strLeftMargin = VALUE_LEFTMARGIN;
	public string m_strRightMargin = VALUE_RIGHTMARGIN;

	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="strHorizonalLabel">横軸見出し</param>
	/// <param name="strVerticalLabel">縦軸見出し</param>
	public ChartXmlWriter(string strHorizonalLabel, string strVerticalLabel)
	{
		m_strHorizonalLabel = strHorizonalLabel;
		m_strVerticalLabel = strVerticalLabel;
	}

	/// <summary>
	/// チャートXML作成
	/// </summary>
	/// <param name="alChartData">表示データ</param>
	/// <returns>データXML</returns>
	public string CreateColumnChartXml(ArrayList alChartData, int iSpeed)
	{
		return CreateChartXml(alChartData, iSpeed, VALUE_TYPE_COLUMN);
	}
	/// <summary>
	/// チャートXML作成
	/// </summary>
	/// <param name="alChartData">表示データ</param>
	/// <returns>データXML</returns>
	public string CreateLineChartXml(ArrayList alChartData, int iSpeed)
	{
		return CreateChartXml(alChartData, iSpeed, VALUE_TYPE_LINE);
	}
	/// <summary>
	/// チャートXML作成
	/// </summary>
	/// <param name="alChartData">表示データ</param>
	/// <param name="iSpeed">描画スピード(1/30秒の描画数)</param>
	/// <param name="strChartType">形状</param>
	/// <returns>データXML</returns>
	public string CreateChartXml( ArrayList alChartData, int iSpeed, string strChartType )
	{
		using (StringWriter stringWriter = new StringWriter())
		using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
		{
			xmlWriter.QuoteChar = '\'';

			// インデントON
			xmlWriter.Formatting = System.Xml.Formatting.Indented;

			// 最大値取得・スケール最大値設定
			long lMinValue = 0;
			long lMaxValue = 0;
			foreach (Hashtable htData in alChartData)
			{
				long lValue = long.Parse(htData[XMLATR_DATA_OBJECT_VALUE].ToString());

				lMaxValue = (lValue > lMaxValue) ? lValue : lMaxValue;
				lMinValue = (lValue < lMinValue) ? lValue : lMinValue;
			}
			// 最大値～スケール最大値までの余分
			long lTmpMaxAddValue = (long)((lMaxValue - lMinValue) * 0.25);

			// スケール最大値設定用（ 756 -> 800 としたい場合、「100」）
			// ロジックとしては、値の幅の一桁上の数（10の乗数）÷ 10
			long lTmpDivide = long.Parse("1".PadRight((lMaxValue - lMinValue).ToString().Length + 1, '0')) / 10;

			// スケール最大値を余分を持って設定する
			long lMaxScale = (((lMaxValue + lTmpMaxAddValue) / lTmpDivide) + 1) * lTmpDivide;

			// 下限が0以下の場合は、余分を持ってスケール最小値を設定する
			long lMinScale = (lMinValue < 0) ? ((lMinValue - lTmpMaxAddValue / lTmpDivide) + 1) * lTmpDivide : 0;

			// インターバルが0だと固まるので1にする。
			long lYScaleInterval = lTmpDivide / 2;
			lYScaleInterval = (lYScaleInterval == 0) ? 1 : lYScaleInterval;

			// テキスト間隔はlYScaleIntervalの個数毎に設定
			long lYScaleTextInterval = 2;

			// Chart
			xmlWriter.WriteStartElement(XMLELM_BASE);
			// Type
			xmlWriter.WriteElementString(XMLELM_TYPE, strChartType);
			// DefaultValueView
			xmlWriter.WriteElementString(XMLELM_DEFAULTVALUEVIEW, "false");

			// TopMargin
			xmlWriter.WriteElementString(XMLELM_TOP_MARGIN, m_strTopMargin);
			// BottomMargin
			xmlWriter.WriteElementString(XMLELM_BOTTOM_MARGIN, m_strBottomMargin);
			// LeftMargin
			xmlWriter.WriteElementString(XMLELM_LEFT_MARGIN, m_strLeftMargin);
			// RightMargin
			xmlWriter.WriteElementString(XMLELM_RIGHT_MARGIN, m_strRightMargin);
			// HorizonalLabel
			xmlWriter.WriteElementString(XMLELM_HORIZONAL_LABEL, m_strHorizonalLabel);
			// RightMargin
			xmlWriter.WriteElementString(XMLELM_VERTICAL_LABEL, m_strVerticalLabel);
			// YMinValue
			xmlWriter.WriteElementString(XMLELM_YMIN, lMinScale.ToString());
			// YMaxValue
			xmlWriter.WriteElementString(XMLELM_YMAX, lMaxScale.ToString());
			// YScaleInterval
			xmlWriter.WriteElementString(XMLELM_YSCALE_INT, lYScaleInterval.ToString());
			// YScaleTextInterval
			xmlWriter.WriteElementString(XMLELM_YSCALETEXT_INT, lYScaleTextInterval.ToString());
			// Animation
			xmlWriter.WriteElementString(XMLELM_ANIMATION, VALUE_ANIMATION);
			// AnimationSpeef
			xmlWriter.WriteElementString(XMLELM_ANIMATION_SPEED, iSpeed.ToString());

			// Data
			xmlWriter.WriteStartElement(XMLELM_DATA);
			foreach (Hashtable htData in alChartData)
			{
				// Object(連続可能）
				xmlWriter.WriteStartElement(XMLELM_OBJECT);
				xmlWriter.WriteAttributeString(XMLATR_DATA_OBJECT_SCALEMARK, (htData[XMLATR_DATA_OBJECT_SCALETEXT].ToString() != "") ? "true" : "");
				xmlWriter.WriteAttributeString(XMLATR_DATA_OBJECT_SCALETEXT, htData[XMLATR_DATA_OBJECT_SCALETEXT].ToString());
				xmlWriter.WriteAttributeString(XMLATR_DATA_OBJECT_VALUE, htData[XMLATR_DATA_OBJECT_VALUE].ToString());
				xmlWriter.WriteAttributeString(XMLATR_DATA_OBJECT_TIPTEXT, htData[XMLATR_DATA_OBJECT_TIPTEXT].ToString());
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();

			return stringWriter.ToString();
		}
	}

	/// <summary>
	/// フラッシュマージン指定(背景、グラフ間)
	/// </summary>
	/// <param name="strTopMargin">トップマージン</param>
	/// <param name="strBottomMargin">ボトムマージン</param>
	/// <param name="strLeftMargin"></param>
	/// <param name="strRightMargin"></param>
	public void SetMargin(string strTopMargin, string strBottomMargin, string strLeftMargin, string strRightMargin)
	{
		// マージン設定
		m_strTopMargin = strTopMargin;
		m_strTopMargin = strBottomMargin;
		m_strLeftMargin = strLeftMargin;
		m_strRightMargin = strRightMargin;
	}
}

