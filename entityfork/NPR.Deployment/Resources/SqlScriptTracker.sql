/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
/****** Object:  Table [dbo].[SqlScriptTracker]    Script Date: 05/21/2012 14:50:13 ******/
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SqlScriptTracker]') AND type in (N'U'))
BEGIN

	BEGIN TRANSACTION;

	CREATE TABLE dbo.SqlScriptTracker
		(
		ScriptTrackerId int NOT NULL IDENTITY (1, 1),
		CreatedDate datetime NULL,
		ScriptName varchar(MAX) NULL,
		IsSuccessful bit NULL,
		Details text NULL
		)  ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
	
	ALTER TABLE dbo.SqlScriptTracker ADD CONSTRAINT
		DF_SqlScriptTracker_CreatedDate DEFAULT GETDATE() FOR CreatedDate;
	
	ALTER TABLE dbo.SqlScriptTracker ADD CONSTRAINT
		PK_SqlScriptTracker PRIMARY KEY CLUSTERED 
		(
		ScriptTrackerId
		) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];

	
	ALTER TABLE dbo.SqlScriptTracker SET (LOCK_ESCALATION = TABLE);
	
	COMMIT;

END