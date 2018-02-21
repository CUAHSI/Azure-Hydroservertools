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

Prerequisites
---
- Runs in the Microsoft Azure cloud as a Cloud Service. 
- Requires asp.net 4.5.
- User database and ODM databases required to be SQl server 2008/2012 databases.

Current Features and Functionality
--
- Built for Windows Azure Web deployment not suitable for local deployment 
- Single Sign on with OpenID and Google account
- View all tables in Database with sort and limited search functionality using jquery datatables. Data processing is implemented on backend to enable scaling to larger datasets  
- Asynchronous upload of data to not block UI thread and prevent timeouts 
- Visualization of results in 4 categories: New, Rejected, Updated and Duplicate
- Basic and advanced mode for uploading
- Inline editing for Basic upload

Major Libraries
--
- **Framework**: Microsoft MVC, Web API 
- **Javascript framework**: Jquery & Jquery  UI
- **Responsive design**: Bootstrap
- **CSV Parsing**: CSV Helper
- **Ajax Upload**: Blueimp Jquery Upload
- **Data visualisation**: jQuery Datatables
- **Data Access ORM**: Entity Framework 
- **Object mapping**: Automapper
- **Error handling**: ELMAH




Set-up
---
Application require at database for the usermanagement and one or many ODM 1.1.1 databases. The Usermanagement database stores the log-in information, the connection to the ODM databses and the associated ODM database for a user (google account). Currently only one database can be associated with a user account. Admininstration can be done when login in a as an admininstrator (role).    
After a successful authentication with google snd successful association of the user account with a registered database, the users can upload timeseries data and associated metadata into the ODM database that is associated with this account. 


To Do
----
- Add additional upload templates to simplify upload.
- inprove performance for large datasets > 50000 values. Currently does not scale well. 

Version
------

1.1.2


Contact
--------

**CUAHSI** 
Consortium of Universities for the Advancement of Hydrologic Science, Inc.

150 Cambridge Park Dr.

CAmbridge, MA 02130

Phone(339) 221-5400

[http://www.cuahsi.org]



[http://www.cuahsi.org]:http://www.cuahsi.org
