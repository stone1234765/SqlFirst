﻿CREATE TABLE [dbo].[StudentScore]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Count] INT NOT NULL, 
    [SubjectId] UNIQUEIDENTIFIER NOT NULL, 
    [StudentId] UNIQUEIDENTIFIER NOT NULL 
)