/*
=========================================================================================================
  Module      : Default Setting Table (DefaultSettingTable.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Common.Util;

namespace w2.App.Common.DefaultSetting
{
	/// <summary>
	/// Default setting table
	/// </summary>
	[Serializable]
	public class DefaultSettingTable
	{
		#region +Contructor
		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="tableName">Table name</param>
		public DefaultSettingTable(string tableName)
		{
			this.Name = tableName;
			this.Fields = new Dictionary<string, DefaultSettingField>();
		}
		#endregion

		#region +Method
		/// <summary>
		/// Add field
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <param name="fieldDefault">Field default</param>
		/// <param name="fieldComment">field comment</param>
		/// <param name="fieldDisplay">Field display</param>
		public void Add(
			string fieldName,
			object fieldDefault,
			string fieldComment,
			bool fieldDisplay)
		{
			this.Fields[fieldName] = new DefaultSettingField(
				fieldName,
				(string)fieldDefault,
				StringUtility.ToEmpty(fieldComment),
				fieldDisplay);
		}

		/// <summary>
		/// Get default setting value
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <returns>Default value</returns>
		public string GetDefaultSettingValue(string fieldName)
		{
			if (this.Fields.ContainsKey(fieldName)) return this.Fields[fieldName].Default;

			return string.Empty;
		}

		/// <summary>
		/// Get default setting display field
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <returns>TRUE: display / FALSE: not display</returns>
		public bool GetDefaultSettingDisplayField(string fieldName)
		{
			if (this.Fields.ContainsKey(fieldName)) return this.Fields[fieldName].Display;

			return true;
		}

		/// <summary>
		/// Get default setting comment field
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <returns>Comment</returns>
		public string GetDefaultSettingCommentField(string fieldName)
		{
			if (this.Fields.ContainsKey(fieldName)) return this.Fields[fieldName].Comment;

			return string.Empty;
		}

		/// <summary>
		/// Get field default setting values
		/// </summary>
		/// <returns>Field default setting values</returns>
		public Dictionary<string, string> GetFieldDefaultSettingValues()
		{
			var fieldDefaultSettingValues = this.Fields.Values.ToDictionary(
				defaultSettingField => defaultSettingField.Name,
				defaultSettingField => defaultSettingField.Default);

			return fieldDefaultSettingValues;
		}
		#endregion

		#region +Properties
		/// <summary>Name</summary>
		public string Name { get; set; }
		/// <summary>Fields</summary>
		public Dictionary<string, DefaultSettingField> Fields { get; set; }
		#endregion
	}
}
