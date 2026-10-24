# Review and prerequisite integrity audit

Prepared for: Trust and Safety and Product owners
Scope: all 2,000 reviews, and all enrolments in courses that have a prerequisite, in the LearnHub dataset, judged as at 1 October 2026 (the dataset's "today").

## Summary

- **65 of 2,000 reviews (3.3%) were written by students with no enrolment in that course.** They sit across 28 courses.
- **25 of the 65 are one burst.** All are 5-star reviews on course 17 (Accessible Web Design) on 15 September 2026, written by 25 accounts that joined two days earlier and never enrolled. Course 17's average fell from 3.69 to 3.08 once they are removed. This is the only course whose rating moves materially.
- **The other 40 are not from new accounts and have no order for their course**, so refunds do not explain them. They are evidence of "not enrolled" only. We cannot prove motive, and the new-account pattern is proven for course 17 alone.
- **47 enrolments broke a prerequisite.** Eight are E2E test students (course 6, enrolled 1 October 2026 at 12:00:00). The remaining **39 real enrolments were made by 24 students**. All 39 are in paid courses and each has a Paid order for that course.
- **Cause:** the reviews endpoint created an account for any unknown name and never checked enrolment. The prerequisite check existed only on the free-enrolment path, so paid courses bypassed it.
- **Status:** both gaps are fixed in commits `aab4e58` and `83918db`, with tests in `3785387` and `7523288`. The suspicious rows and violating enrolments **remain in the database**. Nothing has been deleted or revoked.

## 1 Suspicious reviews

### Criteria

A review is flagged if it meets either criterion.

- **A. Not enrolled.** No row in `enrollments` for that student and course. Threshold: none. This is exact.
- **B. Burst from new accounts.** The review is 5 stars, the reviewer's `joined_at` is 0 to 7 days before the review's `created_at`, and at least 5 such reviews exist for the same course on the same calendar day. The 7-day window and the 5-review threshold are our judgement.

| Criterion | Reviews flagged |
|---|---|
| A and B (both) | 25 |
| A only (not enrolled, not in a burst) | 40 |
| B only (burst, but enrolled) | 0 |
| **Total flagged** | **65** |

Only one course-day met criterion B: course 17 on 15 September 2026. No other course-day reached five such reviews.

### Burst on course 17

| Measure | Value |
|---|---|
| Course | 17 Accessible Web Design |
| Date | 2026-09-15 |
| Review ids | 1787 to 1811 (25 reviews) |
| First and last review | 2026-09-15 09:14:23 to 2026-09-15 10:48:04 (about 94 minutes) |
| Stars | all 5 |
| Reviewers | 25 accounts, all joined between 2026-09-13 08:24:00 and 2026-09-13 19:45:00 |
| Reviewers enrolled in course 17 | 0 |

### The 40 non-burst reviews (not enrolled only)

These reviewers did not join recently and did not enrol. None is part of the burst.

| Review | Course | Student (id) | Stars | Date |
|---|---|---|---|---|
| 796 | 2 JavaScript for the Web | Oscar Haddad (56) | 2 | 2026-03-18 01:23:00 |
| 1096 | 2 JavaScript for the Web | Dev Nguyen (148) | 4 | 2026-05-20 13:02:12 |
| 402 | 3 UI Design Basics | Olga Tanaka (55) | 4 | 2026-01-06 14:28:46 |
| 894 | 3 UI Design Basics | Oscar Bakker (106) | 1 | 2026-04-07 22:42:33 |
| 769 | 6 Deploying Apps with Containers | Ben Haddad (173) | 5 | 2026-03-09 02:02:24 |
| 1481 | 7 Python for Beginners | Ben Kowalski (115) | 4 | 2026-07-18 15:34:37 |
| 1771 | 8 Object-Oriented Design in C# | Kavya Okafor (266) | 3 | 2026-09-11 21:55:56 |
| 5 | 10 React Fundamentals | Rohan Ali (154) | 5 | 2025-10-22 19:40:57 |
| 905 | 10 React Fundamentals | Ibrahim Fernandez (211) | 5 | 2026-04-10 07:00:55 |
| 1465 | 11 Git and Team Workflows | Victor Ivanova (38) | 4 | 2026-07-15 00:33:32 |
| 818 | 12 Testing Your Code | Ibrahim Larsen (70) | 1 | 2026-03-23 12:34:18 |
| 87 | 18 Motion and Interaction Design | Isla Okafor (176) | 5 | 2025-11-15 06:38:39 |
| 1540 | 18 Motion and Interaction Design | Hiro Ivanova (214) | 4 | 2026-07-30 19:21:21 |
| 81 | 19 Excel for Analysts | Victor Abdi (7) | 1 | 2025-11-13 07:38:21 |
| 646 | 20 Statistics for Everyone | Sven Bakker (64) | 1 | 2026-02-10 14:16:36 |
| 918 | 21 Data Visualization with Charts | Aarav Okafor (82) | 5 | 2026-04-12 17:09:44 |
| 1690 | 21 Data Visualization with Charts | Uma Abdi (23) | 3 | 2026-08-28 14:43:48 |
| 1873 | 21 Data Visualization with Charts | Mia Bakker (255) | 5 | 2026-09-22 23:43:21 |
| 1003 | 22 Python for Data Analysis | Olga Tanaka (55) | 5 | 2026-05-01 05:21:53 |
| 1405 | 22 Python for Data Analysis | Rohan Dubois (60) | 3 | 2026-07-03 03:17:07 |
| 1530 | 23 Machine Learning Basics | Sven Nguyen (91) | 5 | 2026-07-29 04:33:09 |
| 1628 | 23 Machine Learning Basics | Kavya Haddad (196) | 5 | 2026-08-18 15:53:55 |
| 655 | 24 Data Warehousing Essentials | Mia Haddad (143) | 3 | 2026-02-12 00:30:02 |
| 1247 | 24 Data Warehousing Essentials | Jamal Cohen (149) | 5 | 2026-06-09 20:38:54 |
| 1572 | 24 Data Warehousing Essentials | Farah Haddad (16) | 5 | 2026-08-06 14:08:53 |
| 709 | 25 Kubernetes in Practice | Olga Abdi (47) | 3 | 2026-02-23 14:27:07 |
| 774 | 26 Serverless Applications | Kavya Reyes (205) | 2 | 2026-03-09 15:22:04 |
| 1665 | 26 Serverless Applications | Sven Jensen (152) | 3 | 2026-08-23 14:55:05 |
| 1817 | 28 Infrastructure as Code | Nadia Tanaka (163) | 5 | 2026-09-15 22:41:53 |
| 1270 | 29 Project Management Basics | Sara Dubois (190) | 2 | 2026-06-12 12:50:58 |
| 156 | 30 Agile and Scrum | Aditi Ivanova (18) | 4 | 2025-12-02 03:54:02 |
| 1050 | 30 Agile and Scrum | Victor Abdi (7) | 4 | 2026-05-09 11:00:09 |
| 1693 | 32 Business Writing | Ruth Patel (4) | 1 | 2026-08-29 08:57:31 |
| 1346 | 34 Leading Remote Teams | Jamal Costa (40) | 3 | 2026-06-22 23:59:56 |
| 1459 | 34 Leading Remote Teams | Kavya Reyes (205) | 5 | 2026-07-14 02:10:22 |
| 370 | 35 Security Fundamentals | Lucia Reyes (189) | 1 | 2025-12-29 12:10:51 |
| 1889 | 36 Web Application Security | Rhea Mehta (15) | 5 | 2026-09-24 12:49:11 |
| 1958 | 38 Network Defense Basics | Ruth Patel (4) | 5 | 2026-09-29 10:08:41 |
| 1676 | 39 Cloud Security | Tara Brown (225) | 5 | 2026-08-26 13:47:36 |
| 1651 | 40 Incident Response | Sara Ali (130) | 4 | 2026-08-21 14:46:15 |

## 2 Rating before and after, per affected course

The 28 courses with at least one flagged review are listed below. "Before" is every review the course has today. "After" leaves out the 65 flagged reviews. The last column counts how many of the removed reviews were 5-star.

| Course id | Course | Reviews before | Avg before | Reviews after | Avg after | Removed | of which 5-star |
|---|---|---|---|---|---|---|---|
| 17 | Accessible Web Design | 78 | 3.69 | 53 | 3.08 | 25 | 25 |
| 21 | Data Visualization with Charts | 48 | 4.13 | 45 | 4.11 | 3 | 2 |
| 24 | Data Warehousing Essentials | 50 | 4.40 | 47 | 4.40 | 3 | 2 |
| 2 | JavaScript for the Web | 45 | 4.38 | 43 | 4.44 | 2 | 0 |
| 3 | UI Design Basics | 19 | 3.00 | 17 | 3.06 | 2 | 0 |
| 10 | React Fundamentals | 53 | 4.30 | 51 | 4.27 | 2 | 2 |
| 18 | Motion and Interaction Design | 83 | 4.04 | 81 | 4.02 | 2 | 1 |
| 22 | Python for Data Analysis | 28 | 3.14 | 26 | 3.08 | 2 | 1 |
| 23 | Machine Learning Basics | 21 | 4.14 | 19 | 4.05 | 2 | 2 |
| 26 | Serverless Applications | 37 | 4.16 | 35 | 4.26 | 2 | 0 |
| 30 | Agile and Scrum | 106 | 3.82 | 104 | 3.82 | 2 | 0 |
| 34 | Leading Remote Teams | 21 | 3.67 | 19 | 3.63 | 2 | 1 |
| 6 | Deploying Apps with Containers | 14 | 3.93 | 13 | 3.85 | 1 | 1 |
| 7 | Python for Beginners | 103 | 3.78 | 102 | 3.77 | 1 | 0 |
| 8 | Object-Oriented Design in C# | 17 | 4.35 | 16 | 4.44 | 1 | 0 |
| 11 | Git and Team Workflows | 37 | 3.92 | 36 | 3.92 | 1 | 0 |
| 12 | Testing Your Code | 32 | 3.78 | 31 | 3.87 | 1 | 0 |
| 19 | Excel for Analysts | 29 | 4.14 | 28 | 4.25 | 1 | 0 |
| 20 | Statistics for Everyone | 101 | 4.22 | 100 | 4.25 | 1 | 0 |
| 25 | Kubernetes in Practice | 46 | 4.46 | 45 | 4.49 | 1 | 0 |
| 28 | Infrastructure as Code | 15 | 3.67 | 14 | 3.57 | 1 | 1 |
| 29 | Project Management Basics | 51 | 3.75 | 50 | 3.78 | 1 | 0 |
| 32 | Business Writing | 34 | 4.00 | 33 | 4.09 | 1 | 0 |
| 35 | Security Fundamentals | 105 | 3.48 | 104 | 3.50 | 1 | 0 |
| 36 | Web Application Security | 11 | 4.27 | 10 | 4.20 | 1 | 1 |
| 38 | Network Defense Basics | 11 | 4.45 | 10 | 4.40 | 1 | 1 |
| 39 | Cloud Security | 10 | 3.70 | 9 | 3.56 | 1 | 1 |
| 40 | Incident Response | 11 | 4.55 | 10 | 4.60 | 1 | 0 |

- **Course 17 is the only course that moves materially.** Its average falls by 0.61, from 3.69 to 3.08, with 25 of 78 reviews removed.
- **Every other course moves by 0.14 or less.** The largest changes are on small samples, such as Cloud Security (10 reviews, 3.70 to 3.56).
- The 65 removed reviews total 25 + 40 across the 28 courses. In total, 41 of the removed reviews were 5-star.

## 3 How the system allowed it

**Reviews.** `ReviewService.AddReview` called `StudentDirectory.GetOrCreate(name)`. Any unknown name created a new student with `JoinedAt` set to now. The method then saved the review without checking that the student was enrolled. The reviews endpoint had no rate limiting, and the database has no constraint linking reviews to enrolments. Together these allow a burst of new accounts to post 5-star reviews on a course they never bought.

**Consistency with the data.** The seed data inserts rows directly, so these historic rows were not written through the API. They are consistent with exactly these gaps: accounts created on the review path two days before reviewing, all 5 stars, all within 94 minutes. We make no claim about real-world provenance.

**Enrolment.** The prerequisite check lived only in `EnrollmentService.EnrollFree`, which serves free courses and rejects paid ones. Paid courses are enrolled by `ShopService.Checkout`, which called the raw `EnrollmentService.Enroll`. That method had no prerequisite check. `ShopService.AddToCart` had none either. The result of `Enroll` was ignored, so a refusal would have passed silently. None of the 39 real violations is in a free course, which is consistent with the free-path check having worked.

## 4 Prerequisite findings

### Totals

| Measure | Value |
|---|---|
| Enrolments that broke a prerequisite | 47 |
| E2E test enrolments (excluded from the figures below) | 8 |
| **Real enrolments that broke a prerequisite** | **39** |
| Real students involved | 24 |
| Real enrolments with a Paid order for that course | 39 of 39 |
| Real enrolments in paid courses | 39 of 39 |

A violation means the student had not completed every lesson of the prerequisite course by the enrolment time. We judge this by `lesson_progress.completed_at <= enrolled_at`.

The 8 E2E rows belong to students named "E2E Refund ...", all enrolled in course 6 at 2026-10-01 12:00:00. They are test artefacts from the Playwright refund tests, as described in `reports/refund-audit.md`. They are removed by `bash db/reset.sh`.

### By course (real enrolments only)

"No progress" means no lesson of the prerequisite was completed at any time. "Partly done, not finished" means some lessons were completed, but not all, as of today. "Finished later" means the student completed every prerequisite lesson, but only after enrolling.

| Course id | Course | Prerequisite (id) | Violations | No progress at all | Partly done, not finished | Finished later |
|---|---|---|---|---|---|---|
| 6 | Deploying Apps with Containers | Cloud Fundamentals (5) | 1 | 0 | 0 | 1 |
| 8 | Object-Oriented Design in C# | Intro to C# Programming (1) | 5 | 2 | 0 | 3 |
| 9 | Building REST APIs with .NET | Intro to C# Programming (1) | 2 | 0 | 0 | 2 |
| 15 | Design Systems | User Research Essentials (16) | 1 | 0 | 0 | 1 |
| 22 | Python for Data Analysis | Python for Beginners (7) | 9 | 3 | 1 | 5 |
| 23 | Machine Learning Basics | Statistics for Everyone (20) | 3 | 0 | 0 | 3 |
| 25 | Kubernetes in Practice | Cloud Fundamentals (5) | 6 | 2 | 1 | 3 |
| 28 | Infrastructure as Code | Cloud Fundamentals (5) | 4 | 1 | 0 | 3 |
| 31 | Product Management 101 | Agile and Scrum (30) | 4 | 1 | 0 | 3 |
| 36 | Web Application Security | Security Fundamentals (35) | 4 | 1 | 0 | 3 |
| **Total** | | | **39** | **10** | **2** | **27** |

Finished later is the largest group (27 of 39). Twelve enrolments (10 with no progress, 2 partly done) had not finished the prerequisite as of today.

One enrolment is a partial case at the moment of enrolling. Enrolment 423 (Chen Jensen, course 23 Machine Learning Basics, enrolled 2025-11-28 01:04:34) had completed 6 of 9 lessons of Statistics for Everyone. Chen finished the prerequisite on 2025-12-01 08:46:03, three days after enrolling.

### How it happened

The prerequisite check was present only on the free path, so a paid course with a prerequisite could be bought without the prerequisite being finished. Every violation is in a paid course with a Paid order for that course. None of those orders is refunded. That pattern matches the gap in `ShopService.Checkout` described in section 3.

## 5 Fixes and commits

### `aab4e58` Require enrolment to review a course

- `ReviewService.AddReview` now uses `StudentDirectory.Find`, not `GetOrCreate`, so no account is created for an unknown name.
- A review is rejected unless the student exists and is enrolled in the course. The message is "You must be enrolled in this course to review it."
- `ReviewEndpoints` returns HTTP 403 for that message.

### `83918db` Enforce prerequisites on every enrolment path

- `EnrollmentService.PrerequisiteError` is now a shared check, called from `Enroll` itself, so every route through `Enroll` is covered.
- `ShopService.AddToCart` and `ShopService.Checkout` both run the check before any order or payment is created.
- `Checkout` no longer ignores the result of `Enroll`. After validation, a refusal in the enrolment loop throws an `InvalidOperationException`, because it should not happen.
- `Enroll` and `AddToCart` use `Find`, not `GetOrCreate`, so a refused request does not leave a new student row behind.

## 6 Tests

- **`3785387` Add review enrolment tests.** Seven tests: five service tests in `backend/LearnHub.Tests/ReviewTests.cs` and two API tests in `backend/LearnHub.Tests/ApiTests.cs`.
- **`7523288` Add prerequisite enforcement tests.** Six tests in `backend/LearnHub.Tests/PrerequisiteTests.cs`.
- **Full backend suite: 117 tests, all passing** (`cd backend && dotnet test`).
- **Against the pre-fix code,** five of the review tests and four of the prerequisite tests fail, so they prove the fixes. The success-path tests pass on both versions and act as regression guards.
- **What they cover.** Reviews: an unenrolled student, an unknown name (no account is created), a 5-star burst from new names, and a student enrolled in a different course are all rejected, and the API returns 403. Prerequisites: a paid course is refused at add-to-cart, at checkout (no order, no enrolment, no coupon use) and on the raw `Enroll` path, a partial prerequisite (3 of 4 lessons) is still refused, and a student who finished the prerequisite can buy the course.

## 7 Existing data and recommended follow-up

The suspicious rows and violating enrolments remain in the database. Nothing has been deleted, revoked or edited.

**(a) List the suspicious review ids (read-only).** This should return 65 rows. The 25 burst reviews are all inside this set, so the enrolment check alone captures every flagged review.

```sql
SELECT r.id, r.course_id, r.student_id, r.stars, r.created_at
FROM reviews r
WHERE NOT EXISTS (
  SELECT 1 FROM enrollments e
  WHERE e.student_id = r.student_id AND e.course_id = r.course_id
)
ORDER BY r.id;
```

**(b) Remove the flagged reviews. NOT EXECUTED.** Run this only after the owner has reviewed section 1, and after taking a backup. The derived table is needed because MySQL does not allow a `DELETE` to select from its own target table directly.

```sql
-- NOT EXECUTED. Do not run without owner approval, and take a backup first.
-- Expected rows affected: 65. Course 17 average would then be 3.08.
-- DELETE FROM reviews
-- WHERE id IN (
--   SELECT id FROM (
--     SELECT r.id
--     FROM reviews r
--     WHERE NOT EXISTS (
--       SELECT 1 FROM enrollments e
--       WHERE e.student_id = r.student_id AND e.course_id = r.course_id
--     )
--   ) AS flagged
-- );
```

**Recommendation for the 47 enrolments.** Review them with the owner rather than revoking them. The 39 real students paid for their courses, so revoking access is a customer and finance decision, not a database fix. The 8 E2E rows need no decision and disappear on the next `bash db/reset.sh`.

**Recommendation for the reviews.** Decide with the owner whether to remove the 65 reviews, or to keep them and flag them. Course 17's rating is the one that matters most to the business.

## 8 Limitations

- **No database-level constraint.** The database does not link reviews to enrolments, and it does not enforce prerequisites. A script or direct SQL can still write both. We recommend a constraint, but it should wait until the seed data is corrected, or the seed will fail to load.
- **The burst thresholds are a judgement.** The 7-day window and the 5-review threshold were chosen by us. A different threshold would change the count of criterion B. We have not tested alternative thresholds.
- **Historic enrolment timing.** We judge violations by comparing `lesson_progress.completed_at` with `enrolled_at`. This assumes the timestamps are accurate. Lesson completion may have been recorded later than it happened, which would make some violations look like "finished later" when they were not.
- **Checkout is not transactional.** The enrolment loop in `Checkout` is not wrapped in a transaction. It is protected by the validation step and the `InvalidOperationException` guard, but a failure part-way through could still leave a partial result.
- **The 40 are not enrolled, but not proven fake.** They have no enrolment and no order for the course. That is evidence, not proof of fraud.
- **Students are identified by name, with no authentication.** The enrolment check does not authenticate the caller. A determined person could still post or enrol using a real student's name.

## Appendix: Method

- **Population.** All 2,000 reviews. For prerequisites, all enrolments in courses with a `course_prerequisites` row.
- **Criterion A.** `NOT EXISTS` on `enrollments` for the review's student and course.
- **Criterion B.** Reviews with `stars = 5`, where `students.joined_at` is 0 to 7 days before `reviews.created_at`. Grouped by course and calendar day (`DATE(created_at)`), keeping groups of 5 or more.
- **Before and after.** Averages are over all reviews per course. "After" excludes the 65 flagged reviews.
- **Prerequisite violation.** Distinct prerequisite lessons completed on or before `enrolled_at`, compared with the total lessons in the prerequisite course. A violation is when that count is less than the total. "No progress" means no prerequisite lesson was ever completed. "Finished later" means every prerequisite lesson was completed, at a time after enrolment.
- **E2E exclusion.** Students whose `full_name` begins with "E2E Refund". This matches the 8 rows in course 6.
- **Read-only.** All queries were `SELECT`. No table was written to.
