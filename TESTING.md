# Testing the QC program against the requirements PDF

There are three checks. Run them in this order.

| Check | What it catches | How to run |
|---|---|---|
| 1. Rule tests | date conversion, contract-date rule, cut type, naming, percent and schedule math | `dotnet run --project Tests\QC1.Tests` |
| 2. Manual tests below | what each form does and prints, compared to the PDF page | run the program and follow the table |
| 3. `Database\verify.sql` | wrong data that got saved anyway | `sqlcmd -S localhost -E -d QC1 -i Database\verify.sql` |

Check 1 needs no database (in Visual Studio you can also right-click QC1.Tests, Set as Startup Project, F5). All tests should say PASS and the exit code should be 0.
Check 3 needs SQL Server 2016 SP1 or newer (it uses `CREATE OR ALTER`). It lists every rule with
PASS, WARN or FAIL and the first offending row. A FAIL is a bug, either in the form that saved
the data or a missing check.

## Setup

```
sqlcmd -S localhost -E -i Database\schema.sql
sqlcmd -S localhost -E -d QC1 -i Database\sample_data.sql
```

`sample_data.sql` adds the example project from the PDF (Alfa Sanat / Kariran Sule, contract date
1404/05/05) with 4 parts, 6 connections and their links. When the program asks
«پروژه ... ادامه یابد؟», answer Yes to use it. To test section 1 from scratch, answer No instead.

Write down the result of each row (OK / not OK) as you go.

## Section 1: project information (PDF page 1)

| # | Do this | Expected |
|---|---|---|
| 1.1 | Start the program, answer No, leave the project name empty, press ذخیره | message «نام پروژه را وارد کنید.», nothing saved |
| 1.2 | Fill every field in Persian and English: project, client, contractor, contract date, QC manager, QA manager, coordinator, phase count, reported phase | all fields exist on the form |
| 1.3 | Type contract date `1404/13/40` | message that the contract date is invalid |
| 1.4 | Choose a logo with انتخاب لوگو, save | main menu opens, the logo is in the purple banner of every form |
| 1.5 | Open any report, press پیش نمایش و چاپ | logo in the header and as a faint watermark in the middle of the page |
| 1.6 | Close and reopen the program, answer Yes | the same project opens, no second project is created |

## Section 1: parts / Assembly List (PDF page 1-2)

| # | Do this | Expected |
|---|---|---|
| 1.7 | روش اول (موردی): add `2C4`, qty 5, length, weight | row appears in the list |
| 1.8 | Add `2C4` again, then ` 2c4 ` | both refused with «قطعه 2C4 قبلاً ثبت شده است.» (PDF note 2) |
| 1.9 | روش دوم (گروهی): import `Templates\parts_template.xlsx` (name, qty, length, weight) | 2C4 is reported as a duplicate, B1 is added |
| 1.10 | Select a row, حذف قطعه | row leaves the list (PDF note 1) |
| 1.11 | Press تایید نام و کد قطعات, close and reopen the form | the list is the same as before closing |
| 1.12 | Open PT, add part `2C4` with the manual picker | numbers 1 to 5 can be chosen, shown as `2C4  3/5` |

## Section 1: connections / Part List (PDF page 2-3)

| # | Do this | Expected |
|---|---|---|
| 1.13 | Add `mc/9` thickness 10, qty 1, weight 5 | row appears |
| 1.14 | Add `mc/9` again | refused, «قبلاً ثبت شده است» |
| 1.15 | Add a connection with thickness 0 or weight 0 | refused, the field is required |
| 1.16 | Import `Templates\connections_template.xlsx` (code, qty, weight, thickness) | rows added with the right thickness in the ضخامت ورق column |
| 1.17 | Pick part C1/1, tick mc/2 and mc/4, ذخیره لینک‌ها | column «قطعات مرتبط» of mc/2 and mc/4 shows C1/1 |
| 1.18 | Link mc/2 to a second part too | allowed, mc/2 shows both parts |
| 1.19 | Delete a connection that is not used in any report | it disappears with its links |

## Section 2: checklist before and during welding (PDF page 3-5)

| # | Do this | Expected |
|---|---|---|
| 2.1 | Start date `1404/05/04` (a day before the contract), ساخت لیست بازرسی | error, the date can't be before 1404/05/05 |
| 2.2 | Start date `1404/05/05` | accepted (same day as the contract is allowed) |
| 2.3 | Capacity 2, interval before 1, interval during 1, percent 40, generate | 1st and 2nd part instance dated 1404/05/05, 3rd and 4th 1404/05/06, and so on; each «حین» date is one day after its row date |
| 2.4 | Count the rows of C1/1 1/11 (5 linked connections, 40%) | exactly 2 rows (ceil(5 x 0.4)); a different random pair for other instances |
| 2.5 | Look at RF1/6 rows with connection bp/1 (20 mm) | cut type «گرم»; mc rows (8 to 12 mm) show «سرد» |
| 2.6 | Unlink every connection from one part and generate | message listing that part as not linked |
| 2.7 | تایید همه موارد, then تایید و ذخیره گزارش, reopen the form | all ticks and header fields come back |
| 2.8 | Print preview | code QC-FR-12/02, revision 1403/10/01, two cut columns (سرد PL ≤ 15mm, گرم PL > 15mm), part code only on its first row, the three footnotes (* ** ***), signers QC and coordinator |

## Section 3: DT (PDF page 6)

| # | Do this | Expected |
|---|---|---|
| 3.1 | Before date `1404/05/01` | error, before the contract date |
| 3.2 | Before `1404/05/29`, after `1404/05/20` | error, «بعد» before «قبل» |
| 3.3 | Before `1404/05/29`, after empty, add parts, save | saved (after is optional) |
| 3.4 | Print preview | portrait page like the PDF: Final Dimensional Report, QC-FR-03/02, columns NO / Item No. / QTY / Fit up / Welding / Dim. check / A to L, 15 rows, the A to L tolerance pictures under the table, two signers |

## Section 4: VT (PDF page 7)

| # | Do this | Expected |
|---|---|---|
| 4.1 | Header fields | start date, report interval, base metal ST37, filler ER70S-6 & EM11, standard AWS D1.1 |
| 4.2 | Add parts C1/1, RF1/6 | type column shows Column for C1/1 and Rafter for RF1/6 |
| 4.3 | Tick Under cut on one row, result «تعمیر (Repair)» | print shows ticks under Under cut and Repair |
| 4.4 | Result «پذیرفته پس از تعمیر» | print ticks both Repair and Accepted Repair, as in the PDF sample |
| 4.5 | Print preview | Visual Test Report, QC-FR-04/02, 20 rows, three signers (QC, approved by, client) |

## Section 5: UT (PDF page 8-10)

| # | Do this | Expected |
|---|---|---|
| 5.1 | افزودن قطعات, tab روش اول, add C1/1 number 1 | 3 rows W1, W2, W3 (welds per part = 3) |
| 5.2 | Tab روش دوم, tick 2 instances | those two are added |
| 5.3 | Tab روش سوم, 20% | label says how many of all instances will be picked; that many are added, never one twice |
| 5.4 | Add the same instance again | message that it was already in the list |
| 5.5 | افزودن شماتیک قطعه, choose an image | thumbnail shows in the header |
| 5.6 | Print preview | for each instance two pages: page 1 of 2 with equipment (Sonatest, 2 Mhz, D-10+, 8, 220633, 70°, 20x22) and the sketch; page 2 of 2 portrait with the weld table and the abbreviations / axes picture at the bottom |

## Section 6: PT (PDF page 11-12)

| # | Do this | Expected |
|---|---|---|
| 6.1 | Header fields | percent %10 ~ %20, post emulsification, wet, brushed, color visible, 30-32 C, solvent based, 14 min, 10~15 min, 800 |
| 6.2 | Three picking methods and the sketch | same behaviour as 5.1 to 5.5 |
| 6.3 | One row: type L.O.F, result Rep., After Rep. ticked | print shows it like row 2 of the PDF sample |
| 6.4 | Print preview | Liquid Penetrant Inspection Report, defect legend (C, C.C, L.O.F, PO), sketch, judgement, three signers |

## Section 7: NCR (PDF page 13)

| # | Do this | Expected |
|---|---|---|
| 7.1 | Save with an empty description | refused |
| 7.2 | Date before the contract | refused |
| 7.3 | Fill everything (type داخلی, part RF1/6 number 2, stage مونتاژ ملحقات, severity عمده, actions table, inspection table, follow-up person, quantities accepted 1, rework 1), save | appears in the list at the top |
| 7.4 | Select it again | all fields and both tables come back |
| 7.5 | Print preview | PU-FR-16/02, every field of the PDF form, signers: preparer, QC manager, follow-up (مسئول پیگیری), inspector |
| 7.6 | حذف مورد | gone, with its action and inspection rows |

## After the manual tests

Run `Database\verify.sql`. With the data from the steps above you should see:

- no FAIL lines
- WARN for parts you left without connections (step 2.6) and for defective parts without an NCR,
  unless you made one for each repaired VT/PT row and rejected UT weld

If you see a FAIL, the Example column names the row. Open that form, find the row, and check
which step let it through.
