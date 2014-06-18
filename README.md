Azure-Hydroservertools
======================

Azure Hydroservertools
---
Description
-----

**HydroServer** is a suite of application for sharing time series water data. It has three components: 
- An instance of the Observations Data Model (ODM) SQL Database in the Azure Cloud
- An application for uploading data to the ODM Database using CSV templates
- A WaterOneFlow Web Service for exposing the database

The motivation for this project is the replacement of several windows desktop based applications with a solution that is entirely web based. This project provides the code for the uploading data client.



Current Features and Functionality
--
- Built for Windows Azure Web deployment not suitable for local deployment 
- Single Sign on with OpenID and Google account
- View all tables in Database with sort and limited search functionality. Data processing is implemented on backend to enable scaling to larger datasets  
- Maintenance of state using caching using azure caching 
- Asynchronous upload of data to not block UI thread and prevent timeouts 
- Visualization of results in 4 categories: new, Rejected, Updated and Duplicate

Major Libraries
--
- **Framework**: Microsoft MVC, Web API 
- **Javascript framework**: Jquery & Jquery  UI
- **Responsive design**: Bootstrap
- **CSV Parsing**: CSV Helper
- **Ajax Upload**: Blueimp Jquery Upload
- **Data visualisation**: jQuery Datatables
- **Caching**: windows azure caching
- **Data Access ORM**: Entity Framework 
- **Object mapping**: Automapper
- **Eroor handling**: ELMAH


Set-up
---
Application requires at least 2 databases. One for the usermanagemnt aand one or many ODM 1.1.1 databases. They are linked through a xml file mapping users to login information. 
After a successful authentication with google users can upload timeseries data into the ODM database that is associated with this account. 
The 

To Do
----
- Build out user management to enable a user to select multiple databses interactively and add roles.
- Add additional upload templates to simplify upload.
 

Version
------

1.0

Contact
--------

**CUAHSI** 
Consortium of Universities for the Advancement of Hydrologic Science, Inc.

196 Boston Ave

Medford, MA 02155

Phone(339) 221-5400

[http://www.cuahsi.org]



[http://www.cuahsi.org]:http://www.cuahsi.org
