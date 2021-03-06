﻿CREATE VIEW [dbo].[SmallSubjectScore]
	AS SELECT sub.Id, sub.[Name], st.FirstName, st.LastName, score.[Value]
FROM [dbo].[Score] score 
JOIN [dbo].[Subject] sub ON sub.Id = score.SubjectId
JOIN [dbo].Student st ON st.Id = score.StudentId
WHERE score.[Value] <
(SELECT AVG(sc.[Value])
FROM [dbo].[Score] sc
WHERE score.SubjectId = sc.SubjectId)