# Providers

**NFX** data access providers provide implementation of contracts for particular technologies.

NFX currently supports the following providers out-of-box:

* CRUD MySQL

* CRUD MongoDB

* Direct MongoDB Driver (BSON level)

* Mongo Log support

* MSSQL Log support

* Erlang Mnesia NFX - OTP wrapper around Mnesia database

* Erlang NFX - OTP custom data feeder from Erlang

The providers lookup particular query scripts using suffix notation, for example: "GetCustomerBySSN.my.sql" vs "GetCustomerBYSSN.ora.sql".