<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" validaterequest="False"%>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	void Main()
	{
		// �K�v�ɉ����ďo�͐�̃f�B���N�g��
		string dirpath = @"C:\Logs\R5044_Kagome.Develop\MacroKiosk";
		if (!Directory.Exists(dirpath))
		{
			Directory.CreateDirectory(dirpath);
		}

		string filepath = string.Format(@"{0}\{1}.txt", dirpath, DateTime.Now.ToString("rev_yyyyMMddHHmmssfff"));
		using (var fout = new StreamWriter(filepath, false))
		{
			fout.WriteLine("��M���܂����B");

			foreach (string pk in this.Request.Form.Keys)
			{
				fout.WriteLine(string.Format("{0}�F{1}", pk, this.Request.Form[pk]));
			}
		}
	}
</script>
<% this.Main(); %>