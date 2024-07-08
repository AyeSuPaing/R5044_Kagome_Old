<%--
=========================================================================================================
Module      : DN��M (Receive.aspx)
�������������������������������������������������������������������������������������������������������
Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%-- Payment�t�H���_���̓����e���O�ɂȂ��Ă��邽�߁A�����ɓ���� --%>
<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" ValidateRequest="False" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	void Main()
	{
		// �K�v�ɉ����ďo�͐�̃f�B���N�g��
		string dirpath =  Constants.MACROKIOSK_DN_OUTPUT_DIR_PATH;
		if (!Directory.Exists(dirpath))
		{
			Directory.CreateDirectory(dirpath);
		}

		string filepath = Path.Combine(dirpath, string.Format(@"{0}.txt", DateTime.Now.ToString("rev_yyyyMMddHHmmssfff")));
		using (var fout = new StreamWriter(filepath, false))
		{
			//fout.WriteLine("��M���܂����B");
			var output = new List<string>();

			foreach (string pk in this.Request.QueryString.Keys)
			{
				output.Add(string.Format("{0}�F{1}", pk, this.Request.QueryString[pk]));
			}
			fout.WriteLine(string.Join(",", output.ToArray()));
		}
	}
</script>
<% this.Main(); %>