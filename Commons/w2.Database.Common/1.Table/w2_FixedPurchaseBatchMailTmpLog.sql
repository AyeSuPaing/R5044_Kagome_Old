if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_FixedPurchaseBatchMailTmpLog]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_FixedPurchaseBatchMailTmpLog]
GO
/*
=========================================================================================================
  Module      : ����w���o�b�`���[���̈ꎞ���O (w2_FixedPurchaseBatchMailTmpLog.sql)
 �������������������������������������������������������������������������������������������������������
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_FixedPurchaseBatchMailTmpLog] (
	[tmp_log_id] [int] IDENTITY(1,1) NOT NULL,
	[master_type] [nvarchar] (10) DEFAULT ('order'),
	[master_id] [nvarchar] (100) NOT NULL DEFAULT (''),
	[action_master_id] [nvarchar] (30) NOT NULL DEFAULT ('')
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[w2_FixedPurchaseBatchMailTmpLog] WITH NOCHECK ADD
	CONSTRAINT [PK_w2_FixedPurchaseBatchMailTmpLog] PRIMARY KEY  CLUSTERED
	(
		[tmp_log_id]
	) ON [PRIMARY]
GO

CREATE INDEX [IX_w2_FixedPurchaseBatchMailTmpLog_1] ON [dbo].[w2_FixedPurchaseBatchMailTmpLog]([action_master_id]) ON [PRIMARY]
GO
