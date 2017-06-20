# Audit table change with WPF, WCF and SqlTableDependency

SqlTableDependency is an open source component that can create a series of database objects used to receive notifications on table record change. When any insert/update/delete operation is detected, a change notification containing the record’s status is sent to SqlTableDependency, thereby eliminating the need of an additional SELECT to update application’s data.

To get notifications, SqlTableDependency provides an on the fly low-level implementation of an infrastructure composed of a table trigger, contracts, messages, queue, service broker and a clean-up stored procedure.

SqlTableDependency class provides access to notifications without knowing anything about the underlying database infrastructure. When a record change happens, this infrastructure notifies SqlTableDependency, which in turn raises a .NET event to subscribers providing the updated record values.
Listen for table change alert
Using the SqlTableDependency is a good way to make your data driven application (whether it be Web or Windows Forms) more efficient by removing the need to constantly re-query your database checking for data changes.

Instead of executing a request from client to the database, we do the reverse: sending a notification from database to clients applications.

The following video show how to build a web application able to send real time notifications to clients. The code is visible below:

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/c2LfyCmy65A/0.jpg)](https://www.youtube.com/watch?v=c2LfyCmy65A)
