using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Week13Day3Assignment.Model;

namespace Week13Day3Assignment.Services
{
    public class StudentDbService : INotifyPropertyChanged
    {

        private const string ConnectionString = @"Data Source=localhost;Initial Catalog=WPF;Integrated Security=True";

        public Student Student;

        public int RecordNumber { get; private set; }
        public int TotalRecords { get; private set; }

        public StudentDbService()
        {
            LoadInitialView();
        }

        private void LoadInitialView()
        {
            RecordNumber = 1;
            TotalRecords = GetTotalRecordFromDb();

            SelectFromDb();

        }

        public void Insert()
        {
            RecordMode = RecordMode.Insert;
            Student = new Student();

            NotifyStudentRecordChanged();
        }

        public void Update()
        {
            RecordMode = RecordMode.Update;
        }

        public void Save()
        {
            if (RecordMode == RecordMode.Insert)
            {
                InsertIntoDb();
                TotalRecords++;
                RecordNumber = TotalRecords;
            }
            else if (RecordMode == RecordMode.Update)
                UpdateInDb();
            else
                return;

            StopEditing();
        }

        public void StopEditing()
        {
            if (RecordMode == RecordMode.Insert)
                SelectFromDb();

            RecordMode = RecordMode.View;
        }

        public void First()
        {
            if (RecordNumber != 1)
            {
                RecordNumber = 1;
                SelectFromDb();
            }
        }

        public void Previous()
        {
            if (RecordNumber > 1)
            {
                RecordNumber--;
                SelectFromDb();
            }
        }

        public void Next()
        {
            if (RecordNumber < TotalRecords)
            {
                RecordNumber++;
                SelectFromDb();
            }
        }

        public void Last()
        {
            if (RecordNumber != TotalRecords)
            {
                RecordNumber = TotalRecords;
                SelectFromDb();
            }
        }

        private static int GetTotalRecordFromDb()
        {
            var query = $"select count(FirstName) from Students";
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);

            connection.Open();

            using var reader = command.ExecuteReader();

            reader.Read();

            var recordCount = (int)reader[0];
            return recordCount;
        }

        public void SelectFromDb()
        {
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand("SelectStudent", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@RecordNumber", RecordNumber);

            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();

                Student = new Student
                {
                    
                    FirstName=reader.GetString(0),
                    LastName=reader.GetString(1),
                    Email=reader.GetString(2),
                    PhoneNo=reader.GetString(3),
                };
            }
            else
                Student = new Student();

            NotifyStudentRecordChanged();
        }

        private void NotifyStudentRecordChanged()
        {
            OnPropertyChanged(nameof(Student));
            OnPropertyChanged(nameof(RecordNumber));
            OnPropertyChanged(nameof(TotalRecords));
        }

        private void InsertIntoDb()
        {
            var query = "InsertStudent";

            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);

            command.CommandType = CommandType.StoredProcedure;

            connection.Open();

            command.Parameters.AddWithValue("@FirstName", Student.FirstName);
            command.Parameters.AddWithValue("@LastName", Student.LastName);
            command.Parameters.AddWithValue("@Email", Student.Email);
            command.Parameters.AddWithValue("@Email", Student.PhoneNo);

            command.ExecuteNonQuery();
        }


        private void UpdateInDb()
        {
            var query = $"UpdateStudent";
            using var connection = new SqlConnection(ConnectionString);
            using var command = new SqlCommand(query, connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@FirstName", Student.FirstName);
            command.Parameters.AddWithValue("@LastName", Student.LastName);
            command.Parameters.AddWithValue("@Email", Student.Email);
            command.Parameters.AddWithValue("@Email", Student.PhoneNo);

            connection.Open();

            command.ExecuteNonQuery();
        }

        #region RecordViewState

        private RecordMode _recordMode = RecordMode.View;
        public RecordMode RecordMode
        {
            get
            {
                return _recordMode;
            }
            set
            {
                _recordMode = value;
                OnPropertyChanged(nameof(InputIsReadOnly));
                OnPropertyChanged(nameof(InputIsEditable));
            }
        }

        public bool InputIsReadOnly
        {
            get
            {
                return RecordMode != RecordMode.Insert && RecordMode != RecordMode.Update;
            }
        }

        public bool InputIsEditable => !InputIsReadOnly;

        #endregion


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
