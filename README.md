# BertScout2025

This is the scouting application for the FIRST FRC 2025 competitions used by Team 133, B.E.R.T.

It is written in .NET 8 C# as a MAUI application. The same application can run on Windows and Android devices (and others, but not tested).

It runs on six Kindle Fire HD 8 (10th generation) tablets, which run Fire OS 7.3.3.1 currently. This is based on Android 9, API Level 28 (Android Pie).

Each Kindle Fire has an assigned position for each match: Red 1, 2, 3 and Blue 1, 2, 3. The scouters each enter data for one robot in every match during the day.

The accumulated data is stored internally in a SQLite database on each Kindle Fire, and transmitted manually at the end of the day into an Airtable cloud spreadsheet. This allows the scouting team to view all the results at the end of the day and determine the desired alliance members.
