if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_LogProccessEndSp]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[w2_LogProccessEndSp]
GO
/*
=========================================================================================================
  Module      : ���O���H�����v���V�[�W��(w2_LogProccessEndSp.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
CREATE PROCEDURE [dbo].[w2_LogProccessEndSp] @TARGET_DATE varchar(10) AS

	-- ���H���������X�e�[�^�X�֕ύX
	UPDATE	w2_AccessLogStatus
	   SET	day_status = '20'
	 WHERE	target_date = @TARGET_DATE

GO