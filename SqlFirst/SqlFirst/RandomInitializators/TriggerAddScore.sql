﻿--CREATE TRIGGER lafa 
--ON [dbo].[Student]  
--AFTER INSERT
--AS 
--BEGIN
--	DECLARE @subjectId UNIQUEIDENTIFIER = 
--	(SELECT TOP 1 sub.Id
--	FROM [dbo].[Subject] sub
--	JOIN [dbo].[SubjectCourse] subCo ON subCo.SubjectId = sub.Id AND subCo.CourseId = (SELECT CourseId FROM inserted)
--	JOIN [dbo].[SubjectSpecialty] subSpec ON subSpec.SubjectId = sub.Id AND subSpec.SpecialtyId = (SELECT SpecialtyId FROM inserted)
--	ORDER BY NEWID()
--	)
--	INSERT INTO [dbo].Score
--	(Id, [Value], StudentId, SubjectId, CourseId)
--	SELECT NEWID(), 5, i.Id, @subjectId, g.CourseId
--	FROM inserted i
--	JOIN [dbo].[Group] g ON g.Id = i.GroupId
--END