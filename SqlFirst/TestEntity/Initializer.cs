﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEntity
{
    class Initializer
    {
        public Initializer()
        {

        }

        private async Task<Course[]> AddCourses(UniversityContext context)
        {
            Course[] courses = new Course[]
            {
                    (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 1 })).Entity,
                    (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 2 })).Entity,
                    (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 3 })).Entity,
                    (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 4 })).Entity,
                    (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 5 })).Entity,
                    (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 6 })).Entity
            };
            return courses;
        }
        private async Task<Specialty[]> AddRandomSpecialties(UniversityContext context, Random random)
        {
            Specialty[] specialties = new Specialty[80];
            for (int i = 0; i < specialties.Length; i++)
            {
                specialties[i] = (await context.Specialties.AddAsync(new Specialty()
                {
                    Id = Guid.NewGuid(),
                    Name = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random)
                })).Entity;
            }
            return specialties;
        }

        public async Task<string> CreateRandomString(SqlParameter[] parameters)
        {
            using (var university = new UniversityContext())
            {
                await university.Database.ExecuteSqlRawAsync("[dbo].[PickRandomString] @minLength, @maxLength, @chars, @randomString OUTPUT", parameters);
                return parameters.Where(parameter => parameter.ParameterName == "@randomString").First().Value.ToString();
            }
        }

        private async Task<Subject[]> AddRandomSubjects(UniversityContext context, Random random)
        {
            Subject[] subjects = new Subject[1000];
            for (int i = 0; i < subjects.Length; i++)
            {
                subjects[i] = (await context.Subjects.AddAsync(new Subject()
                {
                    Id = Guid.NewGuid(),
                    Name = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random)
                })).Entity;
            }
            return subjects;
        }
        private async Task<SubjectCourse[]> AddRandomSubjectCourses(UniversityContext context, Subject[] subjects, Course[] courses, Random random)
        {
            List<SubjectCourse> subjectCourses = new List<SubjectCourse>();
            for (int i = 0; i < subjects.Length; i++)
            {
                for (int j = 0; j < courses.Length; j++)
                {
                    if (CreateRandomBool(random, 30))
                    {
                        subjectCourses.Add((await context.SubjectCourses.AddAsync(new SubjectCourse()
                        {
                            Subject = subjects[i],
                            SubjectId = subjects[i].Id,
                            Course = courses[j],
                            CourseId = courses[j].Id
                        })).Entity);
                    }
                }
            }
            return subjectCourses.ToArray();
        }
        private async Task<SubjectSpecialty[]> AddRandomSubjectSpecialties(UniversityContext context, Subject[] subjects, Specialty[] specialties, Random random)
        {
            List<SubjectSpecialty> subjectSpecialties = new List<SubjectSpecialty>();
            for (int i = 0; i < subjects.Length; i++)
            {
                for (int j = 0; j < specialties.Length; j++)
                {
                    if (CreateRandomBool(random, 5))
                    {
                        subjectSpecialties.Add((await context.SubjectSpecialties.AddAsync(new SubjectSpecialty()
                        {
                            Subject = subjects[i],
                            SubjectId = subjects[i].Id,
                            Specialty = specialties[j],
                            SpecialtyId = specialties[j].Id,
                        })).Entity);
                    }
                }
            }
            return subjectSpecialties.ToArray();
        }
        private async Task<Group[]> AddGroups(UniversityContext context, Course[] courses, Specialty[] specialties)
        {
            List<Group> groups = new List<Group>();
            for (int i = 0; i < courses.Length; i++)
            {
                for (int j = 0; j < specialties.Length; j++)
                {
                    groups.Add((await context.Groups.AddAsync(new Group()
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{courses[i].Name} - {specialties[j].Name}",
                        Course = courses[i],
                        CourseId = courses[i].Id,
                        Specialty = specialties[j],
                        SpecialtyId = specialties[j].Id
                    })).Entity);
                }
            }
            return groups.ToArray();
        }

        private async Task<Student[]> AddRandomStudents(UniversityContext context, Group[] groups, Random random)
        {
            Student[] students = new Student[5];
            for (int i = 0; i < students.Length; i++)
            {
                var indexGroup = random.Next(0, groups.Length);
                students[i] = ((await context.Students.AddAsync(new Student()
                {
                    Id = Guid.NewGuid(),
                    FirstName = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random),
                    LastName = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random),
                    Group = groups[indexGroup],
                    GroupId = groups[indexGroup].Id
                })).Entity);
            }
            return students.ToArray();
        }
        private async Task<Score[]> AddRandomScores(UniversityContext context, Student[] students, Subject[] subjects, Course[] courses, Random random)
        {
            List<Score> scores = new List<Score>();
            for (int i = 0; i < students.Length; i++)
            {
                for (int j = 1; j <= students[i].Group.Course.Name; j++)
                {
                    var course = courses.First(c => c.Name == j);
                    var needSubjects = subjects.Where(subjectss => subjectss.SubjectCourses != null
                        && subjectss.SubjectSpecialties != null
                        && subjectss.SubjectCourses.Select(subCo => subCo.CourseId).Contains(course.Id)
                        && subjectss.SubjectSpecialties.Select(subSpec => subSpec.SpecialtyId).Contains(students[i].Group.Specialty.Id));

                    foreach (var subject in needSubjects)
                    {
                        if (CreateRandomBool(random, 95))
                        {
                            scores.Add((await context.Scores.AddAsync(new Score()
                            {
                                Id = Guid.NewGuid(),
                                Student = students[i],
                                StudentId = students[i].Id,
                                Subject = subject,
                                SubjectId = subject.Id,
                                Course = course,
                                CourseId = course.Id,
                                Value = random.Next(1, 6)
                            })).Entity);
                        }
                    }
                }
            }
            return scores.ToArray();
        }
        public async Task<bool> FirstInitializeData()
        {
            Random random = new Random();
            using (var context = new UniversityContext())
            {
                if ((await context.Courses.ToListAsync()).Count > 0 || (await context.Specialties.ToListAsync()).Count > 0 || (await context.Subjects.ToListAsync()).Count > 0 
                    || (await context.SubjectSpecialties.ToListAsync()).Count > 0 || (await context.SubjectCourses.ToListAsync()).Count > 0 
                    || (await context.Groups.ToListAsync()).Count > 0 || (await context.Students.ToListAsync()).Count > 0 || (await context.Scores.ToListAsync()).Count > 0)
                {
                    return false;
                }
                else
                {
                    var courses = await AddCourses(context);
                    var specialties = await AddRandomSpecialties(context, random);
                    var subjects = await AddRandomSubjects(context, random);
                    var subjectSpecialties = await AddRandomSubjectSpecialties(context, subjects, specialties, random);
                    var subjectCourses = await AddRandomSubjectCourses(context, subjects, courses, random);
                    var groups = await AddGroups(context, courses, specialties);
                    var students = await AddRandomStudents(context, groups, random);
                    var scores = await AddRandomScores(context, students, subjects, courses, random);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
        }
        public async Task AddStudents(List<Student> students)
        {
            using (var university = new UniversityContext())
            {
                await university.Students.AddRangeAsync(students);
                await university.SaveChangesAsync();
            }
        }
        public async Task AddScores(List<Score> scores)
        {
            using (var university = new UniversityContext())
            {
                await university.Scores.AddRangeAsync(scores);
                await university.SaveChangesAsync();
            }
        }
        public async Task ChangeScores(List<Score> scores)
        {
            using (var university = new UniversityContext())
            {
                university.Scores.UpdateRange(scores);
                await university.SaveChangesAsync();
            }
        }
        public async Task<List<StudentScoresCount>> GetStudentScoresCount()
        {
            using (var university = new UniversityContext())
            {
                return await university.StudentScoresCounts
                    .ToListAsync();
            }
        }
        public async Task<List<Student>> GetCleverStudents(SqlParameter sqlParameter)
        {
            using (var university = new UniversityContext())
            {
                return await university.Students.FromSqlRaw("SELECT * " +
                    "FROM GetCleverStudents(@maxFoursCount)", sqlParameter).ToListAsync();
            }
        }
        public async Task<List<Student>> GetStudents()
        {
            using (var university = new UniversityContext())
            {
                return await university.Students
                    .Include(student => student.Group)
                    .ToListAsync();
            }
        }
        public async Task<List<Score>> GetScores()
        {
            using (var university = new UniversityContext())
            {
                return await university.Scores
                    .Include(score => score.Course)
                    .Include(score => score.Subject)
                    .Include(score => score.Student)
                    .ThenInclude(student => student.Group)
                    .ToListAsync();
            }
        }
        public async Task<List<Group>> GetGroups()
        {
            using (var university = new UniversityContext())
            {
                return await university.Groups
                    .Include(group => group.Course)
                    .Include(group => group.Specialty)
                    .ToListAsync();
            }
        }
        public async Task<List<Course>> GetCourses()
        {
            using (var university = new UniversityContext())
            {
                return await university.Courses.ToListAsync();
            }
        }
        public async Task<List<Subject>> GetSubjects()
        {
            using (var university = new UniversityContext())
            {
                return await university.Subjects
                    .Where(subject => subject.SubjectCourses != null && subject.SubjectSpecialties != null)
                    .Include(subject => subject.SubjectCourses)
                    .ThenInclude(subjectCourse => subjectCourse.Course)
                    .Include(subject => subject.SubjectSpecialties)
                    .ThenInclude(subjectSpecialty => subjectSpecialty.Specialty)
                    .ToListAsync();
            }
        }
        public async Task<List<Specialty>> GetSpecialties()
        {
            using (var university = new UniversityContext())
            {
                return await university.Specialties.ToListAsync();
            }
        }
        private async Task<List<Subject>> GetAllSubjects()
        {
            using (var university = new UniversityContext())
            {
                return await university.Subjects
                    .Where(subject => subject.SubjectCourses != null && subject.SubjectSpecialties != null)
                    .Include(subject => subject.SubjectCourses)
                    //.ThenInclude(subjectCourse => subjectCourse.Course)
                    .Include(subject => subject.SubjectSpecialties)
                    //.ThenInclude(subjectSpecialty => subjectSpecialty.Specialty)
                    .ToListAsync();
            }
        }
        public async Task<int> UpdateData(string sql)
        {
            using (var university = new UniversityContext())
            {
                return await university.Database.ExecuteSqlRawAsync(sql);
            }
        }




        private string CreateRandomString(int minLength, int maxLenght, string chars, Random random)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < random.Next(minLength, maxLenght); i++)
            {
                stringBuilder.Append(chars[random.Next(0, chars.Length)]);
            }
            return stringBuilder.ToString();
        }
        private bool CreateRandomBool(Random random, int percent)
        {
            if (percent > random.Next(1, 101)) 
            {
                return true;
            }
            return false;
        }
    }
}
