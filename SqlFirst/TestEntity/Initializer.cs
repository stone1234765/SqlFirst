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

        public void Seed(ModelBuilder modelBuilder)
        {
            Random random = new Random();

            var courses = AddCourses();
            var specialties = AddRandomSpecialties(random);
            var subjects = AddRandomSubjects(random);
            var subjectSpecialties = AddRandomSubjectSpecialties(subjects, specialties, random);
            var subjectCourses = AddRandomSubjectCourses(subjects, courses, random);
            var groups = AddGroups(courses, specialties);
            var students = AddRandomStudents(groups, random);
            var scores = AddRandomScores(students, subjectCourses, subjectSpecialties, courses, groups, random);


            modelBuilder.Entity<Course>().HasData(courses);
            modelBuilder.Entity<Specialty>().HasData(specialties);
            modelBuilder.Entity<Subject>().HasData(subjects);
            modelBuilder.Entity<SubjectSpecialty>().HasData(subjectSpecialties);
            modelBuilder.Entity<SubjectCourse>().HasData(subjectCourses);
            modelBuilder.Entity<Group>().HasData(groups);
            modelBuilder.Entity<Student>().HasData(students);
            modelBuilder.Entity<Score>().HasData(scores);
        }

        private List<Course> AddCourses()
        {
            return new List<Course>
            {
                    new Course() { Id = Guid.NewGuid(), Name = 1 },
                    new Course() { Id = Guid.NewGuid(), Name = 2 },
                    new Course() { Id = Guid.NewGuid(), Name = 3 },
                    new Course() { Id = Guid.NewGuid(), Name = 4 },
                    new Course() { Id = Guid.NewGuid(), Name = 5 },
                    new Course() { Id = Guid.NewGuid(), Name = 6 }
            };
        }
        private List<Specialty> AddRandomSpecialties(Random random)
        {
            List<Specialty> specialties = new List<Specialty>();
            for (int i = 0; i < 80; i++)
            {
                specialties.Add(new Specialty()
                {
                    Id = Guid.NewGuid(),
                    Name = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random)
                });
            }
            return specialties;
        }

        private List<Subject> AddRandomSubjects(Random random)
        {
            List<Subject> subjects = new List<Subject>();
            for (int i = 0; i < 1000; i++)
            {
                subjects.Add(new Subject()
                {
                    Id = Guid.NewGuid(),
                    Name = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random)
                });
            }
            return subjects;
        }


        private List<SubjectCourse> AddRandomSubjectCourses(List<Subject> subjects, List<Course> courses, Random random)
        {
            List<SubjectCourse> subjectCourses = new List<SubjectCourse>();
            for (int i = 0; i < subjects.Count; i++)
            {
                for (int j = 0; j < courses.Count; j++)
                {
                    if (CreateRandomBool(random, 30))
                    {
                        subjectCourses.Add(new SubjectCourse()
                        {
                            SubjectId = subjects[i].Id,
                            CourseId = courses[j].Id
                        });
                    }
                }
            }
            return subjectCourses;
        }

        private List<SubjectSpecialty> AddRandomSubjectSpecialties(List<Subject> subjects, List<Specialty> specialties, Random random)
        {
            List<SubjectSpecialty> subjectSpecialties = new List<SubjectSpecialty>();
            for (int i = 0; i < subjects.Count; i++)
            {
                for (int j = 0; j < specialties.Count; j++)
                {
                    if (CreateRandomBool(random, 5))
                    {
                        subjectSpecialties.Add(new SubjectSpecialty()
                        {
                            SubjectId = subjects[i].Id,
                            SpecialtyId = specialties[j].Id,
                        });
                    }
                }
            }
            return subjectSpecialties;
        }

        private List<Group> AddGroups(List<Course> courses, List<Specialty> specialties)
        {
            List<Group> groups = new List<Group>();
            for (int i = 0; i < courses.Count; i++)
            {
                for (int j = 0; j < specialties.Count; j++)
                {
                    groups.Add(new Group()
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{courses[i].Name} - {specialties[j].Name}",
                        CourseId = courses[i].Id,
                        SpecialtyId = specialties[j].Id
                    });
                }
            }
            return groups;
        }

        private List<Student> AddRandomStudents(List<Group> groups, Random random)
        {
            List<Student> students = new List<Student>();
            for (int i = 0; i < 5; i++)
            {
                var indexGroup = random.Next(0, groups.Count);
                students.Add(new Student()
                {
                    Id = Guid.NewGuid(),
                    FirstName = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random),
                    LastName = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random),
                    GroupId = groups[indexGroup].Id
                });
            }
            return students;
        }

        private List<Score> AddRandomScores(List<Student> students, List<SubjectCourse> subjectCourses, List<SubjectSpecialty> subjectSpecialties,
            List<Course> courses, List<Group> groups, Random random)
        {
            List<Score> scores = new List<Score>();
            for (int i = 0; i < students.Count; i++)
            {
                var needGroup = groups.Where(group => group.Id == students[i].GroupId).First();
                var needCourse = courses.Where(course => course.Id == needGroup.CourseId).First();
                for (int j = 1; j <= needCourse.Name; j++)
                {
                    var course = courses.First(c => c.Name == j);
                    var needSubjects = subjectCourses
                        .Where(subCo => subCo.CourseId == course.Id)
                        .Select(subCo => subCo.SubjectId)
                        .Intersect(subjectSpecialties
                        .Where(subSpec => subSpec.SpecialtyId == needGroup.SpecialtyId)
                        .Select(subCo => subCo.SubjectId));

                    foreach (var subject in needSubjects)
                    {
                        if (CreateRandomBool(random, 95))
                        {
                            scores.Add(new Score()
                            {
                                Id = Guid.NewGuid(),
                                StudentId = students[i].Id,
                                SubjectId = subject,
                                CourseId = course.Id,
                                Value = random.Next(1, 6)
                            });
                        }
                    }
                }
            }
            return scores;
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




















        //private async Task<Course[]> AddCourses(UniversityContext context)
        //{
        //    Course[] courses = new Course[]
        //    {
        //            (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 1 })).Entity,
        //            (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 2 })).Entity,
        //            (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 3 })).Entity,
        //            (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 4 })).Entity,
        //            (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 5 })).Entity,
        //            (await context.Courses.AddAsync(new Course() { Id = Guid.NewGuid(), Name = 6 })).Entity
        //    };
        //    return courses;
        //}
        //private async Task<Specialty[]> AddRandomSpecialties(UniversityContext context, Random random)
        //{
        //    Specialty[] specialties = new Specialty[80];
        //    for (int i = 0; i < specialties.Length; i++)
        //    {
        //        specialties[i] = (await context.Specialties.AddAsync(new Specialty()
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random)
        //        })).Entity;
        //    }
        //    return specialties;
        //}

        //private async Task<Subject[]> AddRandomSubjects(UniversityContext context, Random random)
        //{
        //    Subject[] subjects = new Subject[1000];
        //    for (int i = 0; i < subjects.Length; i++)
        //    {
        //        subjects[i] = (await context.Subjects.AddAsync(new Subject()
        //        {
        //            Id = Guid.NewGuid(),
        //            Name = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random)
        //        })).Entity;
        //    }
        //    return subjects;
        //}
        //private async Task<SubjectCourse[]> AddRandomSubjectCourses(UniversityContext context, Subject[] subjects, Course[] courses, Random random)
        //{
        //    List<SubjectCourse> subjectCourses = new List<SubjectCourse>();
        //    for (int i = 0; i < subjects.Length; i++)
        //    {
        //        for (int j = 0; j < courses.Length; j++)
        //        {
        //            if (CreateRandomBool(random, 30))
        //            {
        //                subjectCourses.Add((await context.SubjectCourses.AddAsync(new SubjectCourse()
        //                {
        //                    Subject = subjects[i],
        //                    SubjectId = subjects[i].Id,
        //                    Course = courses[j],
        //                    CourseId = courses[j].Id
        //                })).Entity);
        //            }
        //        }
        //    }
        //    return subjectCourses.ToArray();
        //}
        //private async Task<SubjectSpecialty[]> AddRandomSubjectSpecialties(UniversityContext context, Subject[] subjects, Specialty[] specialties, Random random)
        //{
        //    List<SubjectSpecialty> subjectSpecialties = new List<SubjectSpecialty>();
        //    for (int i = 0; i < subjects.Length; i++)
        //    {
        //        for (int j = 0; j < specialties.Length; j++)
        //        {
        //            if (CreateRandomBool(random, 5))
        //            {
        //                subjectSpecialties.Add((await context.SubjectSpecialties.AddAsync(new SubjectSpecialty()
        //                {
        //                    Subject = subjects[i],
        //                    SubjectId = subjects[i].Id,
        //                    Specialty = specialties[j],
        //                    SpecialtyId = specialties[j].Id,
        //                })).Entity);
        //            }
        //        }
        //    }
        //    return subjectSpecialties.ToArray();
        //}
        //private async Task<Group[]> AddGroups(UniversityContext context, Course[] courses, Specialty[] specialties)
        //{
        //    List<Group> groups = new List<Group>();
        //    for (int i = 0; i < courses.Length; i++)
        //    {
        //        for (int j = 0; j < specialties.Length; j++)
        //        {
        //            groups.Add((await context.Groups.AddAsync(new Group()
        //            {
        //                Id = Guid.NewGuid(),
        //                Name = $"{courses[i].Name} - {specialties[j].Name}",
        //                Course = courses[i],
        //                CourseId = courses[i].Id,
        //                Specialty = specialties[j],
        //                SpecialtyId = specialties[j].Id
        //            })).Entity);
        //        }
        //    }
        //    return groups.ToArray();
        //}

        //private async Task<Student[]> AddRandomStudents(UniversityContext context, Group[] groups, Random random)
        //{
        //    Student[] students = new Student[5];
        //    for (int i = 0; i < students.Length; i++)
        //    {
        //        var indexGroup = random.Next(0, groups.Length);
        //        students[i] = ((await context.Students.AddAsync(new Student()
        //        {
        //            Id = Guid.NewGuid(),
        //            FirstName = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random),
        //            LastName = CreateRandomString(1, 50, "abcdefghijklmnopqrstuvwxyz", random),
        //            Group = groups[indexGroup],
        //            GroupId = groups[indexGroup].Id
        //        })).Entity);
        //    }
        //    return students.ToArray();
        //}
        //private async Task<Score[]> AddRandomScores(UniversityContext context, Student[] students, Subject[] subjects, Course[] courses, Random random)
        //{
        //    List<Score> scores = new List<Score>();
        //    for (int i = 0; i < students.Length; i++)
        //    {
        //        for (int j = 1; j <= students[i].Group.Course.Name; j++)
        //        {
        //            var course = courses.First(c => c.Name == j);
        //            var needSubjects = subjects.Where(subjectss => subjectss.SubjectCourses != null
        //                && subjectss.SubjectSpecialties != null
        //                && subjectss.SubjectCourses.Select(subCo => subCo.CourseId).Contains(course.Id)
        //                && subjectss.SubjectSpecialties.Select(subSpec => subSpec.SpecialtyId).Contains(students[i].Group.Specialty.Id));

        //            foreach (var subject in needSubjects)
        //            {
        //                if (CreateRandomBool(random, 95))
        //                {
        //                    scores.Add((await context.Scores.AddAsync(new Score()
        //                    {
        //                        Id = Guid.NewGuid(),
        //                        Student = students[i],
        //                        StudentId = students[i].Id,
        //                        Subject = subject,
        //                        SubjectId = subject.Id,
        //                        Course = course,
        //                        CourseId = course.Id,
        //                        Value = random.Next(1, 6)
        //                    })).Entity);
        //                }
        //            }
        //        }
        //    }
        //    return scores.ToArray();
        //}
        //public async Task<bool> FirstInitializeData()
        //{
        //    Random random = new Random();
        //    using (var context = new UniversityContext())
        //    {
        //        if ((await context.Courses.ToListAsync()).Count > 0 || (await context.Specialties.ToListAsync()).Count > 0 || (await context.Subjects.ToListAsync()).Count > 0 
        //            || (await context.SubjectSpecialties.ToListAsync()).Count > 0 || (await context.SubjectCourses.ToListAsync()).Count > 0 
        //            || (await context.Groups.ToListAsync()).Count > 0 || (await context.Students.ToListAsync()).Count > 0 || (await context.Scores.ToListAsync()).Count > 0)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            var courses = await AddCourses(context);
        //            var specialties = await AddRandomSpecialties(context, random);
        //            var subjects = await AddRandomSubjects(context, random);
        //            var subjectSpecialties = await AddRandomSubjectSpecialties(context, subjects, specialties, random);
        //            var subjectCourses = await AddRandomSubjectCourses(context, subjects, courses, random);
        //            var groups = await AddGroups(context, courses, specialties);
        //            var students = await AddRandomStudents(context, groups, random);
        //            var scores = await AddRandomScores(context, students, subjects, courses, random);
        //            await context.SaveChangesAsync();
        //            return true;
        //        }
        //    }
        //}




        //private string CreateRandomString(int minLength, int maxLenght, string chars, Random random)
        //{
        //    StringBuilder stringBuilder = new StringBuilder();
        //    for (int i = 0; i < random.Next(minLength, maxLenght); i++)
        //    {
        //        stringBuilder.Append(chars[random.Next(0, chars.Length)]);
        //    }
        //    return stringBuilder.ToString();
        //}
        //private bool CreateRandomBool(Random random, int percent)
        //{
        //    if (percent > random.Next(1, 101)) 
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
