---
name: seed-sample-data
description: Adds realistic sample data (courses, lessons, quiz questions) to the LearnHub InMemoryStore. Use when the user asks for more sample data, demo data, extra courses, or example quizzes.
---

# Seed sample data

All data lives in `backend/LearnHub.Api/Data/InMemoryStore.cs`. Do not create other data files.

## Steps

1. Read `InMemoryStore.cs`. Note the highest course id (now 6) and the highest lesson id (now 24).
2. Add new courses to the `Courses` list in the same shape:
   - `new Course { Id = ..., Title, ShortDescription, Category, Instructor, Level, Lessons = { ... } }`
   - Categories in use: Programming, Design, Data, Cloud. Levels: Beginner, Intermediate, Advanced.
   - Give each course 3 to 5 lessons: `new Lesson { Id, Title, DurationMinutes, Content }`.
   - Keep content short and in plain words, like the existing lessons.
3. For quiz questions: if a `Quiz` or `Question` model exists in `Models/`, add questions in the same way.
   If it does not exist, ask the user to run the `add-api-endpoint` skill first.
   Each question has text, 3 or 4 options, and the index of the right answer.
4. Check for duplicates.
   - Course ids must be unique. Lesson ids must be unique across all courses. Continue from the highest id.
   - Do not repeat an existing course title.
5. Build: `cd backend && dotnet build`.
6. Optional: run the API and `curl localhost:5080/api/courses` to see the new data. Stop the API after.
7. List the new courses and their ids for the user.
