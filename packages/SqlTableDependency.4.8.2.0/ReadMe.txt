================================================================================================================================================================
SqlTableDependency NuGet Package
================================================================================================================================================================
Copyright (c) 2015-2016 Christian Del Bianco 
Home site: https://tabledependency.codeplex.com/
Documentation: https://tabledependency.codeplex.com/documentation
Examples: https://tabledependency.codeplex.com/wikipage?title=Examples


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.8.2.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* Fix on DB objects disposition


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.8.1.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* https://tabledependency.codeplex.com/workitem/46


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.8.0.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* http://tabledependency.codeplex.com/workitem/48


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.7.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* http://tabledependency.codeplex.com/discussions/658146


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.6.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/42


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.5.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/41


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.4.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE http://tabledependency.codeplex.com/workitem/40


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.3.1
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:
* Strong Key


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.3.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* Internal fix to END Service Broker comminucation


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.3.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* Internal fix to END Service Broker comminucation


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.2.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/31


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.1.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:
* Event on status change:

using (var tableDependency = new SqlTableDependency<SeatsAvailability>(connectionString, "FlightBookings"))
{
    tableDependency.OnStatusChanged += TableDependency_OnStatusChanged;
    tableDependency.Start();

    Console.WriteLine(@"Waiting for receiving notifications...");
    Console.WriteLine(@"Press a key to stop");
    Console.ReadKey();
    
    tableDependency.Stop();
}
            
private static void TableDependency_OnStatusChanged(object sender, StatusChangedEventArgs e)
{
    Console.WriteLine(@"Status: " + e.Status);
}

BUG FIXES:
* ISSUE http://tabledependency.codeplex.com/workitem/19


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.6.0.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:
* Log:

using (var tableDependency = new SqlTableDependency<Customer>(connectionString, "Customer"))
{
   tableDependency.OnStatusChanged += TableDependency_OnStatusChanged;
   tableDependency.OnChanged += TableDependency_Changed;
   tableDependency.OnError += TableDependency_OnError;
   tableDependency.TraceLevel = TraceLevel.Verbose;
   
   tableDependency.TraceListener = new TextWriterTraceListener(Console.Out);

   OR
 
   tableDependency.TraceListener = new TextWriterTraceListener(File.Create("c:\\temp\\output.txt"));
   
   tableDependency.Start();

   Console.WriteLine(@"Waiting for receiving notifications...");
   Console.WriteLine(@"Press a key to stop");
   Console.ReadKey();
   tableDependency.Stop();
}


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.8.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/discussions/655211


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.7.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/24


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.6.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/27


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.5.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/21


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.4.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/29
* ISSUE https://tabledependency.codeplex.com/workitem/22


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.3.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/18
* ISSUE https://tabledependency.codeplex.com/workitem/17
* ISSUE https://tabledependency.codeplex.com/workitem/16


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.2.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:
* Events only when the value change https://tabledependency.codeplex.com/discussions/650517


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.1.4
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE http://tabledependency.codeplex.com/discussions/649610


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.1.3
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/14


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.1.2
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/10


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.1.1
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/11


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.1.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:
* Possibility to define Encoding. By default this value is set to Encoding.Unicode. 


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.5.0.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:
* Possibility to define DML database trigger, specifying INSERT, DELETE and or UPDATE. The following initialization will generate a trigger only for INSERT 
  operations. This means that SqlTableDependency will receive notification about only Insert and Update operations.

  SqlTableDependency<Item> tableDependency = new SqlTableDependency<Item>(
    ConnectionString,
    TableName,
    mapper,
    (IList<string>)null,
    DmlTriggerType.Insert | DmlTriggerType.Update,
    true);

* Model with System.ComponentModel.DataAnnotations support. Supposing a model as follow:

    [Table("Customers")]
    public class Client
    {
      public long Id { get; set; }
      public string Name { get; set; }
      [Column("Surname")]
      public string FamilyName { get; set; }
    }
  
  It is possible to create a SqlTableDependency without specifying table name and mapper:
  
    SqlTableDependency<Item4> tableDependency =  new SqlTableDependency<Client>(ConnectionString);
    tableDependency.OnChanged += TableDependency_Changed;
    tableDependency.Start();
  
  In case you do not want use data annotation, specify table name parameter constructor as well as ModelToTableMapper parameter.

* Possibility to define an UpdateOf list using lamba expression instead of List<string> only. With the following snippet we will
  receive a notification only when the FamilyName model property is changed (that is when the mapped database table column is
  updated):
    
    var updateOfModel = new UpdateOfModel<Client>();
    updateOfModel.Add(i => i.FamilyName);
    
    SqlTableDependency<Item> tableDependency = new SqlTableDependency<Client>(ConnectionString, updateOfModel);
    tableDependency.OnChanged += TableDependency_Changed;
    tableDependency.Start();
    
    
---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.2.0.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/9

CHANGES:
* .NET Framework 4.5.1 OR later versions (MANDATORY)


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Release 4.1.1.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
BUG FIXES:
* ISSUE https://tabledependency.codeplex.com/workitem/8
    

---------------------------------------------------------------------------------------------------------------------------------------------------------------
Version 4.1.0.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:

* SqlTableDependency can accept a naming convention to use during its initialization. This means that it can reuse database Service Broker and Queue created
  previously. In case those objects are not present, SqlTableDependency create for you. In order to reuse Service Broker and Queue, create SqlTableDependency
  with automatic DatabaseObjectsTeardown constructor parameter set to false, in order to mantains the database objects when SqlTableDependency is disposed.

  Here is an example:I create SqlTableDependency. Then the application is closed. Because SqlTableDependency was create with
  automatic DatabaseObjectsTeardown = false, the queue is still alive and can continue to receive change notification and queuing the messages.

    var namingToUse = "CustomNaming";

    var mapper = new ModelToTableMapper<Check_Model>();
    mapper.AddMapping(c => c.Name, "FIRST name").AddMapping(c => c.Surname, "Second Name");

    using (var tableDependency = new SqlTableDependency<Check_Model>(ConnectionString, TableName, mapper, null, false, namingToUse))
    {
        tableDependency.OnChanged += TableDependency_Changed;
        tableDependency.Start();               
        ....
    }

  When the application restart we simply specify the same database object naming convention to use. SqlTableDependency detects that DB objects are
  in place and so it use them. In case those objects are not present, SqlTableDependency will create for us.

    var namingToUse = "CustomNaming";

    using (var tableDependency = new SqlTableDependency<Check_Model>(ConnectionString, TableName, mapper, null, false, namingToUse))
    {
        tableDependency.OnChanged += TableDependency_Changed;
        tableDependency.Start();
        ....
    }


---------------------------------------------------------------------------------------------------------------------------------------------------------------
Version 4.0.0.0
---------------------------------------------------------------------------------------------------------------------------------------------------------------
NEW FEATURES:   

* SqlTableDependency can accept VARCHAR(MAX), NVARCHAR(MAX), BINARY(8000), VARBINARY(MAX) and XML table column types.