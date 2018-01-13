using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using MySql.Data.MySqlClient;

using NFX.DataAccess.CRUD;
using NFX.DataAccess.MySQL;
using NFX.Scripting;

namespace NFX.ITest.CRUD
{
  [Runnable]
  public class MySQLTests2 : IRunnableHook, IRunHook
  {
    private const string ASSEMBLY = "NFX.ITest";
    private string m_ConnectString;

    [Run]
    public void InsertRead_TypedRow()
    {
      var row = makeDefaultPatient();

      Row result;
      using (var ds = makeDataStore())
      {
        ds.Insert(row);
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        var qry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "%ov") };
        result = ds.LoadOneRow(qry);
      }

      Aver.IsTrue(result is Patient);
      Aver.AreEqual(row.First_Name, result["First_Name"].AsString());
      Aver.AreEqual(row.Last_Name, result["Last_Name"].AsString());
      Aver.AreEqual(row.Last_Name, result["Last_Name"].AsString());
      Aver.AreEqual(row.SSN, result["SSN"].AsString());
      Aver.AreEqual(row.Amount, result["Amount"].AsDecimal());
    }

    [Run]
    public void Delete_TypedRow()
    {
      var row = makeDefaultPatient();

      using (var ds = makeDataStore())
      {
        ds.Insert(row);
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;

        var qry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "Ivanov") };
        var result = ds.LoadEnumerable(qry).ToArray();
        Aver.AreEqual(1, result.Length);

        ds.Delete(result[0]);

        result = ds.LoadEnumerable(qry).ToArray();
        Aver.AreEqual(0, result.Length);
      }
    }

    [Run]
    public void UpdateWithoutFetch()
    {
      var row = makeDefaultPatient();

      using (var ds = makeDataStore())
      {
        ds.Insert(row);
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;

        // update amount to 110.99M
        var qry = new Query("CRUD.Patient.UpdateAmount") { new Query.Param("pSSN", row.SSN) };
        ds.ExecuteWithoutFetch(qry);

        var loadQry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "Ivanov") };
        var result = ds.LoadRow(loadQry);

        Aver.IsTrue(result is Patient);
        Aver.AreEqual(110.99M, result.Amount);
      }
    }

    [Run]
    public void UpsertRow_Insert()
    {
      var row1 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Ivanov",
        DOB = new DateTime(1980, 8, 29),
        SSN = "123456",
        NPI = "5478",
        Amount = 10.23M
      };
      var row2 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Petrovich",
        DOB = new DateTime(1970, 3, 8),
        SSN = "156486",
        NPI = "5278",
        Amount = 10.23M
      };

      using (var ds = makeDataStore())
      {
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        ds.Insert(row1);

        var loadQry = new Query<Doctor>("CRUD.Doctor.List") { new Query.Param("pSSN", "%") };
        var result1 = ds.LoadOneRowset(loadQry).ToList();

        Aver.AreEqual(1, result1.Count);

        ds.Upsert(row2);
        var result2 = ds.LoadEnumerable(loadQry).OrderBy(r => r["COUNTER"].AsLong()).ToArray();

        Aver.AreEqual(2, result2.Length);
        Aver.AreObjectsEqual(row1.SSN, result2[0]["SSN"]);
        Aver.AreObjectsEqual(row2.SSN, result2[1]["SSN"]);
      }
    }

    [Run]
    public void UpsertRow_Update()
    {
      var row1 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Ivanov",
        DOB = new DateTime(1980, 8, 29),
        SSN = "123456",
        NPI = "5478",
        Amount = 10.23M
      };
      var row2 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Petrovich",
        DOB = new DateTime(1970, 3, 8),
        SSN = "123456",
        NPI = "5278",
        Amount = 10.23M
      };

      using (var ds = makeDataStore())
      {
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        ds.Insert(row1);

        var loadQry = new Query<Doctor>("CRUD.Doctor.List") { new Query.Param("pSSN", "%") };
        var result = ds.LoadRow(loadQry);

        Aver.AreEqual(row1.Last_Name, result.Last_Name);

        // it makes update due to unique index on SSN column
        ds.Upsert(row2);
        result = ds.LoadRow(loadQry);

        Aver.AreEqual(row2.Last_Name, result.Last_Name);
      }
    }

    [Run]
    public void ManyRowsInsertAndRead()
    {
      using (var ds = makeDataStore())
      {
        var patients = new List<Patient>();
        for(var i=0; i<1000; i++)
        {
          var row = makeDefaultPatient("Ivanov"+i);
          patients.Add(row);
          ds.Insert(row);
        }

        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        var qry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "Ivanov%") };
        var result = ds.LoadEnumerable(qry).OrderBy(p => p.COUNTER);
        Aver.IsTrue(patients.Select(p => p.Last_Name).SequenceEqual(result.Select(r=>r.Last_Name)));
      }
    }

    [Run]
    [Aver.Throws(ExceptionType = typeof(MySQLDataAccessException), Message = "Duplicate entry '123456'", MsgMatch = Aver.ThrowsAttribute.MatchType.Contains)]
    public void IndexViolation ()
    {
      var row1 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Ivanov",
        DOB = new DateTime(1980, 8, 29),
        SSN = "123456",
        NPI = "5478",
        Amount = 10.23M
      };
      var row2 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Petrovich",
        DOB = new DateTime(1970, 3, 8),
        SSN = "123456",
        NPI = "5278",
        Amount = 10.23M
      };

      using (var ds = makeDataStore())
      {
        ds.Insert(row1);
        ds.Insert(row2);
      }
    }

    [Run]
    public void BoolField()
    {
      var row1 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Ivanov",
        DOB = new DateTime(1980, 8, 29),
        SSN = "172486",
        NPI = "5478",
        Amount = 10.23M,
        Is_Certified = true
      };
      var row2 = new Doctor
      {
        First_Name = "Oleg",
        Last_Name = "Petrovich",
        DOB = new DateTime(1970, 3, 8),
        SSN = "293488",
        NPI = "5278",
        Amount = 10.23M,
        Is_Certified = false
      };

      using (var ds = makeDataStore())
      {
        ds.Insert(row1);
        ds.Insert(row2);

        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        var qry = new Query<Doctor>("CRUD.Doctor.List") { new Query.Param("pSSN", "%") };
        var result = ds.LoadEnumerable(qry).OrderBy(p => p.COUNTER).ToArray();

        Aver.AreEqual(2, result.Length);
        Aver.IsTrue(result[0].Is_Certified);
        Aver.IsFalse(result[1].Is_Certified);
      }
    }

    [Run]
    public void JoinedRows()
    {
      var doctor = new Doctor
      {
        First_Name = "Nikolai",
        Last_Name = "Petrovich",
        DOB = new DateTime(1970, 3, 8),
        Phone = "555666777",
        SSN = "293488",
        NPI = "5278",
        Amount = 10.23M
      };
      var patient1 =  makeDefaultPatient("Esenin");
      var patient2 = makeDefaultPatient("Gogol");

      using (var ds = makeDataStore())
      {
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        ds.Insert(doctor);

        var docQry = new Query<Doctor>("CRUD.Doctor.List") { new Query.Param("pSSN", "%") };
        var doc = ds.LoadRow(docQry);
        patient1.C_DOCTOR = doc.COUNTER;

        ds.Insert(patient1);
        ds.Insert(patient2);

        var qry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "%") };
        var result = ds.LoadEnumerable(qry).OrderBy(p => p.COUNTER).ToArray();

        Aver.AreEqual(2, result.Length);
        Aver.AreEqual(doc.COUNTER, result[0].C_DOCTOR);
        Aver.AreEqual(doctor.NPI, result[0].Doctor_ID);
        Aver.AreEqual(doctor.Phone, result[0].Doctor_Phone);
        Aver.IsNull(result[0].Marker);
        Aver.AreEqual(0, result[1].C_DOCTOR);
        Aver.IsNull(result[1].Doctor_ID);
        Aver.IsNull(result[1].Doctor_Phone);
      }
    }

    [Run]
    public void PartialUpdate()
    {
      var patient1 = makeDefaultPatient();
      patient1.City = "Stambul";

      var patient2 = new Patient
      {
        COUNTER = 1,
        First_Name = "Alex",
        Last_Name = "Ivanov",
        DOB = new DateTime(1980, 8, 29),
        SSN = "345678",
        Amount = 10.23M,
        Address1 = "22, Lenina str",
        City = "Salem"
      };

      using (var ds = makeDataStore())
      {
        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        ds.Insert(patient1);

        ds.Update(patient2, filter: "SSN,Address1".OnlyTheseFields());
        var qry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "%") };
        var result = ds.LoadRow(qry);

        Aver.AreEqual(patient1.City, result.City);
        Aver.AreEqual(patient1.First_Name, result.First_Name);
        Aver.AreEqual(patient2.SSN, result.SSN);
        Aver.AreEqual(patient2.Address1, result.Address1);
      }
    }

    [Run]
    public void TwoCursors()
    {
      using (var ds = makeDataStore())
      {
        for(var i = 0; i < 10; i++)
        {
          var row = new TupleData
          {
            COUNTER = i,
            DATA = "Test" + i.ToString()
          };
          ds.Insert(row);
        }

        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        var qry = new Query<TupleData>("CRUD.Tuple.LoadAll");

        using (var cursor1 = ds.OpenCursor(qry))
        using (var cursor2 = ds.OpenCursor(qry))
        {
          Aver.IsFalse(cursor1.Disposed);
          Aver.IsFalse(cursor2.Disposed);

          var enumer1 = cursor1.GetEnumerator();
          for(var j=0; j<3; j++) enumer1.MoveNext();
          Aver.IsNotNull(enumer1.Current);
          Aver.AreEqual(2, enumer1.Current["COUNTER"].AsInt());
          Aver.AreObjectsEqual("Test2", enumer1.Current["DATA"]);

          var enumer2 = cursor2.GetEnumerator();
          Aver.IsNull(enumer2.Current);
          for(var j = 0; j < 6; j++) enumer2.MoveNext();
          Aver.IsNotNull(enumer2.Current);
          Aver.AreEqual(5, enumer2.Current["COUNTER"].AsInt());
          Aver.AreObjectsEqual("Test5", enumer2.Current["DATA"]);

          enumer1.MoveNext();
          Aver.IsNotNull(enumer1.Current);
          Aver.AreEqual(3, enumer1.Current["COUNTER"].AsInt());
          Aver.AreObjectsEqual("Test3", enumer1.Current["DATA"]);
        }
      }
    }

    [Run]
    public void Cursor_RequestWithCondition()
    {
      var patient1 = makeDefaultPatient("Zorkin");
      var patient2 = makeDefaultPatient("Tokugava");

      using (var ds = makeDataStore())
      {
        ds.Insert(patient1);
        ds.Insert(patient2);

        ds.QueryResolver.ScriptAssembly = ASSEMBLY;
        var qry = new Query<Patient>("CRUD.Patient.List") { new Query.Param("LN", "Tokugava") };

        using (var cursor = ds.OpenCursor(qry))
        {
          Aver.IsFalse(cursor.Disposed);
          var cnt = 0;
          foreach(var patient in cursor.AsEnumerableOf<Patient>())
          {
            Aver.AreEqual(patient2.Last_Name, patient.Last_Name);
            Aver.AreEqual(patient2.SSN, patient.SSN);
            cnt++;
          }
          Aver.AreEqual(1, cnt);
        }
      }
    }


    private MySQLDataStore makeDataStore()
    {
      return new MySQLDataStore(m_ConnectString);
    }

    private Patient makeDefaultPatient(string ln = "Ivanov")
    {
      return new Patient
        {
          First_Name = "Oleg",
          Last_Name = ln,
          DOB = new DateTime(1980, 8, 29),
          SSN = ExternalRandomGenerator.Instance.NextScaledRandomInteger(100000,500000).ToString(),
          Amount = 10.23M
        };
    }

    void IRunnableHook.Prologue(Runner runner, FID id)
    {
      var connectionConf = @"nfx{ conneсt-string=$(~NFX_TEST_MYSQL_CONNECT)}".AsLaconicConfig();
      m_ConnectString = connectionConf.AttrByIndex(0).ValueAsString();
    }

    bool IRunnableHook.Epilogue(Runner runner, FID id, Exception error)
    {
      return false;
    }

    bool IRunHook.Prologue(Runner runner, FID id, MethodInfo method, RunAttribute attr, ref object[] args)
    {
      using(var cnn = new MySqlConnection(m_ConnectString))
      {
        cnn.Open();
        using(var cmd = cnn.CreateCommand())
        {
          cmd.CommandText = "TRUNCATE TBL_PATIENT; TRUNCATE TBL_DOCTOR; TRUNCATE TBL_TUPLE;";
          cmd.ExecuteNonQuery();
        }
      }
      return false;
    }

    bool IRunHook.Epilogue(Runner runner, FID id, MethodInfo method, RunAttribute attr, Exception error)
    {
      return false;
    }
  }
}
