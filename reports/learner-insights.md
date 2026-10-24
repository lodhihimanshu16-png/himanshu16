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
