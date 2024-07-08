/*
=========================================================================================================
  Module      : Score Axis Input (ScoreAxisInput.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common;

/// <summary>
/// Score Axis Input
/// </summary>
[Serializable]
public class ScoreAxisInput : ModelBase<ScoreAxisInput>
{
	#region +Constructor
	/// <summary>
	/// Default constructor
	/// </summary>
	public ScoreAxisInput()
	{
		this.AxisAdditional1 = 0;
		this.AxisAdditional2 = 0;
		this.AxisAdditional3 = 0;
		this.AxisAdditional4 = 0;
		this.AxisAdditional5 = 0;
		this.AxisAdditional6 = 0;
		this.AxisAdditional7 = 0;
		this.AxisAdditional8 = 0;
		this.AxisAdditional9 = 0;
		this.AxisAdditional10 = 0;
		this.AxisAdditional11 = 0;
		this.AxisAdditional12 = 0;
		this.AxisAdditional13 = 0;
		this.AxisAdditional14 = 0;
		this.AxisAdditional15 = 0;
	}

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="source">Source</param>
	public ScoreAxisInput(Hashtable source)
		: this()
	{
		this.DataSource = source;
	}
	#endregion

	#region +Method
	/// <summary>
	/// Add score
	/// </summary>
	/// <param name="scores">Score array</param>
	public void AddScores(ScoringSaleQuestionChoiceInput[] scores)
	{
		foreach (var score in scores)
		{
			this.AxisAdditional1 += score.AxisAdditional1;
			this.AxisAdditional2 += score.AxisAdditional2;
			this.AxisAdditional3 += score.AxisAdditional3;
			this.AxisAdditional4 += score.AxisAdditional4;
			this.AxisAdditional5 += score.AxisAdditional5;
			this.AxisAdditional6 += score.AxisAdditional6;
			this.AxisAdditional7 += score.AxisAdditional7;
			this.AxisAdditional8 += score.AxisAdditional8;
			this.AxisAdditional9 += score.AxisAdditional9;
			this.AxisAdditional10 += score.AxisAdditional10;
			this.AxisAdditional11 += score.AxisAdditional11;
			this.AxisAdditional12 += score.AxisAdditional12;
			this.AxisAdditional13 += score.AxisAdditional13;
			this.AxisAdditional14 += score.AxisAdditional14;
			this.AxisAdditional15 += score.AxisAdditional15;
		}
	}

	/// <summary>
	/// Get axis additional key
	/// </summary>
	/// <param name="number">Number</param>
	/// <returns>Axis additional key</returns>
	public string GetAxisAdditionalKey(int number)
	{
		return string.Format("{0}{1}",
			Constants.FIELD_SCORINGSALERESULTCONDITION_SCORE_AXIS_AXIS_NO,
			number);
	}
	#endregion

	#region +Properties
	/// <summary>カラム加算値１</summary>
	public int AxisAdditional1
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(1)]; }
		set { this.DataSource[GetAxisAdditionalKey(1)] = value; }
	}
	/// <summary>カラム加算値２</summary>
	public int AxisAdditional2
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(2)]; }
		set { this.DataSource[GetAxisAdditionalKey(2)] = value; }
	}
	/// <summary>カラム加算値３</summary>
	public int AxisAdditional3
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(3)]; }
		set { this.DataSource[GetAxisAdditionalKey(3)] = value; }
	}
	/// <summary>カラム加算値４</summary>
	public int AxisAdditional4
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(4)]; }
		set { this.DataSource[GetAxisAdditionalKey(4)] = value; }
	}
	/// <summary>カラム加算値５</summary>
	public int AxisAdditional5
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(5)]; }
		set { this.DataSource[GetAxisAdditionalKey(5)] = value; }
	}
	/// <summary>カラム加算値６</summary>
	public int AxisAdditional6
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(6)]; }
		set { this.DataSource[GetAxisAdditionalKey(6)] = value; }
	}
	/// <summary>カラム加算値７</summary>
	public int AxisAdditional7
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(7)]; }
		set { this.DataSource[GetAxisAdditionalKey(7)] = value; }
	}
	/// <summary>カラム加算値８</summary>
	public int AxisAdditional8
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(8)]; }
		set { this.DataSource[GetAxisAdditionalKey(8)] = value; }
	}
	/// <summary>カラム加算値９</summary>
	public int AxisAdditional9
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(9)]; }
		set { this.DataSource[GetAxisAdditionalKey(9)] = value; }
	}
	/// <summary>カラム加算値１０</summary>
	public int AxisAdditional10
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(10)]; }
		set { this.DataSource[GetAxisAdditionalKey(10)] = value; }
	}
	/// <summary>カラム加算値１１</summary>
	public int AxisAdditional11
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(11)]; }
		set { this.DataSource[GetAxisAdditionalKey(11)] = value; }
	}
	/// <summary>カラム加算値１２</summary>
	public int AxisAdditional12
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(12)]; }
		set { this.DataSource[GetAxisAdditionalKey(12)] = value; }
	}
	/// <summary>カラム加算値１３</summary>
	public int AxisAdditional13
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(13)]; }
		set { this.DataSource[GetAxisAdditionalKey(13)] = value; }
	}
	/// <summary>カラム加算値１４</summary>
	public int AxisAdditional14
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(14)]; }
		set { this.DataSource[GetAxisAdditionalKey(14)] = value; }
	}
	/// <summary>カラム加算値１５</summary>
	public int AxisAdditional15
	{
		get { return (int)this.DataSource[GetAxisAdditionalKey(15)]; }
		set { this.DataSource[GetAxisAdditionalKey(15)] = value; }
	}
	#endregion
}
