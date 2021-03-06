/*
   Thursday, November 14, 20133:55:06 PM
   User: res_admin
   Server: restier2staging.cuwhyqxxofly.us-east-1.rds.amazonaws.com
   Database: npr
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Tickets ADD
	ListenToNprStation varchar(255) NULL,
	VisitorOfWebsite bit NOT NULL DEFAULT(0),
	Age varchar(255) NULL,
	Race varchar(255) NULL,
	TopicsOfInterest varchar(1024) NULL
GO
ALTER TABLE dbo.Tickets SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
select Has_Perms_By_Name(N'dbo.Tickets', 'Object', 'ALTER') as ALT_Per, Has_Perms_By_Name(N'dbo.Tickets', 'Object', 'VIEW DEFINITION') as View_def_Per, Has_Perms_By_Name(N'dbo.Tickets', 'Object', 'CONTROL') as Contr_Per 