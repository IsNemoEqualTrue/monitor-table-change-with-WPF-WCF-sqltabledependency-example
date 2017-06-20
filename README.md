# Audit table change with WPF, WCF and SqlTableDependency

SqlTableDependency is an open source component that can create a series of database objects used to receive notifications on table record change. When any insert/update/delete operation is detected, a change notification containing the record’s status is sent to SqlTableDependency, thereby eliminating the need of an additional SELECT to update application’s data.

To get notifications, SqlTableDependency provides an on the fly low-level implementation of an infrastructure composed of a table trigger, contracts, messages, queue, service broker and a clean-up stored procedure.

SqlTableDependency class provides access to notifications without knowing anything about the underlying database infrastructure. When a record change happens, this infrastructure notifies SqlTableDependency, which in turn raises a .NET event to subscribers providing the updated record values.
Listen for table change alert
Using the SqlTableDependency is a good way to make your data driven application (whether it be Web or Windows Forms) more efficient by removing the need to constantly re-query your database checking for data changes.

**Instead of executing a request from client to the database, we do the reverse: sending a notification from database to clients applications**.

The following video show how to build a web application able to send real time notifications to clients. The code is visible below:

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/c2LfyCmy65A/0.jpg)](https://www.youtube.com/watch?v=c2LfyCmy65A)

## Get notifications on record change using WPF and WCF
This example show how to keep up to date WPF client applications displaying Stock prices. Every WPF client has a grid that needs to be automatically updated whenever a stock price change.

### WCF server application implementing Publish-Subscribe pattern
Let's assume that we have a table as:

```SQL
CREATE TABLE [Stocks] (
	[Code] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL,
	[Price] [decimal](18, 0) NULL)
```

that is continuously update with stock's value from an external thread. We want our WPF application be notified every time a new value is updated without polling periodically the Stocks table. This means we want receive notifications from our database on every table change.

To achieve this, we need a service application that will take care of create a SqlTableDependency object and for every change notification received, forward this new stock price to all interested WPF client applications.

For this we are going to use a WCF service implementing the Publish-Subscribe pattern. This service will act as stock price broker receiving database notifications on any stock price change and in turn will notify subscribed WCF client applications:

![alt text][shema]

[shema]: https://github.com/christiandelbianco/Monitor-table-change-with-WPF-WCF-sqltabledependency/blob/master/Schema-min.png "Notifications"


