# Learner insights: completion and drop-off

Prepared for: Head of Learning
Scope: all 40 courses (free and paid) and all enrollments in the LearnHub dataset, judged as at 1 October 2026 (the dataset's "today").

## Summary

- **The five courses with the lowest completion rate are** Building REST APIs with .NET (11.76%), UI Design Basics (27.45%), Figma for Product Teams (27.59%), Python for Data Analysis (33.33%) and Product Management 101 (50.00%).
- **Building REST APIs with .NET is the weakest course:** 2 of 17 enrollments completed.
- **One lesson dominates in each course.** In UI Design Basics and Figma for Product Teams, the largest group stops after lesson 2 (34 of 37 and 36 of 42 non-complete enrollments respectively).
- **Lesson stall points by course:** lesson 3 in Building REST APIs with .NET (13 of 15), lesson 4 in Python for Data Analysis (16 of 20), and lesson 3 in Product Management 101 (12 of 13).
- **Never started:** only Building REST APIs with .NET has an enrollment with no lessons completed (1 of 15 non-complete). That bucket is smaller than the largest lesson bucket for that course, so it is not the main drop-off point. The other four courses have no never-started enrollments.
- **No course was excluded.** All 40 courses have at least one lesson and at least one enrollment.
- **Contact list:** 233 students are on the at-risk contact list (`reports/at-risk-students.csv`): 151 are both overdue and inactive, 47 are overdue only, and 35 are inactive only.
- The 151 "both" students are the suggested first calls this week.

## Bottom 5 by completion rate

Completion rate = enrollments where the student completed every lesson of the course / enrollments in the course. Ranked lowest first. There are no ties within the bottom five.

| Rank | Course id | Title | Price | Enrollments | Completed | Completion rate % |
|---|---|---|---|---|---|---|
| 1 | 9 | Building REST APIs with .NET | 59.00 | 17 | 2 | 11.76 |
| 2 | 3 | UI Design Basics | 30.00 | 51 | 14 | 27.45 |
| 3 | 14 | Figma for Product Teams | 49.00 | 58 | 16 | 27.59 |
| 4 | 22 | Python for Data Analysis | 49.00 | 30 | 10 | 33.33 |
| 5 | 31 | Product Management 101 | 49.00 | 26 | 13 | 50.00 |

Ties are broken by more enrollments first, then lower course id. No ties occurred in this ranking.

## Excluded courses

None. Every course in `courses` has at least one lesson and at least one enrollment, so all 40 are in the ranking.

## Drop-off

For each bottom-five course, the table counts non-complete enrollments by their last completed lesson (the highest lesson position among their completed lessons). "Never started" means no lessons completed.

### Building REST APIs with .NET (course 9)

Most students stop after lesson 3 (Async code), so they stall at lesson 4 (Conditions and loops).

| Last completed lesson | Lesson title | Next lesson | Enrollments stopped |
|---|---|---|---|
| 3 | Async code | Conditions and loops | 13 |
| 4 | Conditions and loops | Functions and methods | 1 |
| Never started | - | - | 1 |

### UI Design Basics (course 3)

Most students stop after lesson 2 (Case study review), so they stall at lesson 3 (Usability checks).

| Last completed lesson | Lesson title | Next lesson | Enrollments stopped |
|---|---|---|---|
| 2 | Case study review | Usability checks | 34 |
| 3 | Usability checks | Wireframes | 1 |
| 4 | Wireframes | Icons and imagery | 2 |
| Never started | - | - | 0 |

### Figma for Product Teams (course 14)

Most students stop after lesson 2 (Wireframes), so they stall at lesson 3 (Usability checks).

| Last completed lesson | Lesson title | Next lesson | Enrollments stopped |
|---|---|---|---|
| 1 | Case study review | Wireframes | 1 |
| 2 | Wireframes | Usability checks | 36 |
| 4 | Feedback sessions | Colour and contrast | 2 |
| 5 | Colour and contrast | Design handoff | 3 |
| Never started | - | - | 0 |

### Python for Data Analysis (course 22)

Most students stop after lesson 4 (Grouping and totals), so they stall at lesson 5 (Reading a table).

| Last completed lesson | Lesson title | Next lesson | Enrollments stopped |
|---|---|---|---|
| 2 | Sampling | Cleaning messy data | 3 |
| 3 | Cleaning messy data | Grouping and totals | 1 |
| 4 | Grouping and totals | Reading a table | 16 |
| Never started | - | - | 0 |

### Product Management 101 (course 31)

Most students stop after lesson 3 (Running meetings), so they stall at lesson 4 (Estimating).

| Last completed lesson | Lesson title | Next lesson | Enrollments stopped |
|---|---|---|---|
| 2 | Stakeholders | Running meetings | 1 |
| 3 | Running meetings | Estimating | 12 |
| Never started | - | - | 0 |

## Checks

- For each course, the enrollments in the drop-off tables sum to enrollments minus completed in the ranking table (course 9: 15, course 3: 37, course 14: 42, course 22: 20, course 31: 13).
- No lesson bucket is tied for the largest count in any of the five courses.

## Who is on the contact list, and why

The contact list (`reports/at-risk-students.csv`) names students who need a support call. A student qualifies on either of two tests, and the list is the union of the two groups. "Now" is 1 October 2026, 12:00 (the dataset clock).

**Inactive test.** A student qualifies for an enrolment when all of the following hold:

- The enrolment is not complete (fewer lessons completed than the course has).
- The student has completed no lesson in that course in the 14-day window, which runs from 17 September 2026 12:00 up to 1 October 2026 12:00.
- The enrolment was made on or before 17 September 2026 12:00. Anyone who enrolled inside the window is excluded, as they have not yet had a fair chance to start.

Completed courses are excluded.

**Overdue test.** A student qualifies for an assignment when all of the following hold:

- The assignment is due before 1 October 2026 12:00.
- The student is enrolled in that course.
- The student enrolled on or before the due date. A student cannot be overdue on work that was already past due when they joined.
- There is no row in `submissions` for that student and assignment. A submission made after the due date still counts as submitted.

Assignments due after 1 October 2026 are not overdue. The latest due date in the data is 10 December 2026.

**Combining the tests.** A student is on the list if they meet either test. Students who meet both are in the "both" group.

**Why the enrolment-date condition.** Without it, 7,936 student-assignment pairs would be overdue, and all 283 enrolled students would qualify, so the list would not discriminate between students. With it, 1,398 pairs across 114 distinct assignments remain, affecting 198 students.

**Exclusions.**

- Completed courses, for the inactive test.
- Enrolments made inside the 14-day window, for the inactive test.
- Assignments already past due when the student enrolled, for the overdue test.
- Late submissions are treated as submitted.
- Twelve end-to-end test accounts (emails ending `.test` but not `example.test`) are excluded. They are leftover test data, not learners.

**Counts.**

- Inactive: 469 student-course enrolments, affecting 186 students.
- Overdue: 1,398 student-assignment pairs across 114 assignments, affecting 198 students.
- The dataset has 312 students, of which 283 have at least one enrolment. The test accounts are counted in those figures but are excluded from the list.
- The list has 233 students: 151 both, 47 overdue only, and 35 inactive only.
- Check: 198 + 186 - 151 = 233.

**How the CSV is sorted.** Rows are ordered by group (both, then overdue only, then inactive only). Within each group, rows are sorted by `days_since_last_activity` (descending), then by `student_id`. There is one row per student.

**Columns.**

- `student_id`, `full_name`, `email`: identify the student.
- `reason`: `both`, `overdue`, or `inactive`.
- `courses`: the inactive courses, joined with ` | `.
- `days_since_last_activity`: days from the student's most recent lesson completion in any course (or their latest enrolment, if they have none) to 1 October 2026 12:00.
- `overdue_assignments`: each overdue assignment with its course and due date, joined with ` | `.
- `suggested_message`: a short message for the support team to use.

`days_since_last_activity` measures activity across all courses, so a student can show 0 days and still be listed because they have gone quiet in a different course.

**How the messages were built.** Each student has one line from one of three templates (inactive only, overdue only, or both). Each uses the student's first name and names a specific course or assignment. Where there are several overdue assignments, the earliest-due one is named, followed by a count of the others. In "both" rows, the named assignment can belong to a different course from the inactive one.

## Caveats

- **The list is long.** It has 233 of the 283 enrolled students, because most seeded assignments have no submissions. The "both" group is the natural priority. Support may want to cap the number of calls per day.
- **The enrolment-date condition is a judgement call.** It goes beyond the literal request ("past its due date with no submission"). Without it, every enrolled student qualifies.
- **`days_since_last_activity` can be 0 for inactive rows.** This is explained above: the student may have been active in another course.
- **The CSV contains personal data.** It holds student names and email addresses, so it is for support use only.
- **Lesson titles do not always match their course topic.** For example, "Building REST APIs with .NET" has a lesson titled "Conditions and loops". The drop-off lesson names above are reported as the data records them.
- **The Phase 1 checks remain valid.** The "Checks" section above is unaffected by this phase, as no existing numbers were changed.
