if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[w2_DatabaseInfo]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[w2_DatabaseInfo]
GO
/*
=========================================================================================================
  Module      : データベース情報(w2_DatabaseInfo.sql)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
CREATE TABLE [dbo].[w2_DatabaseInfo] (
	[version] [nvarchar](20) NOT NULL,
	[number] [nvarchar](20) NOT NULL,
	[date_created] [datetime] NOT NULL,
	[date_changed] [datetime] NOT NULL,
	[last_changed] [nvarchar](50) NOT NULL
) ON [PRIMARY]
GO

CREATE CLUSTERED INDEX [IX_w2_DatabaseINfo_1] ON [dbo].[w2_DatabaseInfo]([version]) ON [PRIMARY]
GO

-- 初期値セット
INSERT INTO w2_DatabaseInfo
SELECT 'V5.0', '005', getdate(),getdate(),'setup'